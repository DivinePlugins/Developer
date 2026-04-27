namespace Divine.Plugin.Engine.IO.Decoder;

internal sealed class StringZeroTerminatedDecoder : IDecoder<string>
{
    public string decode(BitStream bs)
    {
        return bs.readString(int.MaxValue);
    }
}