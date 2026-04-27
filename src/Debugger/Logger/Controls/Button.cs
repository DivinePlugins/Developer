namespace Debugger.Logger.Controls;

using Divine.Extensions;
using Divine.Game;
using Divine.Numerics;
using Divine.Renderer;

internal class Button
{
    private readonly Vector2 buttonSize;

    private readonly string name;

    private Vector2 position;

    private Vector2 textMeasuredSize;

    private Vector2 textPosition;

    private float textSize;

    public Button(string name, Vector2 position, Vector2 buttonSize)
    {
        this.name = name;
        this.position = position;
        this.buttonSize = buttonSize;
    }

    public virtual void Draw()
    {
        var backgroundColor = new Color(50, 50, 50, 200);
        RendererManager.DrawFilledRectangle(new Rect(this.position.X, this.position.Y, this.buttonSize.X, this.buttonSize.Y), this.IsMouseUnderButton() ? backgroundColor.SetAlpha(255) : backgroundColor);
        RendererManager.DrawText(this.name, this.textPosition, Color.DeepSkyBlue, "Arial", this.textSize);
    }

    public bool IsMouseUnderButton()
    {
        return GameManager.MouseScreenPosition.IsUnderRectangle(this.position.X, this.position.Y, this.buttonSize.X, this.buttonSize.Y);
    }

    public virtual void UpdateSize(float size)
    {
        this.textMeasuredSize = RendererManager.MeasureText(this.name, "Arial", size + 1);
        this.textSize = size + 1;
        this.textPosition = new Vector2(
            this.position.X + ((this.buttonSize.X / 2) - (this.textMeasuredSize.X / 2)),
            this.position.Y + ((this.buttonSize.Y / 2) - (this.textMeasuredSize.Y / 2)));
    }

    public virtual void UpdateYPosition(float y)
    {
        this.position = new Vector2(this.position.X, y);
        this.UpdateSize(this.textSize - 1);
    }
}