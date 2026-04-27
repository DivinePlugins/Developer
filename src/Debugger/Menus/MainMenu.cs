namespace Debugger.Menus;

using Divine.Entity.Entities.Abilities.Components;
using Divine.Menu;
using Divine.Menu.Items;
using Divine.Numerics;
using Divine.Renderer;

internal class MainMenu : IMainMenu
{
    public MainMenu()
    {
        this.RootMenu = MenuManager.DeveloperMenu.AddMenu("Debugger", true, false)
            .SetImage((AbilityId.chaos_knight_reality_rift.ToString(), ImageType.Ability))
            .SetDisplayTextFontColor(Color.PaleVioletRed);

        this.OnAddRemoveMenu = this.RootMenu.AddMenu("On add/remove");
        this.OnChangeMenu = this.RootMenu.AddMenu("On change");
        this.OnPropertyChange = this.RootMenu.AddMenu("On property change");
        this.InformationMenu = this.RootMenu.AddMenu("Information");
        this.CheatsMenu = this.RootMenu.AddMenu("Cheats");
        this.OnExecuteOrderMenu = this.RootMenu.AddMenu("On execute order");
        this.GameEventsMenu = this.RootMenu.AddMenu("Game events");
        this.OverlaySettingsMenu = this.RootMenu.AddMenu("Overlay settings");
    }

    public Menu RootMenu { get; private set; }

    public Menu CheatsMenu { get; private set; }

    public Menu GameEventsMenu { get; private set; }

    public Menu InformationMenu { get; private set; }

    public Menu OnAddRemoveMenu { get; private set; }

    public Menu OnChangeMenu { get; private set; }

    public Menu OnPropertyChange { get; private set; }

    public Menu OnExecuteOrderMenu { get; private set; }

    public Menu OverlaySettingsMenu { get; private set; }
}
