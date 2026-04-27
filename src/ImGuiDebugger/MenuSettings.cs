namespace ImGuiDebugger;

using Divine.Common.Service;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Menu;
using Divine.Menu.Items;
using Divine.SourceGenerator;

[Export]
internal sealed class MenuSettings : IService
{
    public Menu RootMenu { get; }

    public MenuToggleKey ImGuiDebuggerSwitcher { get; }

    public MenuSettings()
    {
        RootMenu = MenuManager.AddMenu(("ImGuiTest.RootMenu", "ImGui Debugger")).SetImage(AbilityId.rattletrap_overclocking);
        ImGuiDebuggerSwitcher = RootMenu.AddToggleKey(("ImGuiTest.ImGuiSwitcher", "On/Off"));
    }
}