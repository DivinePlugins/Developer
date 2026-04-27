namespace Divine.Plugin.Engine.IO.Decoder;

internal interface IDecoder
{
    object decode(BitStream bs);
}

internal interface IDecoder<T> : IDecoder
{
    object? IDecoder.decode(BitStream bs)
    {
        return decode(bs);
    }

    new T? decode(BitStream bs);
}