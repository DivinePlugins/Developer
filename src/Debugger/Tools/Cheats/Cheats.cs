namespace Debugger.Tools.Cheats;

using Debugger.Menus;
using Debugger.Metadata;

using Divine.GameConsole;
using Divine.Input;
using Divine.Menu.EventArgs;
using Divine.Menu.Items;
using Divine.Update;

[Priority(5)]
internal sealed class Cheats : IDebuggerTool
{
    private bool allVisionEnabled;

    private readonly MenuHoldKey bot25Lvl;

    private readonly MenuHoldKey creeps;

    private bool creepsEnabled;

    private readonly MenuHoldKey hero25Lvl;

    private readonly MenuHoldKey heroGold;

    private readonly MenuHoldKey refresh;

    private readonly MenuHoldKey vision;

    private readonly MenuHoldKey wtf;

    private bool wtfEnabled;

    public Cheats(IMainMenu mainMenu)
    {
        var menu = mainMenu.CheatsMenu;
        this.refresh = menu.AddHoldKey("Refresh", Key.NumPad3);
        this.wtf = menu.AddHoldKey("Change wtf", Key.Divide);
        this.vision = menu.AddHoldKey("Change vision", Key.Multiply);
        this.creeps = menu.AddHoldKey("Change creeps spawn", Key.NumPad0);
        this.hero25Lvl = menu.AddHoldKey("Hero 25lvl", Key.NumPad1);
        this.heroGold = menu.AddHoldKey("Hero gold", Key.NumPad1);
        this.bot25Lvl = menu.AddHoldKey("Bot 25lvl", Key.NumPad2);
    }

    public void Activate()
    {
        this.refresh.ValueChanged += this.RefreshOnPropertyChanged;
        this.wtf.ValueChanged += this.WtfOnPropertyChanged;

        this.vision.ValueChanged += this.VisionOnPropertyChanged;
        this.allVisionEnabled = GameConsoleManager.GetInt32("dota_all_vision") == 1;

        this.creeps.ValueChanged += this.CreepsOnPropertyChanged;
        this.hero25Lvl.ValueChanged += this.Hero25LvlOnPropertyChanged;
        this.heroGold.ValueChanged += this.HeroGoldOnPropertyChanged;
        this.bot25Lvl.ValueChanged += this.Bot25LvlOnPropertyChanged;
    }

    public void Deactivate()
    {
        this.refresh.ValueChanged -= this.RefreshOnPropertyChanged;
        this.wtf.ValueChanged -= this.WtfOnPropertyChanged;
        this.vision.ValueChanged -= this.VisionOnPropertyChanged;
        this.creeps.ValueChanged -= this.CreepsOnPropertyChanged;
        this.hero25Lvl.ValueChanged -= this.Hero25LvlOnPropertyChanged;
        this.heroGold.ValueChanged -= this.HeroGoldOnPropertyChanged;
        this.bot25Lvl.ValueChanged -= this.Bot25LvlOnPropertyChanged;
    }

    private void Bot25LvlOnPropertyChanged(MenuHoldKey holdKey, HoldKeyChangedEventArgs e)
    {
        UpdateManager.BeginInvoke(() =>
        {
            if (this.bot25Lvl)
            {
                GameConsoleManager.ExecuteCommand("dota_bot_give_level 25");
            }
        });
    }

    private void CreepsOnPropertyChanged(MenuHoldKey holdKey, HoldKeyChangedEventArgs e)
    {
        UpdateManager.BeginInvoke(() =>
        {
            if (this.creeps)
            {
                if (this.creepsEnabled)
                {
                    GameConsoleManager.ExecuteCommand("dota_creeps_no_spawning_disable");
                    this.creepsEnabled = false;
                }
                else
                {
                    GameConsoleManager.ExecuteCommand("dota_creeps_no_spawning_enable");
                    this.creepsEnabled = true;
                }
            }
        });
    }

    private void Hero25LvlOnPropertyChanged(MenuHoldKey holdKey, HoldKeyChangedEventArgs e)
    {
        UpdateManager.BeginInvoke(() =>
        {
            if (this.hero25Lvl)
            {
                GameConsoleManager.ExecuteCommand("dota_hero_level 25");
            }
        });
    }

    private void HeroGoldOnPropertyChanged(MenuHoldKey holdKey, HoldKeyChangedEventArgs e)
    {
        UpdateManager.BeginInvoke(() =>
        {
            if (this.heroGold)
            {
                GameConsoleManager.ExecuteCommand("dota_give_gold 99999");
            }
        });
    }

    private void RefreshOnPropertyChanged(MenuHoldKey holdKey, HoldKeyChangedEventArgs e)
    {
        UpdateManager.BeginInvoke(() =>
        {
            if (this.refresh)
            {
                GameConsoleManager.ExecuteCommand("dota_hero_refresh");
            }
        });
    }

    private void VisionOnPropertyChanged(MenuHoldKey holdKey, HoldKeyChangedEventArgs e)
    {
        UpdateManager.BeginInvoke(() =>
        {
            if (this.vision)
            {
                if (this.allVisionEnabled)
                {
                    GameConsoleManager.ExecuteCommand("dota_all_vision_disable");
                    this.allVisionEnabled = false;
                }
                else
                {
                    GameConsoleManager.ExecuteCommand("dota_all_vision_enable");
                    this.allVisionEnabled = true;
                }
            }
        });
    }

    private void WtfOnPropertyChanged(MenuHoldKey holdKey, HoldKeyChangedEventArgs e)
    {
        UpdateManager.BeginInvoke(() =>
        {
            if (this.wtf)
            {
                if (this.wtfEnabled)
                {
                    GameConsoleManager.ExecuteCommand("dota_ability_debug_disable");
                    this.wtfEnabled = false;
                }
                else
                {
                    GameConsoleManager.ExecuteCommand("dota_ability_debug_enable");
                    this.wtfEnabled = true;
                }
            }
        });
    }
}
