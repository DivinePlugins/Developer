namespace Divine.Plugin.Engine.IO.Decoder.Factory;

using Divine.Numerics;
using Divine.Plugin.Engine.IO.Source2;

internal sealed class VectorDecoderFactory : IDecoderFactory<Vector>
{
    private readonly int dim;

    public VectorDecoderFactory(int dim)
    {
        this.dim = dim;
    }

    public static IDecoder<Vector> createDecoderStatic(int dim, IDecoderProperties f)
    {
        if (dim == 3 && "normal".Equals(f.getEncoderType()))
        {
            return new VectorNormalDecoder();
        }

        return new VectorDefaultDecoder(dim, FloatDecoderFactory.createDecoderStatic(f));
    }

    public IDecoder<Vector> createDecoder(IDecoderProperties f)
    {
        return createDecoderStatic(dim, f);
    }
}