namespace Debugger.Tools.OnChange;

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

[Priority(90)]
internal sealed class UInt64 : IDebuggerTool
{
    private readonly MenuSwitcher heroesOnly;

    private readonly ILog log;

    private readonly Menu menu;

    public UInt64(IMainMenu mainMenu, ILog log)
    {
        this.log = log;

        this.menu = mainMenu.OnPropertyChange.AddMenu("UInt64", true, false).SetTooltip("Entity.OnUInt64PropertyChange");
        this.heroesOnly = this.menu.AddSwitcher("Heroes only", false);
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
            Entity.NetworkPropertyChanged += this.OnUInt64PropertyChange;
        }
        else
        {
            Entity.NetworkPropertyChanged -= this.OnUInt64PropertyChange;
        }
    }

    private bool IsValid(Entity sender, NetworkPropertyChangedEventArgs e)
    {
        if (e.ValueTypeName is not "uint64")
        {
            return false;
        }

        if (e.OldValue.GetUInt64() == e.NewValue.GetUInt64())
        {
            return false;
        }

        if (this.heroesOnly && sender is not Hero && sender.Owner is not Hero)
        {
            return false;
        }

        return true;
    }

    private void OnUInt64PropertyChange(Entity sender, NetworkPropertyChangedEventArgs e)
    {
        if (!this.IsValid(sender, e))
        {
            return;
        }

        var item = new LogItem(LogType.UInt64, Color.Cyan, "UInt64 changed");

        item.AddLine("Property name: " + e.PropertyName, e.PropertyName);
        item.AddLine("Property values: " + e.OldValue.GetUInt64() + " => " + e.NewValue.GetUInt64(), e.NewValue.GetUInt64());
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
