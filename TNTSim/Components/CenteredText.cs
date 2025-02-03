namespace TNTSim.Components;

internal sealed class CenteredText : Component
{
	private readonly string text;
	private readonly int textX;
	private readonly int textY;

	public CenteredText(string text, int x, int y, int w, int h) : base(x, y, w, h)
	{
		this.text = text;
		textX = (w - MeasureText(text, FONT_SIZE)) / 2;
		textY = (h - FONT_SIZE) / 2;
	}

	public override void UpdateAndDraw()
	{
		DrawText(text, X + textX, Y + textY, FONT_SIZE, PrimaryColor);
	}
}
