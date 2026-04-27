namespace Divine.Plugin.Engine.IO.Decoder;

internal sealed class IntVarUnsignedDecoder : IDecoder<int>
{
    public int decode(BitStream bs)
    {
        return bs.readVarUInt();
    }
}