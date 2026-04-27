namespace Divine.Plugin.Engine.IO.Decoder;

internal sealed class IntMinusOneDecoder : IDecoder<int>
{
    public int decode(BitStream bs)
    {
        return bs.readVarUInt() - 1;
    }
}