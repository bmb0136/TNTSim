namespace TNTSim.Components;

internal sealed class TextComponent : Component
{
	private readonly string text;

	public TextComponent(string text, int x, int y) : base(x, y, MeasureText(text, FONT_SIZE) + PADDING + PADDING, CONTROL_HEIGHT)
	{
		this.text = text;
	}

	public override void UpdateAndDraw() => DrawText(text, X + PADDING, Y + PADDING, FONT_SIZE, PrimaryColor);
}
