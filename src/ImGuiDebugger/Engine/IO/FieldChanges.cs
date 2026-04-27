namespace Divine.Plugin.Engine.IO;

using System;

using Divine.Plugin.Engine.Model.Source2;
using Divine.Plugin.Engine.Model.State;

internal sealed class FieldChanges
{

    private IFieldPath[] fieldPaths;
    private object[] values;

    public FieldChanges(IFieldPath[] source, int n)
    {
        fieldPaths = new IFieldPath[n];
        Array.Copy(source, 0, fieldPaths, 0, n);
        values = new object[n];
    }

    public bool applyTo(IEntityState state)
    {
        bool capacityChanged = false;
        for (int i = 0; i < fieldPaths.Length; i++)
        {
            capacityChanged |= state.setValueForFieldPath(fieldPaths[i], values[i]);
        }
        return capacityChanged;
    }

    public object getValue(int idx)
    {
        return values[idx];
    }

    public void setValue(int idx, object value)
    {
        values[idx] = value;
    }

    public IFieldPath[] getFieldPaths()
    {
        return fieldPaths;
    }
}