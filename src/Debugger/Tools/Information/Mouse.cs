namespace Debugger.Tools.Information;

using Debugger.Menus;
using Debugger.Metadata;

using Divine.Game;
using Divine.Input;
using Divine.Input.EventArgs;
using Divine.Menu.EventArgs;
using Divine.Menu.Items;
using Divine.Numerics;
using Divine.Renderer;

using Logger;

using TextCopy;

[Priority(77)]
internal sealed class Mouse : IDebuggerTool
{
    private const uint WM_LBUTTONDOWN = 0x0201;

    private readonly MenuSwitcher copyPosition;

    private readonly ILog log;

    private readonly Menu menu;

    private readonly MenuSwitcher showMousePosition;

    public Mouse(IMainMenu mainMenu, ILog log)
    {
        this.log = log;

        this.menu = mainMenu.InformationMenu.AddMenu("Mouse");
        this.showMousePosition = this.menu.AddSwitcher("Show mouse position");
        this.copyPosition = this.menu.AddSwitcher("Copy position on click", false);
    }

    public void Activate()
    {
        this.showMousePosition.ValueChanged += this.ShowMousePositionOnPropertyChanged;
        this.copyPosition.ValueChanged += this.CopyPositionOnPropertyChanged;
    }

    public void Deactivate()
    {
        this.showMousePosition.ValueChanged -= this.ShowMousePositionOnPropertyChanged;
        this.copyPosition.ValueChanged -= this.CopyPositionOnPropertyChanged;
    }

    private void ShowMousePositionOnPropertyChanged(MenuSwitcher switcher, SwitcherChangedEventArgs e)
    {
        if (e.Value)
        {
            RendererManager.Draw += this.DrawingOnDraw;
        }
        else
        {
            RendererManager.Draw -= this.DrawingOnDraw;
        }
    }

    private void CopyPositionOnPropertyChanged(MenuSwitcher switcher, SwitcherChangedEventArgs e)
    {
        if (e.Value)
        {
            InputManager.WindowProc += this.GameOnWndProc;
        }
        else
        {
            InputManager.WindowProc -= this.GameOnWndProc;
        }
    }

    private void DrawingOnDraw()
    {
        var pos = GameManager.MousePosition;

        RendererManager.DrawText(
            pos.ToCopyFormat(),
            GameManager.MouseScreenPosition + new Vector2(35, 0),
            Color.White,
            "Arial",
            20);
    }

    private void GameOnWndProc(WindowProcEventArgs e)
    {
        if (e.Msg == WM_LBUTTONDOWN && !this.log.IsMouseUnderLog())
        {
            ClipboardService.SetText(GameManager.MousePosition.ToCopyFormat());
        }
    }
}
