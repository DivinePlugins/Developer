namespace Divine.Plugin.Engine.Model.Source2;

internal sealed class LongModifiableFieldPath : IModifiableFieldPath<LongModifiableFieldPath>
{
    private long id;

    public void set(int i, int v)
    {
        id = LongFieldPathFormat.set(id, i, v);
    }

    public int get(int i)
    {
        return LongFieldPathFormat.get(id, i);
    }

    public void down()
    {
        id = LongFieldPathFormat.down(id);
    }

    public void up(int n)
    {
        id = LongFieldPathFormat.up(id, n);
    }

    public int last()
    {
        return LongFieldPathFormat.last(id);
    }

    public IFieldPath unmodifiable()
    {
        return new LongFieldPath(id);
    }

    public override string ToString()
    {
        return ((IFieldPath)this).asString();
    }
    public override bool Equals(object? obj)
    {
        if (obj is LongModifiableFieldPath)
        {
            return id == ((LongModifiableFieldPath)obj).id;
        }

        return false;
    }

    public override int GetHashCode()
    {
        return LongFieldPathFormat.hashCode(id);
    }

    public int CompareTo(LongModifiableFieldPath? other)
    {
        return LongFieldPathFormat.compareTo(id, other.id);
    }
}