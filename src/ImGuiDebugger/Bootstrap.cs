namespace ImGuiDebugger;

using Divine.Service;

internal sealed class Bootstrap : Bootstrapper
{
    private Context Context = null!;

    protected override void OnMainActivate()
    {
        Context = new Context();
    }

    protected override void OnMainDeactivate()
    {
        Context.Dispose();
    }
}