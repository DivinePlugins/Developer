namespace Debugger.Tools.Information;

using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using Debugger.Menus;
using Debugger.Metadata;

using Divine.Menu.EventArgs;
using Divine.Menu.Items;
using Divine.Numerics;
using Divine.Update;

using Logger;

[Priority(1)]
internal sealed partial class Exceptions : IDebuggerTool
{
    private readonly TextWriter defaultOutput = Console.Out;

    private readonly ILog log;

    private readonly MenuSwitcher menu;

    private StringWriter output;

    public Exceptions(IMainMenu mainMenu, ILog log)
    {
        this.log = log;

        this.menu = mainMenu.InformationMenu.AddSwitcher("Exceptions", false);

        this.menu.ValueChanged += this.EnabledOnPropertyChanged;
    }

    [GeneratedRegex(@"(.*?exception)", RegexOptions.IgnoreCase, "ru-RU")]
    private static partial Regex ExceptionRegex { get; }

    public void Dispose()
    {
        this.menu.ValueChanged -= this.EnabledOnPropertyChanged;
    }

    private void Check()
    {
        var text = this.output.ToString();
        if (text.Length == 0)
        {
            return;
        }

        try
        {
            var match = ExceptionRegex.Match(text);
            if (match.Success)
            {
                var item = new LogItem(LogType.Exception, Color.Red, "Exception");
                item.AddLine(match.Value.Split(' ').Last());
                this.log.Display(item);

                Console.ForegroundColor = ConsoleColor.Red;
            }
        }
        finally
        {
            Console.SetOut(this.defaultOutput);
            Console.Write(text);
            Console.ResetColor();
            this.output.Dispose();
            Console.SetOut(this.output = new StringWriter());
        }
    }

    private void EnabledOnPropertyChanged(MenuSwitcher switcher, SwitcherChangedEventArgs e)
    {
        if (e.Value)
        {
            UpdateManager.CreateUpdate(200, true, this.Check);
            this.output?.Dispose();
            Console.SetOut(this.output = new StringWriter());
        }
        else
        {
            UpdateManager.DestroyUpdate(this.Check);
            Console.SetOut(this.defaultOutput);
        }
    }
}
