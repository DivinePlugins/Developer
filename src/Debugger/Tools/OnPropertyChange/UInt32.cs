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

[Priority(91)]
internal sealed class UInt32 : IDebuggerTool
{
    private readonly HashSet<string> ignored =
    [
        "m_iFoWFrameNumber",
        "m_iNetTimeOfDay",
        "m_iNetWorth",
        "m_iIncomeGold",
        "m_iTotalEarnedGold",
        "m_iUnreliableGold",
        "m_iReliableGold",
        "m_nHealthBarOffsetOverride",
        "m_nServerOrderSequenceNumber",
        "m_nResetEventsParity",
        "m_cellY",
        "m_cellX",
        "m_cellZ",
        "m_nGridY",
        "m_nGridX",
        "m_nGridZ",
        "m_nameStringableIndex",
        "m_iUnitNameIndex",
        "m_nIdealMotionType",
        "m_nDebugIndex",
        "m_NetworkSequenceIndex",
        "m_nNewSequenceParity",
        "m_iMusicOperatorVals",
        "m_anglediff",
        "m_iTaggedAsVisibleByTeam",
        "iStockCount"
    ];

    private readonly HashSet<string> semiIgnored =
    [
        "m_iHealth",
        "m_iMaxHealth",
        "m_NetworkActivity",
        "m_iHeroDamage",
        "m_iRecentDamage",
        "m_iDamageBonus",
        "m_iMoveSpeed",
        "m_iDayTimeVisionRange",
        "m_iNightTimeVisionRange",
        "m_iPauseTeam",
        "m_iAttackCapabilities"
    ];

    private readonly MenuSwitcher heroesOnly;

    private readonly MenuSwitcher ignoreSemiUseless;

    private readonly MenuSwitcher ignoreUseless;

    private readonly ILog log;

    private readonly Menu menu;

    public UInt32(IMainMenu mainMenu, ILog log)
    {
        this.log = log;

        this.menu = mainMenu.OnPropertyChange.AddMenu("UInt32", true, false).SetTooltip("Entity.OnUInt32PropertyChange");
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
            Entity.NetworkPropertyChanged += this.OnUInt32PropertyChange;
        }
        else
        {
            Entity.NetworkPropertyChanged -= this.OnUInt32PropertyChange;
        }
    }

    private bool IsValid(Entity sender, NetworkPropertyChangedEventArgs e)
    {
        if (e.ValueTypeName is not "uint32")
        {
            return false;
        }

        if (e.OldValue.GetUInt32() == e.NewValue.GetUInt32())
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

    private void OnUInt32PropertyChange(Entity sender, NetworkPropertyChangedEventArgs e)
    {
        if (!this.IsValid(sender, e))
        {
            return;
        }

        var item = new LogItem(LogType.UInt32, Color.Cyan, "UInt32 changed");

        item.AddLine("Property name: " + e.PropertyName, e.PropertyName);
        item.AddLine("Property values: " + e.OldValue.GetUInt32() + " => " + e.NewValue.GetUInt32(), e.NewValue.GetUInt32());
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
