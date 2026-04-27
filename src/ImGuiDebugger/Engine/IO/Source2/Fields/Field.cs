namespace Divine.Plugin.Engine.IO.Source2.Fields;

using System;

using Divine.Plugin.Engine.IO.Decoder;
using Divine.Plugin.Engine.Model.State;

internal abstract class Field
{
    private FieldType fieldType;

    public Field(FieldType fieldType)
    {
        this.fieldType = fieldType;
    }

    public FieldType getType()
    {
        return fieldType;
    }

    public virtual IDecoderProperties? getDecoderProperties()
    {
        return null;
    }

    public virtual IDecoder? getDecoder()
    {
        return null;
    }

    public virtual Field? getChild(int idx)
    {
        return null;
    }

    public virtual int? getChildIndex(String nameSegment)
    {
        return null;
    }

    public virtual string? getChildNameSegment(int idx)
    {
        return null;
    }

    public virtual object? getArrayEntityState(IArrayEntityState state, int idx)
    {
        return null;
    }

    public virtual void setArrayEntityState(IArrayEntityState state, int idx, object value)
    {
        throw new NotSupportedException(GetType().Name);
    }

    public virtual void ensureArrayEntityStateCapacity(IArrayEntityState state, int capacity)
    {
        throw new NotSupportedException(GetType().Name);
    }

    public virtual bool isHiddenFieldPath()
    {
        // true, if this field's state value is hidden
        return false;
    }

    public string toString()
    {
        return getType().ToString();
    }

}