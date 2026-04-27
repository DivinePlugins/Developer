namespace Divine.Plugin.Engine.IO.Decoder.Factory;

using Divine.Numerics;
using Divine.Plugin.Engine.IO.Source2;

internal sealed class QAngleDecoderFactory : IDecoderFactory<Vector3>
{
    public static IDecoder<Vector3> createDecoderStatic(IDecoderProperties f)
    {
        int bc = f.getBitCountOrDefault(0);
        if ("qangle_pitch_yaw".Equals(f.getEncoderType()))
        {
            return new QAnglePitchYawOnlyDecoder(bc);
        }

        if (bc == 0)
        {
            return new QAngleNoBitCountDecoder();
        }

        if (bc == 32)
        {
            return new QAngleNoScaleDecoder();
        }

        return new QAngleBitCountDecoder(bc);
    }

    public IDecoder<Vector3> createDecoder(IDecoderProperties f)
    {
        return createDecoderStatic(f);
    }
}