namespace Divine.Plugin.Engine.IO.Decoder;

internal sealed class FloatCoordDecoder : IDecoder<float>
{
    public float decode(BitStream bs)
    {
        return bs.readBitCoord();
    }
}