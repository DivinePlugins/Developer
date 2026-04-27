namespace Debugger.Tools.OnChange;

using Debugger.Menus;
using Debugger.Metadata;

using Divine.Entity.Entities;
using Divine.Entity.Entities.EventArgs;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Menu;
using Divine.Menu.EventArgs;
using Divine.Menu.Items;
using Divine.Numerics;

using Logger;

[Priority(95)]
internal sealed class Animations : IDebuggerTool
{
    private readonly MenuSwitcher heroesOnly;

    private readonly ILog log;

    private readonly Menu menu;

    public Animations(IMainMenu mainMenu, ILog log)
    {
        this.log = log;

        this.menu = mainMenu.OnChangeMenu.AddMenu("Animations", true, false).SetTooltip("Entity.OnAnimationChanged");
        this.heroesOnly = this.menu.AddSwitcher("Heroes only", false);
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
            Entity.AnimationChanged += this.EntityOnAnimationChanged;
        }
        else
        {
            Entity.AnimationChanged -= this.EntityOnAnimationChanged;
        }
    }

    private void EntityOnAnimationChanged(Entity sender, AnimationChangedEventArgs e)
    {
        if (!this.IsValid(sender))
        {
            return;
        }

        var item = new LogItem(LogType.Animation, Color.Cyan, "Animation changed");

        item.AddLine("Name: " + e.Name, e.Name);
        item.AddLine("Sender name: " + sender.Name, sender.Name);
        item.AddLine("Sender network name: " + sender.NetworkName, sender.NetworkName);
        item.AddLine("Sender classID: " + sender.ClassId, sender.ClassId);

        this.log.Display(item);
    }

    private bool IsValid(Entity sender)
    {
        if (this.heroesOnly && sender is not Hero)
        {
            return false;
        }

        return true;
    }
}
