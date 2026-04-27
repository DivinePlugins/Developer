namespace Debugger.Tools.OnChange;

using System.Collections.Generic;
using System.Threading.Tasks;

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

[Priority(92)]
internal sealed class Handles : IDebuggerTool
{
    private readonly HashSet<string> ignored =
    [
        "m_hModifierParent",
        "m_hModel",
        "m_hEffectEntity",
        "m_hParent"
    ];

    private readonly HashSet<string> semiIgnored =
    [
        "m_hOwnerEntity",
        "m_hTowerAttackTarget"
    ];

    private readonly MenuSwitcher heroesOnly;

    private readonly MenuSwitcher ignoreSemiUseless;

    private readonly MenuSwitcher ignoreUseless;

    private readonly ILog log;

    private readonly IMainMenu mainMenu;

    private readonly Menu menu;

    public Handles(IMainMenu mainMenu, ILog log)
    {
        this.mainMenu = mainMenu;
        this.log = log;

        this.menu = this.mainMenu.OnPropertyChange.AddMenu("Handles", true, false).SetTooltip("Entity.OnHandlePropertyChange");
        this.heroesOnly = this.menu.AddSwitcher("Heroes only", false);
        this.ignoreSemiUseless = this.menu.AddSwitcher("Ignore semi useless", false);
        this.ignoreUseless = this.menu.AddSwitcher("Ignore useless", true);
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
            Entity.NetworkPropertyChanged += this.OnHandlePropertyChange;
        }
        else
        {
            Entity.NetworkPropertyChanged -= this.OnHandlePropertyChange;
        }
    }

    private bool IsValid(Entity sender, NetworkPropertyChangedEventArgs e)
    {
        if (!e.ValueTypeName.StartsWith("CHandle"))
        {
            return false;
        }

        if (e.OldValue.GetEntity() == e.NewValue.GetEntity())
        {
            return false;
        }

        if (this.ignoreUseless && this.ignored.Contains(e.PropertyName))
        {
            return false;
        }

        if (this.ignoreSemiUseless && this.semiIgnored.Contains(e.PropertyName))
        {
            return false;
        }

        if (this.heroesOnly && sender is not Hero && sender.Owner is not Hero)
        {
            return false;
        }

        return true;
    }

    private async void OnHandlePropertyChange(Entity sender, NetworkPropertyChangedEventArgs e)
    {
        await Task.Delay(1);

        if (!this.IsValid(sender, e))
        {
            return;
        }

        var item = new LogItem(LogType.Handle, Color.Cyan, "Handle changed");

        item.AddLine("Property name: " + e.PropertyName, e.PropertyName);
        item.AddLine(
            "Property values: "
            + e.OldValue.GetEntity()?.Name + " => "
            + e.NewValue.GetEntity()?.Name,
            e.NewValue.GetEntity()?.Name);
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
