namespace Divine.Plugin.Engine.IO.Decoder;

using Divine.Numerics;

internal sealed class VectorNormalDecoder : IDecoder<Vector>
{
    public Vector decode(BitStream bs)
    {
        return new(bs.read3BitNormal());
    }
}