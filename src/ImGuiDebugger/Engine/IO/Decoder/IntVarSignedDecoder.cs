namespace Divine.Plugin.Engine.IO.Decoder;

internal sealed class IntVarSignedDecoder : IDecoder<int>
{
    public int decode(BitStream bs)
    {
        return bs.readVarSInt();
    }
}