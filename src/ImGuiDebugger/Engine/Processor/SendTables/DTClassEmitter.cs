namespace Divine.Plugin.Engine.Processor.SendTables;
using System.Collections.Generic;

using Divine.Entity.Entities.Components;
using Divine.Plugin.Engine.IO;
using Divine.Plugin.Engine.IO.Source2;
using Divine.Protobufs.Dota2;

internal sealed class DTClassEmitter
{
    private readonly DTClasses DTClasses;

    private readonly FieldGenerator fieldGenerator;

    public DTClassEmitter(DTClasses dtClasses, IEnumerable<ClassInfo> networkClassInfos, CSVCMsg_FlattenedSerializer flattenedSerializer)
    {
        DTClasses = dtClasses;

        fieldGenerator = new FieldGenerator(flattenedSerializer, EngineData.getBuildNumber());
        fieldGenerator.createFields();

        foreach (var networkClassInfo in networkClassInfos)
        {
            DTClass dt = fieldGenerator.createDTClass(networkClassInfo.NetworkName);
            DTClasses.onDTClass(dt);

            dt.setClassId(networkClassInfo.Id);
            DTClasses.byClassId[networkClassInfo.Id] = dt;
        }

        DTClasses.classBits = Util.calcBitsNeededFor(DTClasses.byClassId.Count - 1);
    }
}