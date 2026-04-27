namespace Divine.Plugin.Engine.Model.State;

internal interface IArrayEntityState
{
    int length();

    bool has(int idx);

    object? get(int idx);

    void set(int idx, object value);

    void clear(int idx);

    bool isSub(int idx);

    IArrayEntityState sub(int idx);

    IArrayEntityState capacity(int wantedSize, bool shrinkIfNeeded);

    IArrayEntityState capacity(int wantedSize)
    {
        return capacity(wantedSize, false);
    }
}