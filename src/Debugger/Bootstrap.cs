namespace Debugger;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Debugger.Logger;
using Debugger.Menus;
using Debugger.Metadata;

using Divine.Menu.EventArgs;
using Divine.Service;

using Tools;

//[ExportPlugin("Debugger", StartupMode.Auto, priority: 1)]
internal class Bootstrap : Bootstrapper
{
    private readonly List<IDebuggerTool> tools = [];

    private MainMenu MainMenu;

    private Log Log;

    protected override void OnMainActivate()
    {
        MainMenu = new();
        Log = new(MainMenu);

        var mainModules = new Dictionary<Type, object>
        {
            { typeof(IMainMenu), MainMenu },
            { typeof(ILog), Log }
        };

        var types = Assembly.GetExecutingAssembly().GetTypes()
            .Where(x => x.IsClass && !x.IsAbstract && typeof(IDebuggerTool).IsAssignableFrom(x))
            .OrderByDescending(x => x.GetCustomAttribute<PriorityAttribute>()?.Value ?? 0);

        foreach (var type in types)
        {
            var constructor = type.GetConstructors()[0];

            var parameters = constructor.GetParameters();
            var objectParameters = new object[parameters.Length];

            for (var i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                objectParameters[i] = mainModules[parameter.ParameterType];
            }

            tools.Add((IDebuggerTool)Activator.CreateInstance(type, objectParameters));
        }
    }

    protected override void OnMainDeactivate()
    {
        Log.Dispose();

        foreach (var service in this.tools.AsEnumerable().Reverse())
        {
            service.Dispose();
        }
    }

    protected override void OnActivate()
    {
        MainMenu.RootMenu.ValueChanged += OnRootValueChanged;
    }

    protected override void OnDeactivate()
    {
        MainMenu.RootMenu.ValueChanged -= OnRootValueChanged;
    }

    private void OnRootValueChanged(Divine.Menu.Items.Menu sender, SwitcherChangedEventArgs e)
    {
        if (e.Value)
        {
            foreach (var service in this.tools)
            {
                service.Activate();
            }
        }
        else
        {
            foreach (var service in this.tools.AsEnumerable().Reverse())
            {
                service.Deactivate();
            }
        }
    }
}
