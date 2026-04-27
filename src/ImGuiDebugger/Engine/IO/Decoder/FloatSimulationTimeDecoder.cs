namespace Divine.Plugin.Engine.IO.Decoder;

internal sealed class FloatSimulationTimeDecoder : IDecoder<float>
{
    private const float FRAME_TIME = 1.0f / 30.0f;

    public float decode(BitStream bs)
    {
        return bs.readVarULong() * FRAME_TIME;
    }
}