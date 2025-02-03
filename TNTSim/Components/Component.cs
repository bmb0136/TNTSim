namespace TNTSim.Components;

internal abstract class Component
{
	public Color PrimaryColor { get; set; } = Color.GRAY;
	public int X { get; init; }
	public int Y { get; init; }
	public int Width { get; init; }
	public int Height { get; init; }
	protected Color SeconaryColor => new()
	{
		R = (byte)Math.Min(PrimaryColor.R + 70, 255),
		G = (byte)Math.Min(PrimaryColor.G + 70, 255),
		B = (byte)Math.Min(PrimaryColor.B + 70, 255),
		A = PrimaryColor.A
	};

	public Component(int x, int y, int w, int h)
	{
		X = x;
		Y = y;
		Width = w;
		Height = h;
	}

	public abstract void UpdateAndDraw();

	protected void DrawOutline(bool addHover = false)
	{
		DrawRectangleLines(X, Y, Width, Height, PrimaryColor);
		if (addHover && IsMouseOverSelf())
		{
			DrawRectangle(X + 1, Y + 1, Width - 2, Height - 2, SeconaryColor);
		}
	}

	protected bool IsMouseOverSelf()
	{
		int mx = GetMouseX() - X;
		int my = GetMouseY() - Y;
		return mx >= 0 && mx <= Width && my >= 0 && my <= Height;
	}

	public int GetRightSide() => X + Width;
	public int GetBottomSide() => Y + Height;
}