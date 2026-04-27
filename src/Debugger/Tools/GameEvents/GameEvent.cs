namespace Debugger.Tools.GameEvents;

using System.Collections.Generic;

using Debugger.Menus;
using Debugger.Metadata;

using Divine.Game;
using Divine.Game.EventArgs;
using Divine.Menu;
using Divine.Menu.EventArgs;
using Divine.Menu.Items;
using Divine.Numerics;

using Logger;

[Priority(85)]
internal class GameEvent : IDebuggerTool
{
    private readonly HashSet<string> ignored =
    [
        "dota_action_success"
    ];

    private readonly MenuSwitcher ignoreUseless;

    private readonly ILog log;

    private readonly Menu menu;

    public GameEvent(IMainMenu mainMenu, ILog log)
    {
        this.log = log;

        this.menu = mainMenu.GameEventsMenu.AddMenu("Game event", true, false).SetTooltip("Game.OnGameEvent");
        this.ignoreUseless = this.menu.AddSwitcher("Ignore useless");
    }

    public void Activate()
    {
        this.menu.ValueChanged += this.EnabledOnPropertyChanged;
    }

    public void Deactivate()
    {
        this.menu.ValueChanged -= this.EnabledOnPropertyChanged;
    }

    private void EnabledOnPropertyChanged(Menu switcher, SwitcherChangedEventArgs e)
    {
        if (e.Value)
        {
            GameManager.GameEvent += this.GameOnGameEvent;
        }
        else
        {
            GameManager.GameEvent -= this.GameOnGameEvent;
        }
    }

    private void GameOnGameEvent(GameEventEventArgs e)
    {
        if (!this.IsValid(e))
        {
            return;
        }

        var item = new LogItem(LogType.GameEvent, Color.Yellow, "Game event");

        item.AddLine("Name: " + e.GameEvent.Name, e.GameEvent.Name);

        this.log.Display(item);
    }

    private bool IsValid(GameEventEventArgs e)
    {
        if (this.ignoreUseless && this.ignored.Contains(e.GameEvent.Name))
        {
            return false;
        }

        return true;
    }
}
