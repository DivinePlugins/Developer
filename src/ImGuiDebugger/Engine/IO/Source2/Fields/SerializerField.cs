namespace Divine.Plugin.Engine.IO.Source2.Fields;

using Divine.Plugin.Engine.Model.State;

internal class SerializerField : Field
{
    protected Serializer serializer;

    public SerializerField(FieldType fieldType, Serializer serializer)
        : base(fieldType)
    {
        this.serializer = serializer;
    }

    public Serializer getSerializer()
    {
        return serializer;
    }

    public override Field getChild(int idx)
    {
        return serializer.getField(idx);
    }

    public override int? getChildIndex(string nameSegment)
    {
        return serializer.getFieldIndex(nameSegment);
    }

    public override string getChildNameSegment(int idx)
    {
        return serializer.getFieldName(idx);
    }

    public override void ensureArrayEntityStateCapacity(IArrayEntityState state, int capacity)
    {
        state.capacity(serializer.getFieldCount());
    }
}