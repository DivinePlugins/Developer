namespace Divine.Plugin.Engine.IO.Source2.Fields;

using Divine.Plugin.Engine.IO.Decoder;
using Divine.Plugin.Engine.Model.State;

internal sealed class ValueField : Field
{
    private readonly DecoderHolder decoderHolder;

    public ValueField(FieldType fieldType, DecoderHolder decoderHolder)
        : base(fieldType)
    {
        this.decoderHolder = decoderHolder;
    }

    public override IDecoderProperties getDecoderProperties()
    {
        return decoderHolder.getDecoderProperties();
    }

    public override IDecoder getDecoder()
    {
        return decoderHolder.getDecoder();
    }

    public override void setArrayEntityState(IArrayEntityState state, int idx, object value)
    {
        state.set(idx, value);
    }

    public override object getArrayEntityState(IArrayEntityState state, int idx)
    {
        return state.get(idx);
    }
}