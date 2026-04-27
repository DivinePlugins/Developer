namespace Divine.Plugin.Engine.Model.Source2;

internal interface IModifiableFieldPath : IFieldPath
{
    public static IModifiableFieldPath newInstance()
    {
        return new LongModifiableFieldPath();
    }

    void set(int i, int v);

    void down();

    void up(int n);

    IFieldPath unmodifiable();

    void inc(int i, int n)
    {
        set(i, get(i) + n);
    }

    void inc(int n)
    {
        inc(last(), n);
    }

    void cur(int v)
    {
        set(last(), v);
    }

    int cur()
    {
        return get(last());
    }
}

internal interface IModifiableFieldPath<T> : IModifiableFieldPath, IFieldPath<T>
    where T : IFieldPath
{
}