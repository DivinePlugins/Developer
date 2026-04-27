namespace Divine.Plugin.Engine.IO.Source2.Fields;

using Divine.Plugin.Engine.IO.Decoder;
using Divine.Plugin.Engine.Model.State;

internal class PointerField : SerializerField
{
    private static readonly DecoderHolder decoderHolder = DecoderFactory.createDecoder("bool");

    public PointerField(FieldType fieldType, Serializer serializer)
        : base(fieldType, serializer)
    {
    }

    public override IDecoderProperties getDecoderProperties()
    {
        return decoderHolder.getDecoderProperties();
    }

    public override IDecoder getDecoder()
    {
        return decoderHolder.getDecoder();
    }

    public override bool isHiddenFieldPath()
    {
        return true;
    }

    public override void setArrayEntityState(IArrayEntityState state, int idx, object value)
    {
        var existing = (bool)value;
        if (state.has(idx) ^ existing)
        {
            if (existing)
            {
                state.sub(idx);
            }
            else
            {
                state.clear(idx);
            }
        }
    }

    public override object getArrayEntityState(IArrayEntityState state, int idx)
    {
        return state.isSub(idx);
    }
}