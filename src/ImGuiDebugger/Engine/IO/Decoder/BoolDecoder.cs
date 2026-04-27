namespace Divine.Plugin.Engine.IO.Decoder;

internal sealed class BoolDecoder : IDecoder<bool>
{
    public bool decode(BitStream bs)
    {
        return bs.readBitFlag();
    }
}