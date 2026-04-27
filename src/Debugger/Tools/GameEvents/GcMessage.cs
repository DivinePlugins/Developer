//namespace Debugger.Tools.GameEvents;


//using Debugger.Menus;

//using Divine.Menu.EventArgs;
//using Divine.Menu.Items;
//using Divine.Update;

//using Logger;

//internal class GcMessage : IDebuggerTool
//{
//    private MenuSwitcher enabled;

//    private IMainMenu mainMenu;

//    private readonly ILog log;

//    private Menu menu;

//    public GcMessage(IMainMenu mainMenu, ILog log)
//    {
//        this.mainMenu = mainMenu;
//        this.log = log;
//    }

//    public int LoadPriority { get; } = 84;

//    public void Activate()
//    {
//        this.menu = this.mainMenu.GameEventsMenu.CreateMenu("GC message");

//        this.enabled = this.menu.AddSwitcher("Enabled", false);
//        this.enabled.SetTooltip("Game.OnGCMessageReceive");
//        this.enabled.ValueChanged += this.EnabledOnPropertyChanged;

//        this.EnabledOnPropertyChanged(null, null);
//    }

//    public void Dispose()
//    {
//        this.enabled.ValueChanged -= this.EnabledOnPropertyChanged;
//        //GameManager.OnGCMessageReceive -= this.OnGCMessageReceive;
//    }

//    private void EnabledOnPropertyChanged(MenuSwitcher switcher, SwitcherChangedEventArgs e)
//    {
//        UpdateManager.BeginInvoke(() =>
//        {
//            if (this.enabled)
//            {
//                this.menu.AddAsterisk();
//                //GameManager.OnGCMessageReceive += this.OnGCMessageReceive;
//            }
//            else
//            {
//                this.menu.RemoveAsterisk();
//                //GameManager.OnGCMessageReceive -= this.OnGCMessageReceive;
//            }
//        });
//    }

//    /*private void OnGCMessageReceive(GCMessageEventArgs args)
//    {
//        var item = new LogItem(LogType.GameEvent, Color.Yellow, "GC message received");

//        item.AddLine("Name: " + args.MessageID, args.MessageID);

//        this.log.Display(item);
//    }*/
//}