namespace Divine.Plugin.Engine.Processor.SendTables;

using System.Collections;
using System.Collections.Generic;

using Divine.Plugin.Engine.IO.Source2;

internal class DTClasses : IEnumerable<DTClass>
{
    public readonly Dictionary<int, DTClass> byClassId = new();
    public readonly Dictionary<string, DTClass> byDtName = new();
    public int classBits;

    public void onDTClass(DTClass dtClass)
    {
        byDtName[dtClass.getDtName()] = dtClass;
    }

    public DTClass? forClassId(int id)
    {
        return byClassId.TryGetValue(id, out var value) ? value : null;
    }

    public DTClass? forDtName(string dtName)
    {
        return byDtName.TryGetValue(dtName, out var value) ? value : null;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public IEnumerator<DTClass> GetEnumerator()
    {
        return byClassId.Values.GetEnumerator();
    }

    public int getClassCount()
    {
        return byClassId.Count;
    }

    public int getClassBits()
    {
        return classBits;
    }
}