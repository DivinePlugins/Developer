namespace Debugger.Logger;

using System;
using System.Collections.Generic;
using System.Linq;

using Controls;

using Debugger.Menus;

using Divine.Extensions;
using Divine.Game;
using Divine.Helpers;
using Divine.Input;
using Divine.Input.EventArgs;
using Divine.Menu;
using Divine.Menu.EventArgs;
using Divine.Menu.Items;
using Divine.Numerics;
using Divine.Renderer;
using Divine.Update;

using TextCopy;

using Button = Controls.Button;

internal sealed class Log : ILog
{
    private const uint WM_LBUTTONDOWN = 0x0201;

    private const uint WM_MOUSEWHEEL = 0x020A;

    private const uint WM_RBUTTONDOWN = 0x0204;

    private readonly List<LogItem> displayList = [];

    private readonly List<LogItem> items = [];

    private Button clearButton;

    private readonly MenuSlider itemsToSave;

    private Button jumpTopButton;

    private readonly MenuSlider linesToShow;

    private readonly IMainMenu mainMenu;

    private ToggleButton overlayButton;

    private ToggleButton pauseButton;

    private readonly MenuSlider positionX;

    private readonly MenuSlider positionY;

    private readonly MenuSwitcher hideSwitcher;

    private readonly float screenSizeX;

    private int scrollPosition;

    private readonly MenuSlider textSize;

    public Log(IMainMenu mainMenu)
    {
        this.mainMenu = mainMenu;

        var menu = this.mainMenu.OverlaySettingsMenu;

        this.itemsToSave = menu.AddSlider("Items to save", 100, 5, 500);
        this.linesToShow = menu.AddSlider("Lines to show", 30, 10, 100);
        this.textSize = menu.AddSlider("Text size", 20, 10, 30);
        this.screenSizeX = HUDInfo.ScreenSize.X;
        this.positionX = menu.AddSlider("Position x", (int)(this.screenSizeX * 0.75), 0, (int)this.screenSizeX);
        this.positionY = menu.AddSlider("Position y", 100, 0, (int)HUDInfo.ScreenSize.Y);
        this.hideSwitcher = menu.AddSwitcher("Hide").Hide();

        mainMenu.RootMenu.ValueChanged += OnRootValueChanged;
    }

    public void Dispose()
    {
        mainMenu.RootMenu.ValueChanged -= OnRootValueChanged;
    }

    private void OnRootValueChanged(Menu sender, SwitcherChangedEventArgs e)
    {
        if (e.Value)
        {
            this.linesToShow.ValueChanged += this.LinesToShowOnPropertyChanged;
            this.textSize.ValueChanged += this.TextSizeOnPropertyChanged;

            this.overlayButton = new ToggleButton("Hide", "Show", new(this.screenSizeX - 100, this.positionY - 50), new(100, 30))
            {
                Enabled = this.hideSwitcher
            };

            this.pauseButton = new ToggleButton(
                "Pause",
                "Continue",
                new Vector2(this.screenSizeX - 200, this.positionY - 50),
                new Vector2(100, 30));
            this.clearButton = new Button("Clear", new Vector2(this.screenSizeX - 300, this.positionY - 50), new Vector2(100, 30));
            this.jumpTopButton = new Button("^", new Vector2(this.screenSizeX - 400, this.positionY - 50), new Vector2(100, 30));

            this.TextSizeOnPropertyChanged(null, null);

            RendererManager.Draw += this.DrawingOnDraw;
            InputManager.WindowProc += this.GameOnWndProc;
        }
        else
        {
            this.linesToShow.ValueChanged -= this.LinesToShowOnPropertyChanged;
            this.textSize.ValueChanged -= this.TextSizeOnPropertyChanged;

            RendererManager.Draw -= this.DrawingOnDraw;
            InputManager.WindowProc -= this.GameOnWndProc;
        }
    }

    private int ScrollPosition
    {
        get => this.scrollPosition;

        set
        {
            if (value <= 0)
            {
                this.scrollPosition = 0;
            }
            else if (value >= this.items.Count)
            {
                this.scrollPosition = this.items.Count - 1;
            }
            else
            {
                this.scrollPosition = value;
            }
        }
    }

    public void Display(LogItem newItem)
    {
        if (!this.pauseButton.Enabled)
        {
            return;
        }

        if (this.items.Count > this.itemsToSave)
        {
            this.items.RemoveAt(0);
        }

        this.items.Add(newItem);

        if (this.ScrollPosition > 0)
        {
            this.ScrollPosition++;
        }

        this.UpdateOverlay();
    }

    public bool IsMouseUnderLog()
    {
        if (!this.overlayButton.Enabled)
        {
            return false;
        }

        var screenSize = RendererManager.ScreenSize;
        return GameManager.MouseScreenPosition.IsUnderRectangle(this.positionX - 20, this.positionY - 20, screenSize.X, screenSize.Y);
    }

    private void DrawingOnDraw()
    {
        this.overlayButton.Draw();
        if (!this.overlayButton.Enabled)
        {
            return;
        }

        this.pauseButton.Draw();
        this.clearButton.Draw();

        if (this.scrollPosition > 0)
        {
            this.jumpTopButton.Draw();
        }

        var backgroundColor = new Color(50, 50, 50, 200);
        var totalSize = this.displayList.Sum(x => x.Lines.Count + 2);
        if (totalSize > 0)
        {
            RendererManager.DrawFilledRectangle(
                new Rect(this.positionX - 20, this.positionY - 20, 2000, (totalSize * this.textSize) + 20),
                backgroundColor);
        }

        var selectedLine = (int)((GameManager.MouseScreenPosition.Y - this.positionY) / this.textSize);
        var offset = 0;

        foreach (var item in this.displayList)
        {
            if (!string.IsNullOrEmpty(item.FirstLine))
            {
                RendererManager.DrawText(
                    item.FirstLine,
                    new Vector2(this.positionX, this.positionY + (offset * this.textSize)),
                    item.Color,
                    "Arial",
                    this.textSize);
            }

            var startColor = item.Color;
            var endColor = Color.White;
            var time = Math.Min(GameManager.RawGameTime - item.Time, 1);

            var color = new Color(
                (int)(((endColor.R - startColor.R) * time) + startColor.R),
                (int)(((endColor.G - startColor.G) * time) + startColor.G),
                (int)(((endColor.B - startColor.B) * time) + startColor.B));

            foreach (var itemLine in item.Lines)
            {
                offset++;
                RendererManager.DrawText(
                    itemLine.Item1,
                    new Vector2(this.positionX, this.positionY + (offset * this.textSize)),
                    selectedLine == offset && this.IsMouseUnderLog() ? Color.Orange : color,
                    "Arial",
                    this.textSize);
            }

            offset += 2;
        }
    }

    private void GameOnWndProc(WindowProcEventArgs e)
    {
        if (e.Msg == WM_MOUSEWHEEL && this.IsMouseUnderLog())
        {
            var delta = (short)((e.WParam >> 16) & 0xFFFF);
            if (delta > 0)
            {
                this.ScrollPosition--;
            }
            else
            {
                this.ScrollPosition++;
            }

            this.UpdateOverlay();
            e.Process = false;
            return;
        }

        if (e.Msg == WM_LBUTTONDOWN)
        {
            if (this.overlayButton.IsMouseUnderButton())
            {
                this.overlayButton.Enabled = !this.overlayButton.Enabled;
                this.hideSwitcher.Value = this.overlayButton.Enabled;
                e.Process = false;
                return;
            }

            if (this.jumpTopButton.IsMouseUnderButton())
            {
                this.scrollPosition = 0;
                this.UpdateOverlay();
                e.Process = false;
                return;
            }

            if (this.clearButton.IsMouseUnderButton())
            {
                this.scrollPosition = 0;
                this.items.Clear();
                this.UpdateOverlay();
                e.Process = false;
                return;
            }

            if (this.pauseButton.IsMouseUnderButton())
            {
                this.pauseButton.Enabled = !this.pauseButton.Enabled;
                e.Process = false;
                return;
            }

            if (this.IsMouseUnderLog())
            {
                var line = (int)((GameManager.MouseScreenPosition.Y - this.positionY) / this.textSize);
                var offset = 0;

                foreach (var item in this.displayList)
                {
                    foreach (var itemLine in item.Lines)
                    {
                        if (++offset == line)
                        {
                            if (!string.IsNullOrEmpty(itemLine.Item2))
                            {
                                ClipboardService.SetText(itemLine.Item2);
                            }

                            e.Process = false;
                            return;
                        }
                    }

                    offset += 2;
                }
            }
        }
    }

    private void LinesToShowOnPropertyChanged(MenuSlider slider, SliderChangedEventArgs e)
    {
        UpdateManager.BeginInvoke(() =>
        {
            this.UpdateOverlay();
        });
    }

    private void PositionYOnPropertyChanged(MenuSlider slider, SliderChangedEventArgs e)
    {
        UpdateManager.BeginInvoke(() =>
        {
            this.overlayButton.UpdateYPosition(this.positionY - 50);
            this.jumpTopButton.UpdateYPosition(this.positionY - 50);
            this.clearButton.UpdateYPosition(this.positionY - 50);
            this.pauseButton.UpdateYPosition(this.positionY - 50);
        });
    }

    private void TextSizeOnPropertyChanged(MenuSlider slider, SliderChangedEventArgs e)
    {
        UpdateManager.BeginInvoke(() =>
        {
            this.overlayButton.UpdateSize(this.textSize);
            this.jumpTopButton.UpdateSize(this.textSize);
            this.clearButton.UpdateSize(this.textSize);
            this.pauseButton.UpdateSize(this.textSize);
        });
    }

    private void UpdateOverlay()
    {
        this.displayList.Clear();
        var totalLines = 0;

        foreach (var item in this.items.Reverse<LogItem>().Skip(this.scrollPosition))
        {
            this.displayList.Add(item);
            totalLines += item.Lines.Count + 2;

            if (totalLines >= this.linesToShow)
            {
                break;
            }
        }
    }
}
