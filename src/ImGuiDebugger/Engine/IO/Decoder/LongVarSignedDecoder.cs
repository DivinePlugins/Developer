namespace Divine.Plugin.Engine.IO.Decoder;

internal sealed class LongVarSignedDecoder : IDecoder<long>
{
    public long decode(BitStream bs)
    {
        return bs.readVarSLong();
    }
}