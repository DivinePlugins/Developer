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

[Priority(89)]
internal sealed class Strings : IDebuggerTool
{
    private readonly MenuSwitcher heroesOnly;

    private readonly ILog log;

    private readonly Menu menu;

    public Strings(IMainMenu mainMenu, ILog log)
    {
        this.log = log;

        this.menu = mainMenu.OnPropertyChange.AddMenu("Strings", true, false).SetTooltip("Entity.OnStringPropertyChange");
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
            Entity.NetworkPropertyChanged += this.OnStringPropertyChange;
        }
        else
        {
            Entity.NetworkPropertyChanged -= this.OnStringPropertyChange;
        }
    }

    private bool IsValid(Entity sender, NetworkPropertyChangedEventArgs e)
    {
        if (e.ValueTypeName is not "char*")
        {
            return false;
        }

        if (e.OldValue.GetString() == e.NewValue.GetString())
        {
            return false;
        }

        if (this.heroesOnly && sender is not Hero && sender.Owner is not Hero)
        {
            return false;
        }

        return true;
    }

    private void OnStringPropertyChange(Entity sender, NetworkPropertyChangedEventArgs e)
    {
        if (!this.IsValid(sender, e))
        {
            return;
        }

        var item = new LogItem(LogType.String, Color.Cyan, "String changed");

        item.AddLine("Property name: " + e.PropertyName, e.PropertyName);
        item.AddLine("Property values: " + e.OldValue.GetString() + " => " + e.NewValue.GetString(), e.NewValue.GetString());
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
