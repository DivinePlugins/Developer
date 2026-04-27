namespace Divine.Plugin.Engine.IO.Decoder;

internal sealed class LongUnsignedDecoder : IDecoder<long>
{
    private readonly int nBits;

    public LongUnsignedDecoder(int nBits)
    {
        this.nBits = nBits;
    }

    public long decode(BitStream bs)
    {
        return bs.readUBitLong(nBits);
    }
}