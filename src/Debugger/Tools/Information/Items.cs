namespace Debugger.Tools.Information;

using System.Collections.Generic;
using System.Linq;
using System.Text;

using Debugger.Menus;
using Debugger.Metadata;

using Divine.Entity;
using Divine.Entity.Entities.Abilities.Items;
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

[Priority(80)]
internal class Items : IDebuggerTool
{
    private Player player;

    private readonly MenuSwitcher autoUpdate;

    private readonly MenuButton information;

    private uint lastUnitInfo;

    private readonly ILog log;

    private readonly Menu menu;

    private readonly MenuSwitcher showBackpack;

    private readonly MenuSwitcher showBehavior;

    private readonly MenuSwitcher showCastRange;

    private readonly MenuSwitcher showInventory;

    private readonly MenuSwitcher showManaCost;

    private readonly MenuSwitcher showSpecialData;

    private readonly MenuSwitcher showStash;

    private readonly MenuSwitcher showTargetType;

    public Items(IMainMenu mainMenu, ILog log)
    {
        this.log = log;

        this.menu = mainMenu.InformationMenu.AddMenu("Items");
        this.information = this.menu.AddButton("Get");
        this.autoUpdate = this.menu.AddSwitcher("Auto update", false);
        this.showInventory = this.menu.AddSwitcher("Show inventory items", true);
        this.showBackpack = this.menu.AddSwitcher("Show backpack items", false);
        this.showStash = this.menu.AddSwitcher("Show stash items", false);
        this.showManaCost = this.menu.AddSwitcher("Show mana cost", false);
        this.showCastRange = this.menu.AddSwitcher("Show cast range", false);
        this.showBehavior = this.menu.AddSwitcher("Show behavior", false);
        this.showTargetType = this.menu.AddSwitcher("Show target type", false);
        this.showSpecialData = this.menu.AddSwitcher("Show all special data", false);
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
            if (unit?.IsValid != true || !unit.HasInventory)
            {
                return;
            }

            this.lastUnitInfo = unit.Handle;

            var item = new LogItem(LogType.Item, Color.PaleGreen, "Items information");

            item.AddLine("Unit name: " + unit.Name, unit.Name);
            item.AddLine("Unit network name: " + unit.NetworkName, unit.NetworkName);
            item.AddLine("Unit classID: " + unit.ClassId, unit.ClassId);

            var localizeName = LocalizationHelper.LocalizeName(unit);
            item.AddLine("Unit display name: " + localizeName, localizeName);

            var items = new List<Item>();

            if (this.showInventory)
            {
                items.AddRange(unit.Inventory.Items);
            }

            if (this.showBackpack)
            {
                items.AddRange(unit.Inventory.BackpackItems);
            }

            if (this.showStash)
            {
                items.AddRange(unit.Inventory.StashItems);
            }

            foreach (var ability in items.Reverse<Item>())
            {
                var abilityItem = new LogItem(LogType.Spell, Color.PaleGreen);

                abilityItem.AddLine("Name: " + ability.Name, ability.Name);
                abilityItem.AddLine("Network name: " + ability.NetworkName, ability.NetworkName);
                abilityItem.AddLine("ClassID: " + ability.ClassId, ability.ClassId);

                var localizeAbilityName = LocalizationHelper.LocalizeAbilityName(ability.Name);
                abilityItem.AddLine("Display name: " + localizeAbilityName, localizeAbilityName);

                if (this.showManaCost)
                {
                    abilityItem.AddLine("Mana cost: " + ability.ManaCost, ability.ManaCost);
                }

                if (this.showCastRange)
                {
                    abilityItem.AddLine("Cast range: " + ability.CastRange, ability.CastRange);
                }

                if (this.showBehavior)
                {
                    abilityItem.AddLine("Behavior: " + ability.AbilityBehavior, ability.AbilityBehavior);
                }

                if (this.showTargetType)
                {
                    abilityItem.AddLine("Target type: " + ability.TargetType, ability.TargetType);
                    abilityItem.AddLine("Target team type: " + ability.TargetTeamType, ability.TargetTeamType);
                }

                if (this.showSpecialData)
                {
                    abilityItem.AddLine("Special data =>");
                    foreach (var abilitySpecialData in ability.AbilitySpecialData.Where(x => !x.Name.StartsWith('#')))
                    {
                        var values = new StringBuilder();
                        var count = abilitySpecialData.Count;

                        for (uint i = 0; i < count; i++)
                        {
                            values.Append(abilitySpecialData.GetValue(i));
                            if (i < count - 1)
                            {
                                values.Append(", ");
                            }
                        }

                        abilityItem.AddLine("  " + abilitySpecialData.Name + ": " + values, abilitySpecialData.Name);
                    }
                }

                this.log.Display(abilityItem);
            }

            this.log.Display(item);
        });
    }
}
