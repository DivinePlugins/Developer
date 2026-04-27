//namespace Debugger.Tools.GameEvents;


//using Debugger.Menus;

//using Divine.Menu.EventArgs;
//using Divine.Menu.Items;
//using Divine.Update;

//using Logger;

//internal class UiState : IDebuggerTool
//{
//    private MenuSwitcher enabled;

//    private IMainMenu mainMenu;

//    private readonly ILog log;

//    private Menu menu;

//    public UiState(IMainMenu mainMenu, ILog log)
//    {
//        this.mainMenu = mainMenu;
//        this.log = log;
//    }

//    public int LoadPriority { get; } = 83;

//    public void Activate()
//    {
//        this.menu = this.mainMenu.GameEventsMenu.CreateMenu("UI state");

//        this.enabled = this.menu.AddSwitcher("Enabled", false);
//        this.enabled.SetTooltip("Game.OnUIStateChanged");
//        this.enabled.ValueChanged += this.EnabledOnPropertyChanged;

//        this.EnabledOnPropertyChanged(null, null);
//    }

//    public void Dispose()
//    {
//        this.enabled.ValueChanged -= this.EnabledOnPropertyChanged;
//        //GameManager.OnUIStateChanged -= this.OnUIStateChanged;
//    }

//    private void EnabledOnPropertyChanged(MenuSwitcher switcher, SwitcherChangedEventArgs e)
//    {
//        UpdateManager.BeginInvoke(() =>
//        {
//            if (this.enabled)
//            {
//                this.menu.AddAsterisk();
//                //GameManager.OnUIStateChanged += this.OnUIStateChanged;
//            }
//            else
//            {
//                this.menu.RemoveAsterisk();
//                //GameManager.OnUIStateChanged -= this.OnUIStateChanged;
//            }
//        });
//    }

//    /*private void OnUIStateChanged(UIStateChangedEventArgs args)
//    {
//        var item = new LogItem(LogType.GameEvent, Color.Yellow, "UI state changed");

//        item.AddLine("Name: " + args.UIState, args.UIState);

//        this.log.Display(item);
//    }*/
//}