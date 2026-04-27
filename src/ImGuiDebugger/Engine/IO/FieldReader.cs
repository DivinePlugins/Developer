namespace Divine.Plugin.Engine.IO;

using System;

using ConsoleTableExt;

using Divine.Plugin.Engine.IO.Decoder;
using Divine.Plugin.Engine.IO.Source2;
using Divine.Plugin.Engine.Model.Source2;

internal sealed class FieldReader
{
    public const int MAX_PROPERTIES = 0x3FFF;

    private IFieldPath[] fieldPaths = new IFieldPath[MAX_PROPERTIES];

    public FieldChanges readFields(BitStream bs, DTClass dtClass, bool debug, bool opDebug)
    {
        ConsoleTableBuilder dataDebugTable = null!;
        ConsoleTableBuilder opDebugTable = null!;

        if (debug)
        {
            dataDebugTable = ConsoleTableBuilder.From(new ConsoleTableBaseData()
            {
                Column = new() { "FP", "Name", "L", "H", "BC", "Flags", "Decoder", "Type", "Value", "#", "read" }
            })
            .WithTitle(dtClass.ToString())
            .WithMinLength(new()
            {
                { 0, 5 },
                { 1, 50 },
                { 3, 3 },
                { 6, 25 },
                { 7, 20 },
                { 8, 20 },
                { 9, 3 },
                { 10, 40 }
            })
            .WithTextAlignment(new()
            {
                { 2, TextAligntment.Right },
                { 3, TextAligntment.Right },
                { 4, TextAligntment.Right },
                { 5, TextAligntment.Right },
                { 9, TextAligntment.Right },
            });
        }

        if (opDebug)
        {
            opDebugTable = ConsoleTableBuilder.From(new ConsoleTableBaseData()
            {
                Column = new() { "OP", "FP", "#", "read" }
            })
            .WithTitle("FieldPath Operations")
            .WithMinLength(new()
            {
                { 0, 5 },
                { 1, 5 },
                { 2, 3 },
                { 3, 40 }
            })
            .WithTextAlignment(new()
            {
                { 2, TextAligntment.Right },
            });
        }

        try
        {
            int n = 0;
            IModifiableFieldPath mfp = IModifiableFieldPath.newInstance();
            while (true)
            {
                int offsBefore = bs.pos();
                FieldOp fieldOp = bs.readFieldOp();
                fieldOp.Execute(mfp, bs);

                if (opDebug)
                {
                    opDebugTable.AddRow(
                        fieldOp.Type.ToString(),
                        mfp.unmodifiable()?.ToString() ?? "-",
                        bs.pos() - offsBefore,
                        bs.toString(offsBefore, bs.pos()));
                }

                if (fieldOp.Type == FieldOpType.FieldPathEncodeFinish)
                {
                    break;
                }
                fieldPaths[n++] = mfp.unmodifiable();
            }

            FieldChanges result = new FieldChanges(fieldPaths, n);

            for (int r = 0; r < n; r++)
            {
                IFieldPath fp = fieldPaths[r];
                IDecoder? decoder = dtClass.getDecoderForFieldPath(fp);
                if (decoder == null)
                {
                    throw new Exception($"no decoder for class {dtClass.getDtName()} at {fp}!");
                }

                int offsBefore = bs.pos();
                result.setValue(r, decoder.decode(bs));

                if (debug)
                {
                    IDecoderProperties props = dtClass.getFieldForFieldPath(fp).getDecoderProperties();
                    FieldType type = dtClass.getTypeForFieldPath(fp);

                    dataDebugTable.AddRow(
                        fp.ToString(),
                        dtClass.getNameForFieldPath(fp) ?? "-",
                        props.getLowValue()?.ToString() ?? "-",
                        props.getHighValue()?.ToString() ?? "-",
                        props.getBitCount()?.ToString() ?? "-",
                        props.getEncodeFlags()?.ToString("X") ?? "-",
                        decoder.GetType().Name,
                        $"{type}{props.getEncoderType() ?? string.Empty}",
                        result.getValue(r)?.ToString() ?? "-",
                        (bs.pos() - offsBefore).ToString(),
                        bs.toString(offsBefore, bs.pos()));
                }
            }

            return result;
        }
        finally
        {
            if (debug)
            {
                dataDebugTable.ExportAndWriteLine();
            }

            if (opDebug)
            {
                opDebugTable.ExportAndWriteLine();
            }
        }
    }

    public int readDeletions(BitStream bs, int indexBits, int[] deletions)
    {
        int n = bs.readUBitVar();
        int c = 0;
        int idx = -1;
        while (c < n)
        {
            idx += bs.readUBitVar();
            deletions[c++] = idx;
        }
        return n;
    }
}