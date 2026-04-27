namespace Divine.Plugin.Engine.IO.Decoder;

using Divine.Numerics;

internal sealed class VectorDefaultDecoder : IDecoder<Vector>
{
    private readonly int dim;
    private readonly IDecoder<float> floatDecoder;

    public VectorDefaultDecoder(int dim, IDecoder<float> floatDecoder)
    {
        this.dim = dim;
        this.floatDecoder = floatDecoder;
    }

    public Vector decode(BitStream bs)
    {
        float[] result = new float[dim];
        for (int i = 0; i < dim; i++)
        {
            result[i] = floatDecoder.decode(bs);
        }

        return new(result);
    }
}