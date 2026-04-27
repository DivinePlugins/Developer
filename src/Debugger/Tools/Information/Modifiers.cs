namespace Debugger.Tools.Information;

using System.Linq;

using Debugger.Menus;
using Debugger.Metadata;

using Divine.Entity;
using Divine.Entity.Entities.Players;
using Divine.Entity.Entities.Units;
using Divine.Game;
using Divine.Game.EventArgs;
using Divine.Menu.EventArgs;
using Divine.Menu.Items;
using Divine.Numerics;
using Divine.Update;

using Logger;

[Priority(79)]
internal sealed class Modifiers : IDebuggerTool
{
    private Player player;

    private readonly MenuSwitcher autoUpdate;

    private readonly MenuButton information;

    private uint lastUnitInfo;

    private readonly ILog log;

    private readonly Menu menu;

    private readonly MenuSwitcher showElapsedTime;

    private readonly MenuSwitcher showHidden;

    private readonly MenuSwitcher showRemainingTime;

    private readonly MenuSwitcher showTextureName;

    public Modifiers(IMainMenu mainMenu, ILog log)
    {
        this.log = log;

        this.menu = mainMenu.InformationMenu.AddMenu("Modifiers");
        this.information = this.menu.AddButton("Get");
        this.autoUpdate = this.menu.AddSwitcher("Auto update", false);
        this.showHidden = this.menu.AddSwitcher("Show hidden", false);
        this.showTextureName = this.menu.AddSwitcher("Show texture name", false);
        this.showRemainingTime = this.menu.AddSwitcher("Show remaining time", false);
        this.showElapsedTime = this.menu.AddSwitcher("Show elapsed time", false);
    }

    public void Activate()
    {
        this.player = EntityManager.LocalPlayer;

        this.information.Clicked += this.InformationOnPropertyChanged;
        this.autoUpdate.ValueChanged += this.AutoUpdateOnPropertyChanged;
    }

    public void Deactivate()
    {
        this.information.Clicked -= this.InformationOnPropertyChanged;
        this.autoUpdate.ValueChanged -= this.AutoUpdateOnPropertyChanged;
    }

    private void AutoUpdateOnPropertyChanged(MenuSwitcher switcher, SwitcherChangedEventArgs e)
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
        if (e.GameEvent.Name is not "dota_player_update_selected_unit" and not "dota_player_update_query_unit")
        {
            return;
        }

        var unit = (this.player.QueryUnit ?? this.player.SelectedUnits.FirstOrDefault()) as Unit;
        if (unit?.IsValid != true)
        {
            return;
        }

        if (unit.Handle == this.lastUnitInfo)
        {
            return;
        }

        this.InformationOnPropertyChanged(null);
    }

    private void InformationOnPropertyChanged(MenuButton sender)
    {
        UpdateManager.BeginInvoke(() =>
        {
            var unit = (this.player.QueryUnit ?? this.player.SelectedUnits.FirstOrDefault()) as Unit;
            if (unit?.IsValid != true)
            {
                return;
            }

            this.lastUnitInfo = unit.Handle;

            var item = new LogItem(LogType.Modifier, Color.PaleGreen, "Modifiers information");

            item.AddLine("Unit name: " + unit.Name, unit.Name);
            item.AddLine("Unit network name: " + unit.NetworkName, unit.NetworkName);
            item.AddLine("Unit classID: " + unit.ClassId, unit.ClassId);

            foreach (var modifier in unit.Modifiers)
            {
                if (!this.showHidden && modifier.IsHidden)
                {
                    continue;
                }

                var modifierItem = new LogItem(LogType.Modifier, Color.PaleGreen);

                modifierItem.AddLine("Name: " + modifier.Name, modifier.Name);

                if (this.showTextureName)
                {
                    modifierItem.AddLine("Texture name: " + modifier.TextureName, modifier.TextureName);
                }

                if (this.showHidden)
                {
                    modifierItem.AddLine("Is hidden: " + modifier.IsHidden, modifier.IsHidden);
                }

                if (this.showElapsedTime)
                {
                    modifierItem.AddLine("Elapsed time: " + modifier.ElapsedTime, modifier.ElapsedTime);
                }

                if (this.showRemainingTime)
                {
                    modifierItem.AddLine("Remaining time: " + modifier.RemainingTime, modifier.RemainingTime);
                }

                this.log.Display(modifierItem);
            }

            this.log.Display(item);
        });
    }
}
