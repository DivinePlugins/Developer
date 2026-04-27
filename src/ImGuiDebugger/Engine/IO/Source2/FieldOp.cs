namespace Divine.Plugin.Engine.IO.Source2;

using Divine.Plugin.Engine.IO;
using Divine.Plugin.Engine.Model.Source2;

internal sealed class FieldOp // TODO Rework
{
    public FieldOpType Type;

    public int Ordinal;

    public ExecuteFunction Execute = null!;

    public static readonly FieldOp[] FieldsTable = new[]
    {
        new FieldOp
        {
            Type = FieldOpType.PlusOne,
            Execute = (fp, bs) => { fp.inc(1); }
        },
        new FieldOp
        {
            Type = FieldOpType.PlusTwo,
            Execute = (fp, bs) => { fp.inc(2); }
        },
        new FieldOp
        {
            Type = FieldOpType.PlusThree,
            Execute = (fp, bs) => { fp.inc(3); }
        },
        new FieldOp
        {
            Type = FieldOpType.PlusFour,
            Execute = (fp, bs) => { fp.inc(4); }
        },
        new FieldOp
        {
            Type = FieldOpType.PlusN,
            Execute = (fp, bs) => { fp.inc(bs.readUBitVarFieldPath() + 5); }
        },
        new FieldOp
        {
            Type = FieldOpType.PushOneLeftDeltaZeroRightZero,
            Execute = (fp, bs) => { fp.down(); }
        },
        new FieldOp
        {
            Type = FieldOpType.PushOneLeftDeltaZeroRightNonZero,
            Execute = (fp, bs) =>
            {
                fp.down();
                fp.inc(bs.readUBitVarFieldPath());
            }
        },
        new FieldOp
        {
            Type = FieldOpType.PushOneLeftDeltaOneRightZero,
            Execute = (fp, bs) =>
            {
                fp.inc(1);
                fp.down();
            }
        },
        new FieldOp
        {
            Type = FieldOpType.PushOneLeftDeltaOneRightNonZero,
            Execute = (fp, bs) =>
            {
                fp.inc(1);
                fp.down();
                fp.inc(bs.readUBitVarFieldPath());
            }
        },
        new FieldOp
        {
            Type = FieldOpType.PushOneLeftDeltaNRightZero,
            Execute = (fp, bs) =>
            {
                fp.inc(bs.readUBitVarFieldPath());
                fp.down();
            }
        },
        new FieldOp
        {
            Type = FieldOpType.PushOneLeftDeltaNRightNonZero,
            Execute = (fp, bs) =>
            {
                fp.inc(bs.readUBitVarFieldPath() + 2);
                fp.down();
                fp.inc(bs.readUBitVarFieldPath() + 1);
            }
        },
        new FieldOp
        {
            Type = FieldOpType.PushOneLeftDeltaNRightNonZeroPack6Bits,
            Execute = (fp, bs) =>
            {
                fp.inc(bs.readUBitInt(3) + 2);
                fp.down();
                fp.inc(bs.readUBitInt(3) + 1);
            }
        },
        new FieldOp
        {
            Type = FieldOpType.PushOneLeftDeltaNRightNonZeroPack8Bits,
            Execute = (fp, bs) =>
            {
                fp.inc(bs.readUBitInt(4) + 2);
                fp.down();
                fp.inc(bs.readUBitInt(4) + 1);
            }
        },
        new FieldOp
        {
            Type = FieldOpType.PushTwoLeftDeltaZero,
            Execute = (fp, bs) =>
            {
                fp.down();
                fp.inc(bs.readUBitVarFieldPath());
                fp.down();
                fp.inc(bs.readUBitVarFieldPath());
            }
        },
        new FieldOp
        {
            Type = FieldOpType.PushTwoPack5LeftDeltaZero,
            Execute = (fp, bs) =>
            {
                fp.down();
                fp.inc(bs.readUBitInt(5));
                fp.down();
                fp.inc(bs.readUBitInt(5));
            }
        },
        new FieldOp
        {
            Type = FieldOpType.PushThreeLeftDeltaZero,
            Execute = (fp, bs) =>
            {
                fp.down();
                fp.inc(bs.readUBitVarFieldPath());
                fp.down();
                fp.inc(bs.readUBitVarFieldPath());
                fp.down();
                fp.inc(bs.readUBitVarFieldPath());
            }
        },
        new FieldOp
        {
            Type = FieldOpType.PushThreePack5LeftDeltaZero,
            Execute = (fp, bs) =>
            {
                fp.down();
                fp.inc(bs.readUBitInt(5));
                fp.down();
                fp.inc(bs.readUBitInt(5));
                fp.down();
                fp.inc(bs.readUBitInt(5));
            }
        },
        new FieldOp
        {
            Type = FieldOpType.PushTwoLeftDeltaOne,
            Execute = (fp, bs) =>
            {
                fp.inc(1);
                fp.down();
                fp.inc(bs.readUBitVarFieldPath());
                fp.down();
                fp.inc(bs.readUBitVarFieldPath());
            }
        },
        new FieldOp
        {
            Type = FieldOpType.PushTwoPack5LeftDeltaOne,
            Execute = (fp, bs) =>
            {
                fp.inc(1);
                fp.down();
                fp.inc(bs.readUBitInt(5));
                fp.down();
                fp.inc(bs.readUBitInt(5));
            }
        },
        new FieldOp
        {
            Type = FieldOpType.PushThreeLeftDeltaOne,
            Execute = (fp, bs) =>
            {
                fp.inc(1);
                fp.down();
                fp.inc(bs.readUBitVarFieldPath());
                fp.down();
                fp.inc(bs.readUBitVarFieldPath());
                fp.down();
                fp.inc(bs.readUBitVarFieldPath());
            }
        },
        new FieldOp
        {
            Type = FieldOpType.PushThreePack5LeftDeltaOne,
            Execute = (fp, bs) =>
            {
                fp.inc(1);
                fp.down();
                fp.inc(bs.readUBitInt(5));
                fp.down();
                fp.inc(bs.readUBitInt(5));
                fp.down();
                fp.inc(bs.readUBitInt(5));
            }
        },
        new FieldOp
        {
            Type = FieldOpType.PushTwoLeftDeltaN,
            Execute = (fp, bs) =>
            {
                fp.inc(bs.readUBitVar() + 2);
                fp.down();
                fp.inc(bs.readUBitVarFieldPath());
                fp.down();
                fp.inc(bs.readUBitVarFieldPath());
            }
        },
        new FieldOp
        {
            Type = FieldOpType.PushTwoPack5LeftDeltaN,
            Execute = (fp, bs) =>
            {
                fp.inc(bs.readUBitVar() + 2);
                fp.down();
                fp.inc(bs.readUBitInt(5));
                fp.down();
                fp.inc(bs.readUBitInt(5));
            }
        },
        new FieldOp
        {
            Type = FieldOpType.PushThreeLeftDeltaN,
            Execute = (fp, bs) =>
            {
                fp.inc(bs.readUBitVar() + 2);
                fp.down();
                fp.inc(bs.readUBitVarFieldPath());
                fp.down();
                fp.inc(bs.readUBitVarFieldPath());
                fp.down();
                fp.inc(bs.readUBitVarFieldPath());
            }
        },
        new FieldOp
        {
            Type = FieldOpType.PushThreePack5LeftDeltaN,
            Execute = (fp, bs) =>
            {
                fp.inc(bs.readUBitVar() + 2);
                fp.down();
                fp.inc(bs.readUBitInt(5));
                fp.down();
                fp.inc(bs.readUBitInt(5));
                fp.down();
                fp.inc(bs.readUBitInt(5));
            }
        },
        new FieldOp
        {
            Type = FieldOpType.PushN,
            Execute = (fp, bs) =>
            {
                int n = bs.readUBitVar();
                fp.inc(bs.readUBitVar());
                for (int i = 0; i < n; i++)
                {
                    fp.down();
                    fp.inc(bs.readUBitVarFieldPath());
                }
            }
        },
        new FieldOp
        {
            Type = FieldOpType.PushNAndNonTopographical,
            Execute = (fp, bs) =>
            {
                for (int i = 0; i <= fp.last(); i++)
                {
                    if (bs.readBitFlag())
                    {
                        fp.inc(i, bs.readVarSInt() + 1);
                    }
                }

                int c = bs.readUBitVar();
                for (int i = 0; i < c; i++)
                {
                    fp.down();
                    fp.inc(bs.readUBitVarFieldPath());
                }
            }
        },
        new FieldOp
        {
            Type = FieldOpType.PopOnePlusOne,
            Execute = (fp, bs) =>
            {
                fp.up(1);
                fp.inc(1);
            }
        },
        new FieldOp
        {
            Type = FieldOpType.PopOnePlusN,
            Execute = (fp, bs) =>
            {
                fp.up(1);
                fp.inc(bs.readUBitVarFieldPath() + 1);
            }
        },
        new FieldOp
        {
            Type = FieldOpType.PopAllButOnePlusOne,
            Execute = (fp, bs) =>
            {
                fp.up(fp.last());
                fp.inc(1);
            }
        },
        new FieldOp
        {
            Type = FieldOpType.PopAllButOnePlusN,
            Execute = (fp, bs) =>
            {
                fp.up(fp.last());
                fp.inc(bs.readUBitVarFieldPath() + 1);
            }
        },
        new FieldOp
        {
            Type = FieldOpType.PopAllButOnePlusNPack3Bits,
            Execute = (fp, bs) =>
            {
                fp.up(fp.last());
                fp.inc(bs.readUBitInt(3) + 1);
            }
        },
        new FieldOp
        {
            Type = FieldOpType.PopAllButOnePlusNPack6Bits,
            Execute = (fp, bs) =>
            {
                fp.up(fp.last());
                fp.inc(bs.readUBitInt(6) + 1);
            }
        },
        new FieldOp
        {
            Type = FieldOpType.PopNPlusOne,
            Execute = (fp, bs) =>
            {
                fp.up(bs.readUBitVarFieldPath());
                fp.inc(1);
            }
        },
        new FieldOp
        {
            Type = FieldOpType.PopNPlusN,
            Execute = (fp, bs) =>
            {
                fp.up(bs.readUBitVarFieldPath());
                fp.inc(bs.readVarSInt());
            }
        },
        new FieldOp
        {
            Type = FieldOpType.PopNAndNonTopographical,
            Execute = (fp, bs) =>
            {
                fp.up(bs.readUBitVarFieldPath());
                for (int i = 0; i <= fp.last(); i++)
                {
                    if (bs.readBitFlag())
                    {
                        fp.inc(i, bs.readVarSInt());
                    }
                }
            }
        },
        new FieldOp
        {
            Type = FieldOpType.NonTopoComplex,
            Execute = (fp, bs) =>
            {
                for (int i = 0; i <= fp.last(); i++)
                {
                    if (bs.readBitFlag())
                    {
                        fp.inc(i, bs.readVarSInt());
                    }
                }
            }
        },
        new FieldOp
        {
            Type = FieldOpType.NonTopoPenultimatePluseOne,
            Execute = (fp, bs) => { fp.inc(fp.last() - 1, 1); }
        },
        new FieldOp
        {
            Type = FieldOpType.NonTopoComplexPack4Bits,
            Execute = (fp, bs) =>
            {
                for (int i = 0; i <= fp.last(); i++)
                {
                    if (bs.readBitFlag())
                    {
                        fp.inc(i, bs.readUBitInt(4) - 7);
                    }
                }
            }
        },
        new FieldOp
        {
            Type = FieldOpType.FieldPathEncodeFinish,
            Execute = (fp, bs) => { }
        }
    };

    static FieldOp()
    {
        for (var i = 0; i < FieldsTable.Length; i++)
        {
            FieldsTable[i].Ordinal = i;
        }
    }

    public int Weight
    {
        get
        {
            return (int)Type;
        }
    }

    public delegate void ExecuteFunction(IModifiableFieldPath fp, BitStream bs);
}

internal enum FieldOpType
{
    PlusOne = 36271,
    PlusTwo = 10334,
    PlusThree = 1375,
    PlusFour = 646,
    PlusN = 4128,
    PushOneLeftDeltaZeroRightZero = 35,
    PushOneLeftDeltaZeroRightNonZero = 3,
    PushOneLeftDeltaOneRightZero = 521,
    PushOneLeftDeltaOneRightNonZero = 2942,
    PushOneLeftDeltaNRightZero = 560,
    PushOneLeftDeltaNRightNonZero = 471,
    PushOneLeftDeltaNRightNonZeroPack6Bits = 10530,
    PushOneLeftDeltaNRightNonZeroPack8Bits = 251,
    PushTwoLeftDeltaZero = 0,
    PushTwoPack5LeftDeltaZero = 0,
    PushThreeLeftDeltaZero = 0,
    PushThreePack5LeftDeltaZero = 0,
    PushTwoLeftDeltaOne = 0,
    PushTwoPack5LeftDeltaOne = 0,
    PushThreeLeftDeltaOne = 0,
    PushThreePack5LeftDeltaOne = 0,
    PushTwoLeftDeltaN = 0,
    PushTwoPack5LeftDeltaN = 0,
    PushThreeLeftDeltaN = 0,
    PushThreePack5LeftDeltaN = 0,
    PushN = 0,
    PushNAndNonTopographical = 310,
    PopOnePlusOne = 2,
    PopOnePlusN = 0,
    PopAllButOnePlusOne = 1837,
    PopAllButOnePlusN = 149,
    PopAllButOnePlusNPack3Bits = 300,
    PopAllButOnePlusNPack6Bits = 634,
    PopNPlusOne = 0,
    PopNPlusN = 0,
    PopNAndNonTopographical = 1,
    NonTopoComplex = 76,
    NonTopoPenultimatePluseOne = 271,
    NonTopoComplexPack4Bits = 99,
    FieldPathEncodeFinish = 25474
}