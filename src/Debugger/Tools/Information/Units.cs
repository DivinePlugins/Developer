namespace Debugger.Tools.Information;

using System.Linq;

using Debugger.Menus;
using Debugger.Metadata;

using Divine.Entity;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Players;
using Divine.Entity.Entities.Units;
using Divine.Game;
using Divine.Game.EventArgs;
using Divine.Helpers;
using Divine.Menu.EventArgs;
using Divine.Menu.Items;
using Divine.Numerics;
using Divine.Update;

using Logger;

[Priority(78)]
internal sealed class Units : IDebuggerTool
{
    private Player player;

    private readonly MenuSwitcher autoUpdate;

    private readonly MenuButton information;

    private uint lastUnitInfo;

    private readonly ILog log;

    private readonly Menu menu;

    private readonly MenuSwitcher showItemInfo;

    private readonly MenuSwitcher showLevel;

    private readonly MenuSwitcher showModifierInfo;

    private readonly MenuSwitcher showsAbilityInfo;

    private readonly MenuSwitcher showsHpMp;

    private readonly MenuSwitcher showState;

    private readonly MenuSwitcher showTeam;

    private readonly MenuSwitcher showVision;

    public Units(IMainMenu mainMenu, ILog log)
    {
        this.log = log;

        this.menu = mainMenu.InformationMenu.AddMenu("Units");
        this.information = this.menu.AddButton("Get");
        this.autoUpdate = this.menu.AddSwitcher("Auto update", false);
        this.showTeam = this.menu.AddSwitcher("Show team", true);
        this.showLevel = this.menu.AddSwitcher("Show level", true);
        this.showsHpMp = this.menu.AddSwitcher("Show hp/mp", true);
        this.showVision = this.menu.AddSwitcher("Show vision", true);
        this.showState = this.menu.AddSwitcher("Show state", true);
        this.showsAbilityInfo = this.menu.AddSwitcher("Show ability information", false);
        this.showItemInfo = this.menu.AddSwitcher("Show item information", false);
        this.showModifierInfo = this.menu.AddSwitcher("Show modifier information", false);
    }

    public void Activate()
    {
        this.player = EntityManager.LocalPlayer;

        this.information.Clicked += this.InformationOnPropertyChanged;
        this.autoUpdate.ValueChanged += this.AutoUpdateOnPropertyChanged;
    }

    public void Dispose()
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

            var item = new LogItem(LogType.Unit, Color.PaleGreen, "Unit information");

            item.AddLine("Unit name: " + unit.Name, unit.Name);
            item.AddLine("Unit network name: " + unit.NetworkName, unit.NetworkName);
            item.AddLine("Unit classID: " + unit.ClassId, unit.ClassId);
            var localizeName = LocalizationHelper.LocalizeName(unit);
            item.AddLine("Unit display name: " + localizeName, localizeName);
            item.AddLine("Unit type: " + unit.UnitType, unit.UnitType);
            item.AddLine("Unit position: " + unit.Position, unit.Position);
            if (this.showLevel)
            {
                item.AddLine("Unit level: " + unit.Level, unit.Level);
            }

            if (this.showTeam)
            {
                item.AddLine("Unit team: " + unit.Team, unit.Team);
            }

            if (this.showsHpMp)
            {
                item.AddLine("Unit health: " + unit.Health + "/" + unit.MaximumHealth);
                item.AddLine("Unit mana: " + (int)unit.Mana + "/" + (int)unit.MaximumMana);
            }

            item.AddLine("Unit attack capability: " + unit.AttackCapability, unit.AttackCapability);
            if (this.showVision)
            {
                item.AddLine("Unit vision: " + unit.DayVision + "/" + unit.NightVision);
            }

            if (this.showState)
            {
                item.AddLine("Unit state: " + unit.UnitState, unit.UnitState);
            }

            if (this.showsAbilityInfo)
            {
                item.AddLine("Abilities =>");
                item.AddLine("  Talents count: " + unit.Spellbook.Spells.Count(x => x.Name.StartsWith("special_")));
                item.AddLine(
                    "  Active spells count: " + unit.Spellbook.Spells.Count(
                        x => !x.Name.StartsWith("special_") && x.AbilityBehavior != AbilityBehavior.Passive));
                item.AddLine(
                    "  Passive spells count: " + unit.Spellbook.Spells.Count(
                        x => !x.Name.StartsWith("special_") && x.AbilityBehavior == AbilityBehavior.Passive));
            }

            if (this.showItemInfo && unit.HasInventory)
            {
                item.AddLine("Items =>");
                item.AddLine("  Inventory Items count: " + unit.Inventory.Items.Count());
                item.AddLine("  Backpack Items count: " + unit.Inventory.BackpackItems.Count());
                item.AddLine("  Stash Items count: " + unit.Inventory.StashItems.Count());
            }

            if (this.showModifierInfo)
            {
                item.AddLine("Modifiers =>");
                item.AddLine("  Active modifiers count: " + unit.Modifiers.Count(x => !x.IsHidden));
                item.AddLine("  Hidden modifiers count: " + unit.Modifiers.Count(x => x.IsHidden));
            }

            this.log.Display(item);
        });
    }
}
