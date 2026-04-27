namespace Divine.Plugin.Engine.Processor.SendTables;

using Divine.Plugin.Engine.IO.Source2;

internal sealed class ProtoDecoderProperties : IDecoderProperties
{
    private int? encodeFlags;
    private int? bitCount;
    public float? lowValue;
    public float? highValue;
    public string? encoderType;

    public ProtoDecoderProperties(int? encodeFlags, int? bitCount, float? lowValue, float? highValue, string? encoderType)
    {
        this.encodeFlags = encodeFlags;
        this.bitCount = bitCount;
        this.lowValue = lowValue;
        this.highValue = highValue;
        this.encoderType = encoderType;
    }

    public int? EncodeFlags { get { return encodeFlags; } }

    public int? BitCount { get { return bitCount; } }

    public float? LowValue { get { return lowValue; } }

    public float? HighValue { get { return highValue; } }

    public string? EncoderType { get { return encoderType; } }

    public int? getEncodeFlags()
    {
        return encodeFlags;
    }

    public int? getBitCount()
    {
        return bitCount;
    }

    public float? getLowValue()
    {
        return lowValue;
    }

    public float? getHighValue()
    {
        return highValue;
    }

    public string? getEncoderType()
    {
        return encoderType;
    }

    public int getEncodeFlagsOrDefault(int defaultValue)
    {
        return encodeFlags ?? defaultValue;
    }

    public int getBitCountOrDefault(int defaultValue)
    {
        return bitCount ?? defaultValue;
    }

    public float getLowValueOrDefault(float defaultValue)
    {
        return lowValue ?? defaultValue;
    }

    public float getHighValueOrDefault(float defaultValue)
    {
        return highValue ?? defaultValue;
    }
}