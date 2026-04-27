namespace Debugger.Tools.Cheats;

using System;
using System.Linq;

using Debugger.Menus;
using Debugger.Metadata;

using Divine.Entity;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Entity.Entities.Units.Heroes.Components;
using Divine.GameConsole;
using Divine.Input;
using Divine.Menu.EventArgs;
using Divine.Menu.Items;
using Divine.Update;

[Priority(5)]
internal sealed class CreateUnits : IDebuggerTool
{
    private readonly Random random = new();

    private readonly MenuHoldKey meleeAllyCreep;

    private readonly MenuHoldKey meleeEnemyCreep;

    private readonly MenuHoldKey randomAlly;

    private readonly MenuHoldKey randomEnemy;

    private readonly MenuHoldKey rangedAllyCreep;

    private readonly MenuHoldKey rangedEnemyCreep;

    public CreateUnits(IMainMenu mainMenu)
    {
        var menu = mainMenu.CheatsMenu.AddMenu("Create unit");
        this.randomAlly = menu.AddHoldKey("Random ally hero", Key.NumPad3);
        this.meleeAllyCreep = menu.AddHoldKey("Melee ally creep", Key.NumPad8);
        this.rangedAllyCreep = menu.AddHoldKey("Ranged ally creep", Key.NumPad9);
        this.randomEnemy = menu.AddHoldKey("Random enemy hero", Key.NumPad4);
        this.meleeEnemyCreep = menu.AddHoldKey("Melee enemy creep", Key.NumPad5);
        this.rangedEnemyCreep = menu.AddHoldKey("Ranged enemy creep", Key.NumPad6);
    }

    public void Activate()
    {
        this.randomAlly.ValueChanged += this.RandomAllyOnPropertyChanged;
        this.meleeAllyCreep.ValueChanged += this.MeleeAllyCreepOnPropertyChanged;
        this.rangedAllyCreep.ValueChanged += this.RangedAllyCreepOnPropertyChanged;
        this.randomEnemy.ValueChanged += this.RandomEnemyOnPropertyChanged;
        this.meleeEnemyCreep.ValueChanged += this.MeleeEnemyCreepOnPropertyChanged;
        this.rangedEnemyCreep.ValueChanged += this.RangedEnemyCreepOnPropertyChanged;
    }

    public void Deactivate()
    {
        this.randomAlly.ValueChanged -= this.RandomAllyOnPropertyChanged;
        this.meleeAllyCreep.ValueChanged -= this.MeleeAllyCreepOnPropertyChanged;
        this.rangedAllyCreep.ValueChanged -= this.RangedAllyCreepOnPropertyChanged;
        this.randomEnemy.ValueChanged -= this.RandomEnemyOnPropertyChanged;
        this.meleeEnemyCreep.ValueChanged -= this.MeleeEnemyCreepOnPropertyChanged;
        this.rangedEnemyCreep.ValueChanged -= this.RangedEnemyCreepOnPropertyChanged;
    }

    private string GetRandomHero()
    {
        var alreadyAdded = EntityManager.GetEntities<Hero>().Select(x => x.Id);
        var heroes = Enum.GetValues<HeroId>().Cast<HeroId>().Except(alreadyAdded).ToList();
        var randomHero = heroes[this.random.Next(1, heroes.Count - 1)];

        return randomHero.ToString();
    }

    private void MeleeAllyCreepOnPropertyChanged(MenuHoldKey holdKey, HoldKeyChangedEventArgs e)
    {
        UpdateManager.BeginInvoke(() =>
        {
            if (this.meleeAllyCreep)
            {
                GameConsoleManager.ExecuteCommand("dota_create_unit npc_dota_creep_goodguys_melee");
            }
        });
    }

    private void MeleeEnemyCreepOnPropertyChanged(MenuHoldKey holdKey, HoldKeyChangedEventArgs e)
    {
        UpdateManager.BeginInvoke(() =>
        {
            if (this.meleeEnemyCreep)
            {
                GameConsoleManager.ExecuteCommand("dota_create_unit npc_dota_creep_goodguys_melee enemy");
            }
        });
    }

    private void RandomAllyOnPropertyChanged(MenuHoldKey holdKey, HoldKeyChangedEventArgs e)
    {
        UpdateManager.BeginInvoke(() =>
        {
            if (this.randomAlly)
            {
                GameConsoleManager.ExecuteCommand("dota_create_unit " + this.GetRandomHero());
            }
        });
    }

    private void RandomEnemyOnPropertyChanged(MenuHoldKey holdKey, HoldKeyChangedEventArgs e)
    {
        UpdateManager.BeginInvoke(() =>
        {
            if (this.randomEnemy)
            {
                GameConsoleManager.ExecuteCommand("dota_create_unit " + this.GetRandomHero() + " enemy");
            }
        });
    }

    private void RangedAllyCreepOnPropertyChanged(MenuHoldKey holdKey, HoldKeyChangedEventArgs e)
    {
        UpdateManager.BeginInvoke(() =>
        {
            if (this.rangedAllyCreep)
            {
                GameConsoleManager.ExecuteCommand("dota_create_unit npc_dota_creep_goodguys_ranged");
            }
        });
    }

    private void RangedEnemyCreepOnPropertyChanged(MenuHoldKey holdKey, HoldKeyChangedEventArgs e)
    {
        UpdateManager.BeginInvoke(() =>
        {
            if (this.rangedEnemyCreep)
            {
                GameConsoleManager.ExecuteCommand("dota_create_unit npc_dota_creep_goodguys_ranged enemy");
            }
        });
    }
}
