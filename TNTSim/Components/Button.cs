namespace TNTSim.Components;

internal sealed class Button : Component
{
	private readonly string text;
	private readonly Action? onClick;
	private readonly int textOffsetX;

	public Button(string text, int x, int y, int w, Action? onClick = null) : base(x, y, w, CONTROL_HEIGHT)
	{
		this.text = text;
		this.onClick = onClick;
#if DEBUG
		this.onClick ??= () => TraceLog(TraceLogLevel.LOG_INFO, $"Button {text} pressed");
#endif
		textOffsetX = (w - MeasureText(text, FONT_SIZE)) / 2;
	}

	public override void UpdateAndDraw()
	{
		DrawOutline(true);
		if (IsMouseOverSelf() && IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
		{
			onClick?.Invoke();
		}
		DrawText(text, X + textOffsetX, Y + 4, FONT_SIZE, PrimaryColor);
	}

	public static int GetMinWidth(string text) => MeasureText(text, FONT_SIZE) + (2 * PADDING);
}
