namespace Debugger.Tools.OnAddRemove;

using System.Collections.Generic;

using Debugger.Menus;
using Debugger.Metadata;

using Divine.Entity;
using Divine.Entity.Entities;
using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Entity.EventArgs;
using Divine.Menu;
using Divine.Menu.EventArgs;
using Divine.Menu.Items;
using Divine.Numerics;

using Logger;

[Priority(97)]
internal sealed class Abilities : IDebuggerTool
{
    private readonly HashSet<string> ignored =
    [
        "special_",
        "_empty",
        "_hidden"
    ];

    private readonly MenuSwitcher addEnabled;

    private readonly MenuSwitcher heroesOnly;

    private readonly MenuSwitcher ignoreUseless;

    private readonly ILog log;

    private readonly Menu menu;

    private readonly MenuSwitcher removeEnabled;

    public Abilities(IMainMenu mainMenu, ILog log)
    {
        this.log = log;

        this.menu = mainMenu.OnAddRemoveMenu.AddMenu("Abilities", true, false);
        this.addEnabled = this.menu.AddSwitcher("On add enabled", false).SetTooltip("EntityManager<Ability>.EntityAdded");
        this.removeEnabled = this.menu.AddSwitcher("On remove enabled", false).SetTooltip("EntityManager<Ability>.EntityRemoved");
        this.heroesOnly = this.menu.AddSwitcher("Heroes only", false);
        this.ignoreUseless = this.menu.AddSwitcher("Ignore useless", true);
    }

    public void Activate()
    {
        menu.ValueChanged += OnMenuValueChanged;
    }

    public void Dispose()
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

        if (e.Entity is not Ability ability)
        {
            return;
        }

        if (!this.IsValid(ability))
        {
            return;
        }

        var item = new LogItem(LogType.Ability, Color.LightGreen, "Ability added");

        item.AddLine("Name: " + ability.Name, ability.Name);
        item.AddLine("Network name: " + ability.NetworkName, ability.NetworkName);
        item.AddLine("ClassID: " + ability.ClassId, ability.ClassId);
        item.AddLine("Owner name: " + ability.Owner?.Name, ability.Owner?.Name);
        item.AddLine("Owner network name: " + ability.Owner?.NetworkName, ability.Owner?.NetworkName);
        item.AddLine("Owner classID: " + ability.Owner?.ClassId, ability.Owner?.ClassId);

        this.log.Display(item);
    }

    private void EntityManagerOnEntityRemoved(EntityRemovedEventArgs e)
    {
        if (e.Entity is not Ability ability)
        {
            return;
        }

        if (!this.IsValid(ability))
        {
            return;
        }

        var item = new LogItem(LogType.Ability, Color.LightPink, "Ability removed");

        item.AddLine("Name: " + ability.Name, ability.Name);
        item.AddLine("Network name: " + ability.NetworkName, ability.NetworkName);
        item.AddLine("ClassID: " + ability.ClassId, ability.ClassId);
        item.AddLine("Owner name: " + ability.Owner?.Name, ability.Owner?.Name);
        item.AddLine("Owner network name: " + ability.Owner?.NetworkName, ability.Owner?.NetworkName);
        item.AddLine("Owner classID: " + ability.Owner?.ClassId, ability.Owner?.ClassId);

        this.log.Display(item);
    }

    private bool IsValid(Ability entity)
    {
        if (entity?.IsValid != true)
        {
            return false;
        }

        if (this.ignoreUseless && this.ignored.Contains(entity.Name))
        {
            return false;
        }

        if (this.heroesOnly && entity.Owner is not Hero)
        {
            return false;
        }

        return true;
    }
}
