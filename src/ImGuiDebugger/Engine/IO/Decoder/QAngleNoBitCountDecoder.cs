namespace Divine.Plugin.Engine.IO.Decoder;

using Divine.Numerics;

internal sealed class QAngleNoBitCountDecoder : IDecoder<Vector3>
{
    public Vector3 decode(BitStream bs)
    {
        float[] v = new float[3];
        bool b0 = bs.readBitFlag();
        bool b1 = bs.readBitFlag();
        bool b2 = bs.readBitFlag();
        if (b0)
        {
            v[0] = bs.readBitCoord();
        }

        if (b1)
        {
            v[1] = bs.readBitCoord();
        }

        if (b2)
        {
            v[2] = bs.readBitCoord();
        }

        return new(v);
    }
}