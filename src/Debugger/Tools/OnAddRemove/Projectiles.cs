namespace Debugger.Tools.OnAddRemove;

using Debugger.Menus;
using Debugger.Metadata;

using Divine.Entity.Entities.Units.Heroes;
using Divine.Menu;
using Divine.Menu.EventArgs;
using Divine.Menu.Items;
using Divine.Numerics;
using Divine.Projectile;
using Divine.Projectile.EventArgs;
using Divine.Projectile.Projectiles;

using Logger;

[Priority(96)]
internal sealed class Projectiles : IDebuggerTool
{
    private readonly MenuSwitcher addEnabled;

    private readonly MenuSwitcher heroesOnly;

    private readonly ILog log;

    private readonly Menu menu;

    private readonly MenuSwitcher removeEnabled;

    public Projectiles(IMainMenu mainMenu, ILog log)
    {
        this.log = log;

        this.menu = mainMenu.OnAddRemoveMenu.AddMenu("Projectiles", true, false);
        this.addEnabled = this.menu.AddSwitcher("On add enabled", false).SetTooltip("ObjectManager.OnAddTrackingProjectile");
        this.removeEnabled = this.menu.AddSwitcher("On remove enabled", false).SetTooltip("ObjectManager.OnRemoveTrackingProjectile");
        this.heroesOnly = this.menu.AddSwitcher("Heroes only", false);
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
            ProjectileManager.TrackingProjectileAdded += this.OnTrackingProjectileAdded;
        }
        else
        {
            ProjectileManager.TrackingProjectileAdded -= this.OnTrackingProjectileAdded;
        }
    }

    private void RemoveEnabledOnPropertyChanged(MenuSwitcher switcher, SwitcherChangedEventArgs e)
    {
        if (e.Value)
        {
            ProjectileManager.TrackingProjectileRemoved += this.OnRemoveTrackingProjectile;
        }
        else
        {
            ProjectileManager.TrackingProjectileRemoved -= this.OnRemoveTrackingProjectile;
        }
    }

    private bool IsValid(TrackingProjectile projectile)
    {
        if (this.heroesOnly && projectile.Source is not Hero)
        {
            return false;
        }

        return true;
    }

    private void OnRemoveTrackingProjectile(TrackingProjectileRemovedEventArgs e)
    {
        var projectile = e.Projectile;

        if (!this.IsValid(projectile))
        {
            return;
        }

        var item = new LogItem(LogType.Projectile, Color.LightPink, "Projectile removed");

        item.AddLine("Source name: " + projectile.Source?.Name, projectile.Source?.Name);
        item.AddLine("Source network name: " + projectile.Source?.NetworkName, projectile.Source?.NetworkName);
        item.AddLine("Source classID: " + projectile.Source?.ClassId, projectile.Source?.ClassId);
        item.AddLine("Speed: " + projectile.Speed, projectile.Speed);
        item.AddLine("Position: " + projectile.Position, projectile.Position);
        item.AddLine("Target name: " + projectile.Target?.Name, projectile.Target?.Name);
        item.AddLine("Target network name: " + projectile.Target?.NetworkName, projectile.Target?.NetworkName);
        item.AddLine("Target classID: " + projectile.Target?.ClassId, projectile.Target?.ClassId);
        item.AddLine("Target position: " + projectile.TargetPosition, projectile.TargetPosition);

        this.log.Display(item);
    }

    private void OnTrackingProjectileAdded(TrackingProjectileAddedEventArgs e)
    {
        var projectile = e.Projectile;

        if (!this.IsValid(projectile))
        {
            return;
        }

        var item = new LogItem(LogType.Projectile, Color.LightGreen, "Projectile added");

        item.AddLine("Source name: " + projectile.Source?.Name, projectile.Source?.Name);
        item.AddLine("Source network name: " + projectile.Source?.NetworkName, projectile.Source?.NetworkName);
        item.AddLine("Source classID: " + projectile.Source?.ClassId, projectile.Source?.ClassId);
        item.AddLine("Speed: " + projectile.Speed, projectile.Speed);
        item.AddLine("Position: " + projectile.Position, projectile.Position);
        item.AddLine("Target name: " + projectile.Target?.Name, projectile.Target?.Name);
        item.AddLine("Target network name: " + projectile.Target?.NetworkName, projectile.Target?.NetworkName);
        item.AddLine("Target classID: " + projectile.Target?.ClassId, projectile.Target?.ClassId);
        item.AddLine("Target position: " + projectile.TargetPosition, projectile.TargetPosition);

        this.log.Display(item);
    }
}
