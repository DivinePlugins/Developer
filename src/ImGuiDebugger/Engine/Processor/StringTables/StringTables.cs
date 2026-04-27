namespace Divine.Plugin.Engine.Processor.StringTables;

using System;
using System.Collections.Generic;

using Divine.Plugin.Engine.Model;

internal sealed class StringTables
{
    private readonly Dictionary<int, StringTable> byId = new();
    private readonly Dictionary<string, StringTable> byName = new();

    //[OnStringTableCreated]
    public void onStringTableCreated(int tableNum, StringTable table)
    {
        if (byId.ContainsKey(tableNum) || byName.ContainsKey(table.getName()))
        {
            throw new Exception($"String table {tableNum} ({table.getName()}) already exists!");
        }

        byId[tableNum] = table;
        byName[table.getName()] = table;
    }

    //[OnStringTableClear]
    public void clearAllStringTables()
    {
        byId.Clear();
        byName.Clear();
    }

    public StringTable? forName(string name)
    {
        if (!byName.TryGetValue(name, out var stringTable))
        {
            return null;
        }

        return stringTable;
    }

    public StringTable? forId(int id)
    {
        if (!byId.TryGetValue(id, out var stringTable))
        {
            return null;
        }

        return stringTable;
    }
}