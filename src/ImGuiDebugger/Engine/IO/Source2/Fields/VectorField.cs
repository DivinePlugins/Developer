namespace Divine.Plugin.Engine.IO.Source2.Fields;

using Divine.Plugin.Engine.IO.Decoder;
using Divine.Plugin.Engine.Model.State;

internal sealed class VectorField : Field
{
    private static readonly DecoderHolder decoderHolder = DecoderFactory.createDecoder("uint32");

    private readonly Field elementField;

    public VectorField(FieldType fieldType, Field elementField)
        : base(fieldType)
    {
        this.elementField = elementField;
    }

    public override IDecoderProperties getDecoderProperties()
    {
        return decoderHolder.getDecoderProperties();
    }

    public override IDecoder getDecoder()
    {
        return decoderHolder.getDecoder();
    }

    public override Field getChild(int idx)
    {
        return elementField;
    }

    public override int? getChildIndex(string nameSegment)
    {
        return int.Parse(nameSegment);
    }

    public override string getChildNameSegment(int idx)
    {
        return Util.arrayIdxToString(idx);
    }

    public override void ensureArrayEntityStateCapacity(IArrayEntityState state, int capacity)
    {
        state.capacity(capacity, false);
    }

    public override bool isHiddenFieldPath()
    {
        return true;
    }

    public override void setArrayEntityState(IArrayEntityState state, int idx, object value)
    {
        state.sub(idx).capacity((int)value, true);
    }

    public override object getArrayEntityState(IArrayEntityState state, int idx)
    {
        return state.sub(idx).length();
    }
}