namespace TNTSim.Components;

internal sealed class NumberBox : Component
{
	public int Value
	{
		get => value;
		set
		{
			int old = this.value;
			this.value = Math.Clamp(value, min, max);
			if (old != this.value)
			{
				valueText = this.value.ToString();
			}
		}
	}

	private readonly ScrollHandler scroller = new(1f);
	private readonly DragHandler dragger = new();
	private readonly int min;
	private readonly int max;
	private int value;
	private string valueText;
	private int valueOnDragStart;

	public NumberBox(int min, int max, int x, int y, int w, int? initial = null) : base(x, y, w, CONTROL_HEIGHT)
	{
		this.min = min;
		this.max = max;
		value = initial ?? min;
		valueText = value.ToString();
	}

	public override void UpdateAndDraw()
	{
		dragger.Update(IsMouseOverSelf);

		if (dragger.IsDragging)
		{
			int delta = -dragger.DeltaY / 2;
			Value = valueOnDragStart + delta;
		}
		else
		{
			valueOnDragStart = value;
		}

		scroller.Update(IsMouseOverSelf);
		if (!dragger.IsDragging && scroller.HasScrolledOneTick(out int scrollSign))
		{
			Value += scrollSign;
		}

		if (IsMouseOverSelf() && IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_RIGHT))
		{
			Value = min;
		}

		if (IsMouseOverSelf() && !dragger.IsDragging)
		{
			if (IsKeyPressed(KeyboardKey.KEY_MINUS))
			{
				Value *= -1;
			}
			if (IsKeyPressed(KeyboardKey.KEY_BACKSPACE))
			{
				Value /= 10;
			}
			for (int i = 0; i <= 9; i++)
			{
				if (IsKeyPressed(KeyboardKey.KEY_ZERO + i))
				{
					Value = i + (Value * 10);
				}
			}
		}

		DrawOutline(true);
		DrawText(valueText, X + PADDING, Y + PADDING, FONT_SIZE, PrimaryColor);
	}

	public static int GetMinWidth(int min, int max) => (2 * PADDING) + Math.Max(MeasureText(new string('0', GetMaxNumberSize(min, max)), FONT_SIZE), MeasureText(max.ToString(), FONT_SIZE));

	private static int GetMaxNumberSize(int a, int b)
	{
		int mag = (int)Math.Ceiling(Math.Log10(Math.MaxMagnitude(a, b)));
		if (a < 0 || b < 0)
		{
			mag++;
		}
		return mag;
	}
}
