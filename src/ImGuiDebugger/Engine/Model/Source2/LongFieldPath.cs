namespace Divine.Plugin.Engine.Model.Source2;

internal sealed class LongFieldPath : IFieldPath<LongFieldPath>
{
    private long id;

    public LongFieldPath(long id)
    {
        this.id = id;
    }

    public int get(int i)
    {
        return LongFieldPathFormat.get(id, i);
    }

    public int last()
    {
        return LongFieldPathFormat.last(id);
    }

    public override string ToString()
    {
        return ((IFieldPath)this).asString();
    }

    public override bool Equals(object? obj)
    {
        if (obj is LongFieldPath)
        {
            return id == ((LongFieldPath)obj).id;
        }

        return false;
    }

    public override int GetHashCode()
    {
        return LongFieldPathFormat.hashCode(id);
    }

    public int CompareTo(LongFieldPath? other)
    {
        return LongFieldPathFormat.compareTo(id, other.id);
    }
}