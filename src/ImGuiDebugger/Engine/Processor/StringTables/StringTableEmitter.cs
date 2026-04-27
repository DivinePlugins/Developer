namespace Divine.Plugin.Engine.Processor.StringTables;

using System;

using Divine.Plugin.Engine.IO;
using Divine.Plugin.Engine.Model;
using Divine.Plugin.Engine.Utils;
using Divine.Protobufs.Dota2;

using Google.Protobuf;

using Snappy.Sharp;

internal sealed class StringTableEmitter
{
    private const int MAX_NAME_LENGTH = 0x400;

    private const int KEY_HISTORY_BITS = 5;
    private const int KEY_HISTORY_SIZE = (1 << KEY_HISTORY_BITS);
    private const int KEY_HISTORY_MASK = (KEY_HISTORY_SIZE - 1);

    private int numTables = 0;

    //private HashSet<string> requestedTables = new();
    //private HashSet<string> updateEventTables = new();

    private readonly StringTables stringTables = new();

    private readonly byte[] tempBuf = new byte[0x4000];

    public event Action<StringTable, int, string, ByteString>? updateEvent;

    public event Action<int, StringTable>? evCreated;

    public event Action? evClear;

    private void raise(StringTable table, int index, string name, ByteString value)
    {
        updateEvent?.Invoke(table, index, name, value);
    }

    private bool isProcessed(string tableName)
    {
        return tableName == "instancebaseline";
        //return tableName.Contains("*") || tableName.Contains(tableName);
    }

    //[OnMessage]
    public void clearAllStringTables(CSVCMsg_ClearAllStringTables msg)
    {
        numTables = 0;

        stringTables.clearAllStringTables();
        evClear?.Invoke();
    }

    //[OnMessage]
    public void onCreateStringTable(CSVCMsg_CreateStringTable message)
    {
        if (isProcessed(message.Name))
        {
            StringTable table = new StringTable(
                message.Name,
                null,
                message.UserDataFixedSize,
                message.UserDataSize,
                message.UserDataSizeBits,
                message.Flags);

            ByteString data = message.StringData;
            if (message.DataCompressed)
            {
                byte[] dst;
                if (EngineData.getBuildNumber() != -1 && EngineData.getBuildNumber() <= 962)
                {
                    dst = LZSS.unpack(data);
                }
                else
                {
                    dst = Snappy.Uncompress(data.ToByteArray()); // data.Span
                }

                data = ByteString.CopyFrom(dst);
            }

            decodeEntries(table, data, message.NumEntries);
            table.markInitialState();

            stringTables.onStringTableCreated(numTables, table);
            evCreated?.Invoke(numTables, table);
        }

        numTables++;
    }

    //[OnMessage]
    public void onUpdateStringTable(CSVCMsg_UpdateStringTable message)
    {
        var table = stringTables.forId(message.TableId);
        if (table != null)
        {
            decodeEntries(table, message.StringData, message.NumChangedEntries);
        }
    }

    private void decodeEntries(StringTable table, ByteString encodedData, int numEntries)
    {
        BitStream stream = BitStream.createBitStream(encodedData);
        var keyHistory = new string[KEY_HISTORY_SIZE];

        int index = -1;
        for (int i = 0; i < numEntries; i++)
        {
            // read index
            if (stream.readBitFlag())
            {
                index++;
            }
            else
            {
                index += stream.readVarUInt() + 2;
            }

            // read name
            string name = null;
            if (stream.readBitFlag())
            {
                if (stream.readBitFlag())
                {
                    int @base = i > KEY_HISTORY_SIZE ? i : 0;
                    int offs = stream.readUBitInt(5);
                    int len = stream.readUBitInt(5);
                    string str = keyHistory[(@base + offs) & KEY_HISTORY_MASK];
                    name = str[..len] + stream.readString(MAX_NAME_LENGTH);
                }
                else
                {
                    name = stream.readString(MAX_NAME_LENGTH);
                }
            }

            // read value
            ByteString data = null;
            if (stream.readBitFlag())
            {
                bool isCompressed = false;
                int bitLength;
                if (table.getUserDataFixedSize())
                {
                    bitLength = table.getUserDataSizeBits();
                }
                else
                {
                    if ((table.getFlags() & 0x1) != 0)
                    {
                        // this is the case for the instancebaseline for console recorded replays
                        isCompressed = stream.readBitFlag();
                    }
                    bitLength = stream.readUBitInt(17) * 8;
                }

                int byteLength = (bitLength + 7) / 8;
                byte[] valueBuf;
                if (isCompressed)
                {
                    stream.readBitsIntoByteArray(tempBuf, bitLength);

                    var decompressor = new SnappyDecompressor();
                   valueBuf = decompressor.Decompress(tempBuf, 0, byteLength);
                }
                else
                {
                    valueBuf = new byte[byteLength];
                    stream.readBitsIntoByteArray(valueBuf, bitLength);
                }

                data = ByteString.CopyFrom(valueBuf);
            }

            int entryCount = table.getEntryCount();
            if (index < entryCount)
            {
                // update old entry
                table.setValueForIndex(index, data);
                name = table.getNameByIndex(index);
            }
            else if (index == entryCount)
            {
                // add a new entry
                table.addEntry(name, data);
            }
            else
            {
                throw new Exception("index > entryCount");
            }

            keyHistory[i & KEY_HISTORY_MASK] = name;

            raise(table, index, name, data);
        }
    }
}