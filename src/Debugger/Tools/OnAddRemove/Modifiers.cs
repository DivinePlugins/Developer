namespace Debugger.Tools.OnAddRemove;

using System.Collections.Generic;
using System.Threading.Tasks;

using Debugger.Menus;
using Debugger.Metadata;

using Divine.Entity.Entities.Units.Heroes;
using Divine.Menu;
using Divine.Menu.EventArgs;
using Divine.Menu.Items;
using Divine.Modifier;
using Divine.Modifier.EventArgs;
using Divine.Modifier.Modifiers;
using Divine.Numerics;

using Logger;

[Priority(99)]
internal sealed class Modifiers : IDebuggerTool
{
    private readonly HashSet<string> ignored =
    [
        "modifier_projectile_vision",
        "modifier_truesight",
        "modifier_creep_haste",
        "modifier_creep_slow",
        "modifier_tower_aura",
        "modifier_tower_truesight_aura"
    ];

    private readonly MenuSwitcher addEnabled;

    private readonly MenuSwitcher heroesOnly;

    private readonly MenuSwitcher ignoreUseless;

    private readonly ILog log;

    private readonly Menu menu;

    private readonly MenuSwitcher removeEnabled;

    public Modifiers(IMainMenu mainMenu, ILog log)
    {
        this.log = log;

        this.menu = mainMenu.OnAddRemoveMenu.AddMenu("Modifiers", true, false);
        this.addEnabled = this.menu.AddSwitcher("On add enabled", false).SetTooltip("Unit.OnModifierAdded");
        this.removeEnabled = this.menu.AddSwitcher("On remove enabled", false).SetTooltip("Unit.OnModifierRemoved");
        this.heroesOnly = this.menu.AddSwitcher("Heroes only", false);
        this.ignoreUseless = this.menu.AddSwitcher("Ignore useless", true);
    }

    public void Activate()
    {
        menu.ValueChanged += OnMenuOnValueChanged;
    }

    public void Dispose()
    {
        menu.ValueChanged -= OnMenuOnValueChanged;
    }

    private void OnMenuOnValueChanged(Menu sender, SwitcherChangedEventArgs e)
    {
        if (e.Value)
        {
            this.addEnabled.ValueChanged += this.AddEnabledPropertyChanged;
            this.removeEnabled.ValueChanged += this.RemoveEnabledOnPropertyChanged;
        }
        else
        {
            this.addEnabled.ValueChanged -= this.AddEnabledPropertyChanged;
            this.removeEnabled.ValueChanged -= this.RemoveEnabledOnPropertyChanged;
        }
    }

    private void AddEnabledPropertyChanged(MenuSwitcher switcher, SwitcherChangedEventArgs e)
    {
        if (e.Value)
        {
            ModifierManager.ModifierAdded += this.UnitOnModifierAdded;
        }
        else
        {
            ModifierManager.ModifierAdded -= this.UnitOnModifierAdded;
        }
    }

    private void RemoveEnabledOnPropertyChanged(MenuSwitcher switcher, SwitcherChangedEventArgs e)
    {
        if (e.Value)
        {
            ModifierManager.ModifierRemoved += this.UnitOnModifierRemoved;
        }
        else
        {
            ModifierManager.ModifierRemoved -= this.UnitOnModifierRemoved;
        }
    }

    private bool IsValid(Modifier modifier)
    {
        if (modifier?.IsValid != true)
        {
            return false;
        }

        var sender = modifier.Owner;
        if (sender?.IsValid != true)
        {
            return false;
        }

        if (this.heroesOnly && sender is not Hero)
        {
            return false;
        }

        if (this.ignoreUseless && this.ignored.Contains(modifier.Name))
        {
            return false;
        }

        return true;
    }

    private async void UnitOnModifierAdded(ModifierAddedEventArgs e)
    {
        if (e.IsCollection)
        {
            return;
        }

        await Task.Delay(1);
        var modifier = e.Modifier;

        if (!this.IsValid(modifier))
        {
            return;
        }

        var item = new LogItem(LogType.Modifier, Color.LightGreen, "Modifier added");

        item.AddLine("Name: " + modifier.Name, modifier.Name);
        item.AddLine("Texture name: " + modifier.TextureName, modifier.TextureName);
        item.AddLine("Elapsed time: " + modifier.ElapsedTime, modifier.ElapsedTime);
        item.AddLine("Remaining time: " + modifier.RemainingTime, modifier.RemainingTime);

        var sender = modifier.Owner;
        item.AddLine("Sender name: " + sender.Name, sender.Name);
        item.AddLine("Sender network name: " + sender.NetworkName, sender.NetworkName);
        item.AddLine("Sender classID: " + sender.ClassId, sender.ClassId);

        this.log.Display(item);
    }

    private void UnitOnModifierRemoved(ModifierRemovedEventArgs e)
    {
        var modifier = e.Modifier;

        if (!this.IsValid(modifier))
        {
            return;
        }

        var item = new LogItem(LogType.Modifier, Color.LightPink, "Modifier removed");

        item.AddLine("Name: " + modifier.Name, modifier.Name);
        item.AddLine("Texture name: " + modifier.TextureName, modifier.TextureName);

        var sender = modifier.Owner;
        item.AddLine("Sender name: " + sender.Name, sender.Name);
        item.AddLine("Sender network name: " + sender.NetworkName, sender.NetworkName);
        item.AddLine("Sender classID: " + sender.ClassId, sender.ClassId);

        this.log.Display(item);
    }
}
