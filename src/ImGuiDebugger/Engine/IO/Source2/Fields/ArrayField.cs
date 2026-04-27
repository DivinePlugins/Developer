namespace Divine.Plugin.Engine.IO.Source2.Fields;

using Divine.Plugin.Engine.Model.State;

internal sealed class ArrayField : Field
{
    private readonly Field elementField;
    private readonly int length;

    public ArrayField(FieldType fieldType, Field elementField, int length)
        : base(fieldType)
    {
        this.elementField = elementField;
        this.length = length;
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
        state.capacity(length);
    }

    public override object getArrayEntityState(IArrayEntityState state, int idx)
    {
        return state.sub(idx).length();
    }
}