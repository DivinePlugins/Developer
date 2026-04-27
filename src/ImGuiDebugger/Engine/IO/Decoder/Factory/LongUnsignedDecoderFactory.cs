namespace Divine.Plugin.Engine.IO.Decoder.Factory;

using Divine.Plugin.Engine.IO.Source2;

internal sealed class LongUnsignedDecoderFactory : IDecoderFactory<long>
{
    public static IDecoder<long> createDecoderStatic(IDecoderProperties f)
    {
        if ("fixed64".Equals(f.getEncoderType()))
        {
            return new LongUnsignedDecoder(64);
        }

        return new LongVarUnsignedDecoder();
    }

    public IDecoder<long> createDecoder(IDecoderProperties f)
    {
        return createDecoderStatic(f);
    }
}