namespace Divine.Plugin.Engine.IO.Decoder;

using Divine.Numerics;

internal sealed class QAngleBitCountDecoder : IDecoder<Vector3>
{
    private readonly int nBits;

    public QAngleBitCountDecoder(int nBits)
    {
        this.nBits = nBits;
    }

    public Vector3 decode(BitStream bs)
    {
        float[] v = new float[3];
        v[0] = bs.readBitAngle(nBits);
        v[1] = bs.readBitAngle(nBits);
        v[2] = bs.readBitAngle(nBits);
        return new(v);
    }
}