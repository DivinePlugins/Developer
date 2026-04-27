namespace Divine.Plugin.Engine.IO.Source2;

using Divine.Plugin.Engine.IO.Decoder;

internal class DecoderHolder
{
    private readonly IDecoderProperties decoderProperties;
    private readonly IDecoder decoder;

    public DecoderHolder(IDecoderProperties decoderProperties, IDecoder decoder)
    {
        this.decoderProperties = decoderProperties;
        this.decoder = decoder;
    }

    public IDecoderProperties getDecoderProperties()
    {
        return decoderProperties;
    }

    public IDecoder getDecoder()
    {
        return decoder;
    }
}