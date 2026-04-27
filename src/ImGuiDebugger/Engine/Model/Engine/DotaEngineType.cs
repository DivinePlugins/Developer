namespace Divine.Plugin.Engine.Model.Engine;

using Divine.Plugin.Engine.IO;

internal sealed class DotaEngineType
{
    private const bool sendTablesContainer = false;

    private const int indexBits = 14;

    private const int serialBits = 17;

    private const int indexMask = (1 << indexBits) - 1;

    private const int _emptyHandle = (1 << (indexBits + 10)) - 1;

    public bool isSendTablesContainer()
    {
        return sendTablesContainer;
    }

    public bool handleDeletions()
    {
        return true;
    }

    public int getIndexBits()
    {
        return indexBits;
    }

    public int getSerialBits()
    {
        return serialBits;
    }

    public int indexForHandle(int handle)
    {
        return handle & indexMask;
    }

    public int serialForHandle(int handle)
    {
        return handle >> indexBits;
    }

    public int handleForIndexAndSerial(int index, int serial)
    {
        return serial << indexBits | index;
    }

    public int emptyHandle()
    {
        return _emptyHandle;
    }

    public float getMillisPerTick()
    {
        return 1000.0f / 30.0f;
    }

    public bool isFullPacketSeekAllowed()
    {
        return true;
    }

    public int getExpectedFullPacketInterval()
    {
        return 1800;
    }

    //public Class<? extends GeneratedMessage> embeddedPacketClassForKind(int kind)
    //{
    //    return EmbeddedPackets.classForKind(kind);
    //}

    //public Class<? extends GeneratedMessage> userMessagePacketClassForKind(int kind)
    //{
    //    throw new UnsupportedOperationException();
    //}

    //public bool isUserMessage(Class<? extends GeneratedMessage> clazz)
    //{
    //    return false;
    //}

    public FieldReader getNewFieldReader()
    {
        return new FieldReader();
    }

    //public void readHeader(Source source)
    //{
    //    source.skipBytes(8);
    //}

    //public void skipHeader(Source source)
    //{
    //    source.skipBytes(8);
    //}

    //public void emitHeader()
    //{
    //}

    //public int readEmbeddedKind(BitStream bs)
    //{
    //    return bs.readUBitVar();
    //}

    //public int determineLastTick(Source source)
    //{
    //    int backup = source.getPosition();
    //    source.setPosition(8);
    //    source.setPosition(source.readFixedInt32());
    //    source.skipVarInt32();
    //    int lastTick = source.readVarInt32();
    //    source.setPosition(backup);
    //    return lastTick;
    //}

    //public <T extends GeneratedMessage> PacketInstance<T> getNextPacketInstance(final Source source) throws IOException {
    //    int rawKind = source.readVarInt32();
    //    final boolean isCompressed = (rawKind & getCompressedFlag()) == getCompressedFlag();
    //    final int kind = rawKind &~ getCompressedFlag();
    //    final int tick = source.readVarInt32();
    //    final int size = source.readVarInt32();
    //    final Class<T> messageClass = (Class<T>) DemoPackets.classForKind(kind);
    //    return new PacketInstance<T>() {

    //        public int getKind() {
    //            return kind;
    //        }

    //        public int getTick() {
    //            return tick;
    //        }

    //        public Class<T> getMessageClass() {
    //            return messageClass;
    //        }

    //        public ResetRelevantKind getResetRelevantKind() {
    //            switch(kind) {
    //                case Demo.EDemoCommands.DEM_SyncTick_VALUE:
    //                    return ResetRelevantKind.SYNC;
    //                case Demo.EDemoCommands.DEM_StringTables_VALUE:
    //                    return ResetRelevantKind.STRINGTABLE;
    //                case Demo.EDemoCommands.DEM_FullPacket_VALUE:
    //                    return ResetRelevantKind.FULL_PACKET;
    //                default:
    //                    return null;
    //            }
    //        }

    //        public T parse() throws IOException {
    //            return Packet.parse(
    //                    messageClass,
    //                    ZeroCopy.wrap(packetReader.readFromSource(source, size, isCompressed))
    //            );
    //        }

    //        public void skip() throws IOException {
    //            source.skipBytes(size);
    //        }
    //    };
    //}
}