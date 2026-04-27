namespace Divine.Plugin.Engine.IO.Decoder;

using System;

internal sealed class FloatNoScaleDecoder : IDecoder<float>
{
    public float decode(BitStream bs)
    {
        return BitConverter.Int32BitsToSingle(bs.readUBitInt(32));
    }
}