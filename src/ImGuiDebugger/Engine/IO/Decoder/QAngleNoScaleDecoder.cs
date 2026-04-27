namespace Divine.Plugin.Engine.IO.Decoder;

using System;

using Divine.Numerics;

internal sealed class QAngleNoScaleDecoder : IDecoder<Vector3>
{
    public Vector3 decode(BitStream bs)
    {
        float[] v = new float[3];
        v[0] = BitConverter.Int32BitsToSingle(bs.readUBitInt(32));
        v[1] = BitConverter.Int32BitsToSingle(bs.readUBitInt(32));
        v[2] = BitConverter.Int32BitsToSingle(bs.readUBitInt(32));
        return new(v);
    }
}