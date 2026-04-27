namespace Divine.Plugin.Engine.IO.Decoder;

using Divine.Plugin.Engine.IO.Source2;

internal interface IDecoderFactory
{
    IDecoder createDecoder(IDecoderProperties f);
}

internal interface IDecoderFactory<T> : IDecoderFactory
{
    IDecoder IDecoderFactory.createDecoder(IDecoderProperties f)
    {
        return createDecoder(f);
    }

    new IDecoder<T> createDecoder(IDecoderProperties f);
}