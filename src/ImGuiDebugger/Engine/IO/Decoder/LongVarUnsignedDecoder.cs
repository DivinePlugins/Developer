namespace Divine.Plugin.Engine.IO.Decoder;

internal sealed class LongVarUnsignedDecoder : IDecoder<long>
{
    public long decode(BitStream bs)
    {
        return bs.readVarULong();
    }
}