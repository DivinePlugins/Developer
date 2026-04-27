namespace Divine.Plugin.Engine.Model;

internal sealed class BuildNumberRange
{
    private readonly int? start;
    private readonly int? end;

    public BuildNumberRange(int? start, int? end)
    {
        this.start = start;
        this.end = end;
    }

    public bool appliesTo(int buildNumber)
    {
        if (start == null && end == null)
        {
            return true;
        }

        return buildNumber != -1 && (start == null || start <= buildNumber) && (end == null || end >= buildNumber);
    }
}