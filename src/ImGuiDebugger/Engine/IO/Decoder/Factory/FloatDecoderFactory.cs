namespace Divine.Plugin.Engine.IO.Decoder.Factory;

using Divine.Plugin.Engine.IO.Source2;

internal sealed class FloatDecoderFactory : IDecoderFactory<float>
{
    public IDecoder<float> createDecoder(IDecoderProperties f)
    {
        return createDecoderStatic(f);
    }

    public static IDecoder<float> createDecoderStatic(IDecoderProperties f)
    {
        if ("coord".Equals(f.getEncoderType()))
        {
            return new FloatCoordDecoder();
        }

        if ("simulationtime".Equals(f.getEncoderType()))
        {
            return new FloatSimulationTimeDecoder();
        }

        int bc = f.getBitCountOrDefault(0);
        if (bc <= 0 || bc >= 32)
        {
            return new FloatNoScaleDecoder();
        }

        // TODO: get real name
        return new FloatQuantizedDecoder("N/A", bc, f.getEncodeFlagsOrDefault(0) & 0xF, f.getLowValueOrDefault(0.0f), f.getHighValueOrDefault(1.0f));
    }
}