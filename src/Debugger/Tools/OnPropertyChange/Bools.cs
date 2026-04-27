namespace Debugger.Tools.OnChange;

using System.Collections.Generic;

using Debugger.Menus;
using Debugger.Metadata;

using Divine.Entity.Entities;
using Divine.Entity.Entities.EventArgs;
using Divine.Entity.Entities.Units;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Menu;
using Divine.Menu.EventArgs;
using Divine.Menu.Items;
using Divine.Numerics;

using Logger;

[Priority(94)]
internal sealed class Bools : IDebuggerTool
{
    private readonly HashSet<string> ignored =
    [
        "m_bIsMoving",
        "m_bHidden",
        "m_bIsGeneratingEconItem",
        "m_bInitialized",
        "m_bCanBeDominated",
        "m_bSelectionRingVisible",
        "m_bActivated",
        "m_bSellable",
        "m_bKillable",
        "m_bPurchasable",
        "m_bDroppable",
        "m_bCombinable",
        "m_bStackable",
        "m_bValid"
    ];

    private readonly MenuSwitcher heroesOnly;

    private readonly MenuSwitcher ignoreUseless;

    private readonly ILog log;

    private readonly Menu menu;

    public Bools(IMainMenu mainMenu, ILog log)
    {
        this.log = log;

        this.menu = mainMenu.OnPropertyChange.AddMenu("Bools", true, false).SetTooltip("Entity.OnBoolPropertyChange");
        this.heroesOnly = this.menu.AddSwitcher("Heroes only", false);
        this.ignoreUseless = this.menu.AddSwitcher("Ignore useless", true);
    }

    public void Activate()
    {
        this.menu.ValueChanged += this.EnabledOnPropertyChanged;
    }

    public void Dispose()
    {
        this.menu.ValueChanged -= this.EnabledOnPropertyChanged;
    }

    private void EnabledOnPropertyChanged(Menu switcher, SwitcherChangedEventArgs e)
    {
        if (e.Value)
        {
            Entity.NetworkPropertyChanged += this.OnBoolPropertyChange;
        }
        else
        {
            Entity.NetworkPropertyChanged -= this.OnBoolPropertyChange;
        }
    }

    private bool IsValid(Entity sender, NetworkPropertyChangedEventArgs e)
    {
        if (e.ValueTypeName != "bool")
        {
            return false;
        }

        if (e.OldValue.GetBoolean() == e.NewValue.GetBoolean())
        {
            return false;
        }

        if (this.ignoreUseless && this.ignored.Contains(e.PropertyName))
        {
            return false;
        }

        if (this.heroesOnly && sender is not Hero && sender.Owner is not Hero)
        {
            return false;
        }

        return true;
    }

    private void OnBoolPropertyChange(Entity sender, NetworkPropertyChangedEventArgs e)
    {
        if (!this.IsValid(sender, e))
        {
            return;
        }

        var item = new LogItem(LogType.Bool, Color.Cyan, "Bool changed");

        item.AddLine("Property name: " + e.PropertyName, e.PropertyName);
        item.AddLine("Property values: " + e.OldValue.GetBoolean() + " => " + e.NewValue.GetBoolean(), e.NewValue.GetBoolean());
        item.AddLine("Sender name: " + sender.Name, sender.Name);
        item.AddLine("Sender network name: " + sender.NetworkName, sender.NetworkName);
        item.AddLine("Sender classID: " + sender.ClassId, sender.ClassId);

        if (sender.Owner is Unit)
        {
            item.AddLine("Owner name: " + sender.Owner.Name, sender.Owner.Name);
            item.AddLine("Owner network name: " + sender.Owner.NetworkName, sender.Owner.NetworkName);
            item.AddLine("Owner classID: " + sender.Owner.ClassId, sender.Owner.ClassId);
        }

        this.log.Display(item);
    }
}
