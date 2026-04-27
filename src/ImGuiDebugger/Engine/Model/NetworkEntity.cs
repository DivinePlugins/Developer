namespace Divine.Plugin.Engine.Model;

using System;

using Divine.Plugin.Engine.IO.Source2;
using Divine.Plugin.Engine.Model.Source2;
using Divine.Plugin.Engine.Model.State;

internal sealed class NetworkEntity
{
    private readonly int index;
    private readonly int serial;
    private readonly int handle;
    private readonly DTClass dtClass;

    private bool existent;
    private bool active;
    private IEntityState state = null!;

    public NetworkEntity(int index, int serial, int handle, DTClass dtClass)
    {
        this.index = index;
        this.serial = serial;
        this.handle = handle;
        this.dtClass = dtClass;
    }

    public int getIndex()
    {
        return index;
    }

    public int getSerial()
    {
        return serial;
    }

    public int getHandle()
    {
        return handle;
    }

    public DTClass getDtClass()
    {
        return dtClass;
    }

    public bool isExistent()
    {
        return existent;
    }

    public void setExistent(bool existent)
    {
        this.existent = existent;
    }

    public bool isActive()
    {
        return active;
    }

    public void setActive(bool active)
    {
        this.active = active;
    }

    public IEntityState getState()
    {
        return state;
    }

    public void setState(IEntityState? state)
    {
        this.state = state;
    }

    public bool hasProperty(string property)
    {
        return getDtClass().getFieldPathForName(property) != null;
    }

    public bool hasProperties(params string[] properties)
    {
        foreach (string property in properties)
        {
            if (!hasProperty(property))
            {
                return false;
            }
        }

        return true;
    }

    public T getProperty<T>(string property)
    {
        IFieldPath? fp = getDtClass().getFieldPathForName(property);
        if (fp == null)
        {
            throw new ArgumentException($"property {property} not found on entity of class {getDtClass().getDtName()}");
        }

        return getPropertyForFieldPath<T>(fp);
    }

    public T getPropertyForFieldPath<T>(IFieldPath fp)
    {
        return getState().getValueForFieldPath<T>(fp);
    }

    public override string ToString()
    {
        string title = "idx: " + getIndex() + ", serial: " + getSerial() + ", class: " + getDtClass().getDtName();
        return getState().dump(title, getDtClass().getNameForFieldPath);
    }

    public long getUid()
    {
        return uid(dtClass.getClassId(), handle);
    }

    public static long uid(int dtClassId, int handle)
    {
        return (long)dtClassId << 32 | handle;
    }
}