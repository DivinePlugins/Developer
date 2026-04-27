namespace Debugger.Logger;

using System;
using System.Collections.Generic;

using Divine.Game;
using Divine.Numerics;

internal class LogItem
{
    public LogItem(LogType type, Color color, string firstLine = null)
    {
        if (firstLine != null)
        {
            this.FirstLine = firstLine + " // " + GameManager.RawGameTime + " (" + TimeSpan.FromSeconds(GameManager.GameTime).ToString(@"mm\:ss")
                             + ")";
        }

        this.Type = type;
        this.Color = color;
        this.Time = GameManager.RawGameTime;
    }

    public Color Color { get; }

    public string FirstLine { get; }

    public List<Tuple<string, string>> Lines { get; } = [];

    public float Time { get; }

    public LogType Type { get; }

    public void AddLine(string text, object param = null)
    {
        this.Lines.Add(Tuple.Create(text, param.ToCopyFormat()));
    }
}
