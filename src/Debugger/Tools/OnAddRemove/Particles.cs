namespace Debugger.Tools.OnAddRemove;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Debugger.Menus;
using Debugger.Metadata;

using Divine.Menu;
using Divine.Menu.EventArgs;
using Divine.Menu.Items;
using Divine.Particle;
using Divine.Particle.EventArgs;
using Divine.Particle.Particles;

using Logger;

[Priority(98)]
internal sealed class Particles : IDebuggerTool
{
    private readonly HashSet<string> ignored =
    [
        "ui_mouseactions",
        "generic_hit_blood",
        "base_attacks",
        "generic_gameplay",
        "ensage_ui"
    ];

    private readonly MenuSwitcher addEnabled;

    private readonly MenuSwitcher ignoreUseless;

    private readonly MenuSwitcher ignoreZeroCp;

    private readonly ILog log;

    private readonly Menu menu;

    private readonly MenuSwitcher showCpValues;

    public Particles(IMainMenu mainMenu, ILog log)
    {
        this.log = log;

        this.menu = mainMenu.OnAddRemoveMenu.AddMenu("Particles", true, false);
        this.addEnabled = this.menu.AddSwitcher("On add enabled", false).SetTooltip("Entity.OnParticleEffectAdded");
        this.showCpValues = this.menu.AddSwitcher("Show CP values", false);
        this.ignoreZeroCp = this.menu.AddSwitcher("Ignore zero CP values", true);
        this.ignoreUseless = this.menu.AddSwitcher("Ignore useless", true);
    }

    public void Activate()
    {
        menu.ValueChanged += this.OnMenuOnValueChanged;
    }

    public void Dispose()
    {
        menu.ValueChanged -= this.OnMenuOnValueChanged;
    }

    private void OnMenuOnValueChanged(Menu sender, SwitcherChangedEventArgs e)
    {
        if (e.Value)
        {
            this.addEnabled.ValueChanged += this.AddEnabledPropertyChanged;
        }
        else
        {
            this.addEnabled.ValueChanged -= this.AddEnabledPropertyChanged;
        }
    }

    private void AddEnabledPropertyChanged(MenuSwitcher switcher, SwitcherChangedEventArgs e)
    {
        if (e.Value)
        {
            ParticleManager.ParticleAdded += this.EntityOnParticleEffectAdded;
            //Entity.OnParticleEffectReleased += this.EntityOnParticleEffectReleased;
        }
        else
        {
            ParticleManager.ParticleAdded -= this.EntityOnParticleEffectAdded;
            //Entity.OnParticleEffectReleased -= this.EntityOnParticleEffectReleased;
        }
    }

    private async void EntityOnParticleEffectAdded(ParticleAddedEventArgs e)
    {
        if (e.IsCollection)
        {
            return;
        }

        var particle = e.Particle;

        if (!this.IsValid(particle))
        {
            return;
        }

        var item = new LogItem(LogType.Particle, Color.LightGreen, "Particle added");

        item.AddLine("Name: " + particle.Name, particle.Name);

        var owner = particle.Owner;
        if (owner is not null)
        {
            item.AddLine("Owner name: " + owner.Name, owner.Name);
            item.AddLine("Owner classId: " + owner.ClassId, owner.ClassId);
            item.AddLine("Owner index: " + owner.Index, owner.Index);
        }

        await Task.Delay(1);

        if (this.IsValid(particle))
        {
            item.AddLine("Highest control point: " + particle.HighestControlPoint, particle.HighestControlPoint);

            if (this.showCpValues)
            {
                for (var i = 0; i <= particle.HighestControlPoint; i++)
                {
                    var point = particle.GetControlPoint(i);
                    if (this.ignoreZeroCp && point == Vector3.Zero)
                    {
                        continue;
                    }

                    item.AddLine("CP " + i + ": " + point, point);
                }
            }
        }

        this.log.Display(item);
    }

    /*private void EntityOnParticleEffectReleased(Entity sender, ParticleEffectReleasedEventArgs args)
    {
        var particle = args.ParticleEffect;

        if (!this.IsValid(sender, particle, particle.Name))
        {
            return;
        }

        var item = new LogItem(LogType.Particle, Color.LightGreen, "Particle added/released");

        item.AddLine("Name: " + particle.Name, particle.Name);
        item.AddLine("Highest control point: " + particle.HighestControlPoint, particle.HighestControlPoint);

        if (this.showCpValues)
        {
            for (var i = 0u; i <= args.ParticleEffect.HighestControlPoint; i++)
            {
                var point = args.ParticleEffect.GetControlPoint(i);
                if (this.ignoreZeroCp && point.IsZero)
                {
                    continue;
                }

                item.AddLine("CP " + i + ": " + point, point);
            }
        }

        this.log.Display(item);
    }*/

    private bool IsValid(Particle particle)
    {
        /*if (sender?.IsValid != true)
        {
            return false;
        }*/

        if (particle?.IsValid != true)
        {
            return false;
        }

        if (this.ignoreUseless && this.ignored.Any(particle.Name.Contains))
        {
            return false;
        }

        return true;
    }
}
