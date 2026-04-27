namespace Debugger.Tools.OnExecuteOrder;

using System.Collections.Generic;

using Debugger.Menus;
using Debugger.Metadata;

using Divine.Menu;
using Divine.Menu.EventArgs;
using Divine.Menu.Items;
using Divine.Numerics;
using Divine.Order;
using Divine.Order.EventArgs;
using Divine.Order.Orders.Components;

using Logger;

[Priority(87)]
internal sealed class AttackMove(IMainMenu mainMenu, ILog log) : IDebuggerTool
{
    private readonly HashSet<OrderType> orders =
    [
        OrderType.AttackPosition,
        OrderType.AttackTarget,
        OrderType.MovePosition,
        OrderType.MoveTarget,
        OrderType.Stop,
        OrderType.Hold,
        OrderType.Continue,
        OrderType.Patrol
    ];

    private readonly MenuSwitcher enabled = mainMenu.OnExecuteOrderMenu.AddSwitcher("Attack/move", false).SetTooltip("Player.OnExecuteOrder");

    public void Activate()
    {
        this.enabled.ValueChanged += this.EnabledOnPropertyChanged;
    }

    public void Dispose()
    {
        this.enabled.ValueChanged -= this.EnabledOnPropertyChanged;
    }

    private void EnabledOnPropertyChanged(MenuSwitcher switcher, SwitcherChangedEventArgs e)
    {
        if (e.Value)
        {
            OrderManager.OrderAdding += this.PlayerOnExecuteOrder;
        }
        else
        {
            OrderManager.OrderAdding -= this.PlayerOnExecuteOrder;
        }
    }

    private bool IsValid(OrderAddingEventArgs e)
    {
        if (!this.orders.Contains(e.Order.Type))
        {
            return false;
        }

        return true;
    }

    private void PlayerOnExecuteOrder(OrderAddingEventArgs e)
    {
        if (!this.IsValid(e))
        {
            return;
        }

        var item = new LogItem(LogType.ExecuteOrder, Color.Magenta, "Execute attack/move order");

        var order = e.Order;

        item.AddLine("Order: " + order.Type, order.Type);

        if (order.Target != null)
        {
            item.AddLine("Target name: " + order.Target.Name, order.Target.Name);
            item.AddLine("Target network name: " + order.Target.NetworkName, order.Target.NetworkName);
            item.AddLine("Target classID: " + order.Target.ClassId, order.Target.ClassId);
        }

        if (!order.Position.IsDefault)
        {
            item.AddLine("Position: " + order.Position, order.Position);
        }

        log.Display(item);
    }
}
