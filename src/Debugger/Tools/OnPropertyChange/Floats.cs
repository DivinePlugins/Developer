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

[Priority(93)]
internal class Floats : IDebuggerTool
{
    private readonly HashSet<string> ignored =
    [
        "m_vecX",
        "m_vecY",
        "m_vecZ",
        "m_fGameTime",
        "m_flMana",
        "m_flStartSequenceCycle",
        "m_flPlaybackRate",
        "m_flLastSpawnTime",
        "m_flMusicOperatorVals",
        "m_fStuns",
        "m_vecMins",
        "m_vecMaxs"
    ];

    private readonly HashSet<string> semiIgnored =
    [
        "m_fCooldown",
        "m_flCooldownLength",
        "m_flPurchaseTime",
        "m_flAssembledTime",
        "m_flRadarCooldowns",
        "m_flElasticity",
        "m_flScale"
    ];

    private readonly MenuSwitcher heroesOnly;

    private readonly MenuSwitcher ignoreSemiUseless;

    private readonly MenuSwitcher ignoreUseless;

    private readonly ILog log;

    private readonly Menu menu;

    public Floats(IMainMenu mainMenu, ILog log)
    {
        this.log = log;

        this.menu = mainMenu.OnPropertyChange.AddMenu("Floats", true, false).SetTooltip("Entity.OnFloatPropertyChange");
        this.heroesOnly = this.menu.AddSwitcher("Heroes only", false);
        this.ignoreSemiUseless = this.menu.AddSwitcher("Ignore semi useless", false);
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
            Entity.NetworkPropertyChanged += this.OnFloatPropertyChange;
        }
        else
        {
            Entity.NetworkPropertyChanged -= this.OnFloatPropertyChange;
        }
    }

    private bool IsValid(Entity sender, NetworkPropertyChangedEventArgs e)
    {
        if (e.ValueTypeName is not "float32" and not "GameTime_t")
        {
            return false;
        }

        if (e.OldValue.GetSingle() == e.NewValue.GetSingle())
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

    private void OnFloatPropertyChange(Entity sender, NetworkPropertyChangedEventArgs e)
    {
        if (!this.IsValid(sender, e))
        {
            return;
        }

        var item = new LogItem(LogType.Float, Color.Cyan, "Float changed");

        item.AddLine("Property name: " + e.PropertyName, e.PropertyName);
        item.AddLine("Property values: " + e.OldValue.GetSingle() + " => " + e.NewValue.GetSingle(), e.NewValue.GetSingle());
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
