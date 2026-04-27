namespace Divine.Plugin.Engine.Model;

using System;
using System.Collections.Generic;
using System.Linq;

using ConsoleTableExt;

using Google.Protobuf;

internal sealed class StringTable
{
    private readonly string name;
    private readonly int? maxEntries;
    private readonly bool userDataFixedSize;
    private readonly int userDataSize;
    private readonly int userDataSizeBits;
    private readonly int flags;

    private List<Entry> entries;
    private List<Entry> initialEntries;

    public StringTable(string name, int? maxEntries, bool userDataFixedSize, int userDataSize, int userDataSizeBits, int flags)
    {
        this.name = name;
        this.maxEntries = maxEntries;
        this.userDataFixedSize = userDataFixedSize;
        this.userDataSize = userDataSize;
        this.userDataSizeBits = userDataSizeBits;
        this.flags = flags;
        this.entries = new();
        this.initialEntries = new();
    }

    public void setValueForIndex(int index, ByteString value)
    {
        entries[index].value = value;
    }

    public void addEntry(String name, ByteString value)
    {
        entries.Add(new Entry(name, value));
    }

    public bool hasIndex(int index)
    {
        return index >= 0 && index < entries.Count;
    }

    public ByteString getValueByIndex(int index)
    {
        return entries[index].value;
    }

    public string getNameByIndex(int index)
    {
        return entries[index].name;
    }

    public void markInitialState()
    {
        initialEntries = entries.Select(e => new Entry(e.name, e.value)).ToList();
    }

    public void reset()
    {
        entries.Clear();
        initialEntries.Select(e => new Entry(e.name, e.value)).ToList().ForEach(entries.Add);
    }

    public int? getMaxEntries()
    {
        return maxEntries;
    }

    public bool getUserDataFixedSize()
    {
        return userDataFixedSize;
    }

    public int getUserDataSize()
    {
        return userDataSize;
    }

    public int getUserDataSizeBits()
    {
        return userDataSizeBits;
    }

    public string getName()
    {
        return name;
    }

    public int getFlags()
    {
        return flags;
    }

    public int getEntryCount()
    {
        return entries.Count;
    }

    public override string ToString()
    {
        var t = ConsoleTableBuilder.From(new ConsoleTableBaseData()
        {
            Column = new() { "Index", "Key", "Value" }
        })
        .WithTitle(getName())
        .WithTextAlignment(new()
        {
            { 0, TextAligntment.Right },
            { 1, TextAligntment.Right },
            { 2, TextAligntment.Right }
        });

        int n = entries.Count;
        for (int i = 0; i < n; i++)
        {
            ByteString v = getValueByIndex(i);

            t.AddRow(i, getNameByIndex(i), v != null ? (v.Length + " bytes") : "-");
        }

        return t.Export().ToString();
    }

    private sealed class Entry
    {
        public string name;

        public ByteString value;

        public Entry(string name, ByteString value)
        {
            this.name = name;
            this.value = value;
        }
    }
}