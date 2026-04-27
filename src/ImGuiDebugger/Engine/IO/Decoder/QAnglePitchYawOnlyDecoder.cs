namespace Divine.Plugin.Engine.IO.Decoder;

using System;

using Divine.Numerics;

internal sealed class QAnglePitchYawOnlyDecoder : IDecoder<Vector3>
{
    private readonly int nBits;

    public QAnglePitchYawOnlyDecoder(int nBits)
    {
        this.nBits = nBits;
    }

    public Vector3 decode(BitStream bs)
    {
        float[] v = new float[3];

        if ((nBits | 0x20) == 0x20)
        {
            v[0] = BitConverter.Int32BitsToSingle(bs.readUBitInt(32));
            v[1] = BitConverter.Int32BitsToSingle(bs.readUBitInt(32));
        }
        else
        {
            v[0] = bs.readBitAngle(nBits);
            v[1] = bs.readBitAngle(nBits);
        }

        return new(v);
    }
}