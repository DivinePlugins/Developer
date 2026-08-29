namespace Debugger.Tools.OnAddRemove;

using System.Collections.Generic;

using Debugger.Menus;
using Debugger.Metadata;

using Divine.Entity;
using Divine.Entity.Entities.Units;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Entity.EventArgs;
using Divine.Menu;
using Divine.Menu.EventArgs;
using Divine.Menu.Items;
using Divine.Numerics;

using Logger;

[Priority(100)]
internal sealed class Units : IDebuggerTool
{
    private readonly HashSet<string> ignored =
    [
        "portrait_world_unit"
    ];

    private readonly MenuSwitcher addEnabled;

    private readonly MenuSwitcher heroesOnly;

    private readonly MenuSwitcher ignoreUseless;

    private readonly ILog log;

    private readonly Menu menu;

    private readonly MenuSwitcher removeEnabled;

    public Units(IMainMenu mainMenu, ILog log)
    {
        this.log = log;

        this.menu = mainMenu.OnAddRemoveMenu.AddMenu("Units", true, false);

        this.addEnabled = this.menu.AddSwitcher("On add enabled", false).SetTooltip("EntityManager<Unit>.EntityAdded");
        this.removeEnabled = this.menu.AddSwitcher("On remove enabled", false).SetTooltip("EntityManager<Unit>.EntityRemoved");
        this.heroesOnly = this.menu.AddSwitcher("Heroes only", false);
        this.ignoreUseless = this.menu.AddSwitcher("Ignore useless", true);
    }

    public void Activate()
    {
        menu.ValueChanged += OnMenuValueChanged;
    }

    public void Deactivate()
    {
        menu.ValueChanged -= OnMenuValueChanged;
    }

    private void OnMenuValueChanged(Menu sender, SwitcherChangedEventArgs e)
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
            EntityManager.EntityAdded += this.EntityManagerOnEntityAdded;
        }
        else
        {
            EntityManager.EntityAdded -= this.EntityManagerOnEntityAdded;
        }
    }

    private void RemoveEnabledOnPropertyChanged(MenuSwitcher switcher, SwitcherChangedEventArgs e)
    {
        if (e.Value)
        {
            EntityManager.EntityRemoved += this.EntityManagerOnEntityRemoved;
        }
        else
        {
            EntityManager.EntityRemoved -= this.EntityManagerOnEntityRemoved;
        }
    }

    private void EntityManagerOnEntityAdded(EntityAddedEventArgs e)
    {
        if (e.IsCollection)
        {
            return;
        }

        if (e.Entity is not Unit unit)
        {
            return;
        }

        if (!this.IsValid(unit))
        {
            return;
        }

        var item = new LogItem(LogType.Unit, Color.LightGreen, "Unit added");

        item.AddLine("Name: " + unit.Name, unit.Name);
        item.AddLine("Network name: " + unit.NetworkName, unit.NetworkName);
        item.AddLine("ClassID: " + unit.ClassId, unit.ClassId);
        item.AddLine("Position: " + unit.Position, unit.Position);
        item.AddLine("Attack capability: " + unit.AttackCapability, unit.AttackCapability);
        item.AddLine("Move capability: " + unit.MoveCapability, unit.MoveCapability);
        item.AddLine("Vision: " + unit.DayVision + "/" + unit.NightVision, unit.DayVision + "/" + unit.NightVision);
        item.AddLine("Health: " + unit.Health, unit.Health);

        this.log.Display(item);
    }

    private void EntityManagerOnEntityRemoved(EntityRemovedEventArgs e)
    {
        if (e.Entity is not Unit unit)
        {
            return;
        }

        if (!this.IsValid(unit))
        {
            return;
        }

        var item = new LogItem(LogType.Unit, Color.LightPink, "Unit removed");

        item.AddLine("Name: " + unit.Name, unit.Name);
        item.AddLine("Network name: " + unit.NetworkName, unit.NetworkName);
        item.AddLine("ClassID: " + unit.ClassId, unit.ClassId);
        item.AddLine("Position: " + unit.Position, unit.Position);

        this.log.Display(item);
    }

    private bool IsValid(Unit entity)
    {
        if (entity?.IsValid != true)
        {
            return false;
        }

        if (this.ignoreUseless && this.ignored.Contains(entity.Name))
        {
            return false;
        }

        if (this.heroesOnly && entity is not Hero)
        {
            return false;
        }

        return true;
    }
}
