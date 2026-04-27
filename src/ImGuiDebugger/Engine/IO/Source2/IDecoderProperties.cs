namespace Divine.Plugin.Engine.IO.Source2;

internal interface IDecoderProperties
{
    public static readonly IDecoderProperties Default = new DefaultDecoderProperties();

    int? getEncodeFlags();

    int? getBitCount();

    float? getLowValue();

    float? getHighValue();

    string? getEncoderType();

    int getEncodeFlagsOrDefault(int defaultValue);

    int getBitCountOrDefault(int defaultValue);

    float getLowValueOrDefault(float defaultValue);

    float getHighValueOrDefault(float defaultValue);

    private sealed class DefaultDecoderProperties : IDecoderProperties
    {
        int? IDecoderProperties.getEncodeFlags()
        {
            return null;
        }

        int? IDecoderProperties.getBitCount()
        {
            return null;
        }

        float? IDecoderProperties.getLowValue()
        {
            return null;
        }

        float? IDecoderProperties.getHighValue()
        {
            return null;
        }

        string? IDecoderProperties.getEncoderType()
        {
            return null;
        }

        int IDecoderProperties.getEncodeFlagsOrDefault(int defaultValue)
        {
            return defaultValue;
        }

        int IDecoderProperties.getBitCountOrDefault(int defaultValue)
        {
            return defaultValue;
        }

        float IDecoderProperties.getLowValueOrDefault(float defaultValue)
        {
            return defaultValue;
        }

        float IDecoderProperties.getHighValueOrDefault(float defaultValue)
        {
            return defaultValue;
        }
    }
}