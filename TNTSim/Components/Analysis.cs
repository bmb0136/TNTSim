namespace TNTSim.Components;
internal sealed class Analysis : Component
{
	public const int HEIGHT = 200;
	public const int TEXT_Y = PADDING;
	public const int GRAPH_Y = TEXT_Y + CONTROL_HEIGHT;
	public const int GRAPH_HEIGHT = HEIGHT - GRAPH_Y - PADDING;
	public const int AVG_Y = TEXT_Y + CONTROL_HEIGHT;
	public const int MED_Y = AVG_Y + CONTROL_HEIGHT;
	public const int MIN_Y = MED_Y + CONTROL_HEIGHT;
	public const int MAX_Y = MIN_Y + CONTROL_HEIGHT;
	public const int GRAPH_Y_PAD = PADDING * 10;
	public const int GRAPH_X_PAD = PADDING * 5;
	public const int AXIS_FONT_SIZE = FONT_SIZE / 3;
	public const int SPECIAL_MIN = 0;
	public const int SPECIAL_MAX = 1;
	public const int SPECIAL_MED = 2;
	public const float SPECIAL_POINT_SIZE = 2.5f;
	public const int AXIS_TEXT_OFFSET = AXIS_FONT_SIZE + PADDING;
	private static readonly Color[] SPECIAL_POINT_COLORS = new Color[3] { Color.RED, Color.BLUE, Color.ORANGE };

	public IReadOnlyList<double> Data
	{
		get => data;
		set
		{
			data = value;
			average = data.Average();
			min = data.Min();
			max = data.Max();
			median = data.Order().ElementAtOrDefault(data.Count / 2);

			minPos = data.Zip(Enumerable.Range(0, data.Count)).OrderBy(static t => t.First).FirstOrDefault().Second;
			maxPos = data.Zip(Enumerable.Range(0, data.Count)).OrderBy(static t => t.First).LastOrDefault().Second;
			medianPos = data.Zip(Enumerable.Range(0, data.Count)).Where(t => t.First == median).FirstOrDefault().Second;
		}
	}

	private readonly string name;
	private readonly int textX;
	private IReadOnlyList<double> data = Array.Empty<double>();
	private double average, min, max, median;
	private int minPos, maxPos, medianPos;
	private readonly int graphStartX;

	public Analysis(string name, int y) : base(PADDING, y, WINDOW_WIDTH - (PADDING * 2), HEIGHT)
	{
		this.name = name;
		textX = X + ((Width - MeasureText(name, FONT_SIZE)) / 2);
		graphStartX = X + (Width / 4);
	}

	public override void UpdateAndDraw()
	{
		DrawText(name, textX, Y + TEXT_Y, FONT_SIZE, PrimaryColor);

		DrawText($"Average: {average:F2}", PADDING, Y + AVG_Y, FONT_SIZE, PrimaryColor);
		DrawText($"Median: {median:F2}", PADDING, Y + MED_Y, FONT_SIZE, SPECIAL_POINT_COLORS[SPECIAL_MED]);
		DrawText($"Minimum: {min:F2}", PADDING, Y + MIN_Y, FONT_SIZE, SPECIAL_POINT_COLORS[SPECIAL_MIN]);
		DrawText($"Maximum: {max:F2}", PADDING, Y + MAX_Y, FONT_SIZE, SPECIAL_POINT_COLORS[SPECIAL_MAX]);
		DrawText($"Left: value over time", PADDING, Y + MAX_Y + CONTROL_HEIGHT, FONT_SIZE, PrimaryColor);
		DrawText($"Right: # of occurances", PADDING, Y + MAX_Y + (CONTROL_HEIGHT * 2), FONT_SIZE, PrimaryColor);

		int width = (Width - graphStartX - (PADDING * 4)) / 2;
		DrawLineChart(graphStartX + PADDING, Y + GRAPH_Y, width);

		DrawHistogram(graphStartX + PADDING + width + PADDING, Y + GRAPH_Y, width);
	}

	private void DrawHistogram(int startX, int startY, int width)
	{
		int bottomY = startY + GRAPH_HEIGHT - AXIS_TEXT_OFFSET;
		DrawLine(startX, startY + AXIS_TEXT_OFFSET, startX, bottomY + 1, SPECIAL_POINT_COLORS[SPECIAL_MIN]);
		DrawLine(startX + width, startY + AXIS_TEXT_OFFSET, startX + width, bottomY + 1, SPECIAL_POINT_COLORS[SPECIAL_MAX]);

		Span<double> heights = stackalloc double[width / 4];
		double maxH = 1;
		for (int i = 0; i < data.Count; i++)
		{
			double x = (data[i] - min) / (max - min) * (heights.Length + 2);
			if (!double.IsFinite(x))
			{
				continue;
			}
			int j = (int)double.Clamp(x, 0, heights.Length - 1e-9);
			heights[j]++;
			if (heights[j] > maxH)
			{
				maxH = heights[j];
			}
		}

		int first = heights.IndexOfAnyExcept(0);
		if (first < 0)
		{
			first = 0;
		}
		int last = heights.LastIndexOfAnyExcept(0);
		if (last >= heights.Length)
		{
			last = heights.Length - 1;
		}
		int count = last - first;
		if (count == 0)
		{
			return;
		}
		count++;
		float rectW = (float)width / count;

		first = first * data.Count / heights.Length;
		last = last * data.Count / heights.Length;

		string text = maxH.ToString("F0");
		int textX = startX - (MeasureText(text, AXIS_FONT_SIZE) / 2);
		DrawText(text, textX, startY, AXIS_FONT_SIZE, PrimaryColor);

		text = data.Order().ElementAtOrDefault(first).ToString("F0");
		textX = startX - (MeasureText(text, AXIS_FONT_SIZE) / 2);
		DrawText(text, textX, bottomY, AXIS_FONT_SIZE, PrimaryColor);

		text = data.Order().ElementAtOrDefault(last).ToString("F0");
		textX = startX + width - (MeasureText(text, AXIS_FONT_SIZE) / 2);
		DrawText(text, textX, bottomY, AXIS_FONT_SIZE, PrimaryColor);

		startY += AXIS_TEXT_OFFSET;
		int gh = GRAPH_HEIGHT - (AXIS_TEXT_OFFSET * 2);

		int medPos = int.Clamp((int)Math.Round((median - min) / (max - min) * width), 0, heights.Length - 1);
		int avgPos = int.Clamp((int)Math.Round((average - min) / (max - min) * width), 0, heights.Length - 1);

		for (int i = 0; i < count; i++)
		{
			double raw = heights[first + i];
			float h = (float)(raw * gh / maxH);
			float x = startX + (i * rectW);
			float y = startY + gh - h;
			DrawRectangleRec(new(x, y, rectW, h), Color.GREEN);

			int x2 = (int)(x + (rectW / 2));
			if (i == avgPos)
			{
				DrawLine(x2, startY, x2, bottomY, Color.GRAY);
			}
			if (i == medPos)
			{
				DrawLine(x2, startY, x2, bottomY, SPECIAL_POINT_COLORS[SPECIAL_MED]);
			}
		}

		// Draw bottom line after
		DrawLine(startX, bottomY, startX + width, bottomY, Color.GRAY);
	}

	private void DrawLineChart(int startX, int startY, int width)
	{
		DrawAxes(startX, startY, width);

		DrawAverageLine(startX, startY, width);

		double dx = (width - GRAPH_X_PAD) / (data.Count - 1.0);

		Span<(int x, int y)> points = stackalloc (int x, int y)[3];
		points.Fill((-1, -1));

		int last = 0;
		for (int i = 0; i < data.Count; i++)
		{
			double temp = data[i];
			temp *= (GRAPH_HEIGHT - GRAPH_Y_PAD) / (max - min);
			temp -= min;
			temp += GRAPH_Y_PAD / 2;
			temp = startY + GRAPH_HEIGHT - temp;
			int y = (int)temp;

			int x = (int)(startX + (i * dx) + (GRAPH_X_PAD / 2));

			if (i == minPos)
			{
				points[SPECIAL_MIN] = (x, y);
			}
			if (i == maxPos)
			{
				points[SPECIAL_MAX] = (x, y);
			}
			if (i == medianPos)
			{
				points[SPECIAL_MED] = (x, y);
			}

			if (i != 0)
			{
				DrawLine((int)(x - dx), last, x, y, Color.GREEN);
			}

			last = y;
		}

		for (int i = 0; i < points.Length; i++)
		{
			(int x, int y) = points[i];
			if (x > 0 && y > 0)
			{
				DrawCircle(x, y, SPECIAL_POINT_SIZE, SPECIAL_POINT_COLORS[i]);
			}
		}
	}

	private void DrawAxes(int startX, int startY, int width)
	{
		int bottomY = startY + GRAPH_HEIGHT - AXIS_TEXT_OFFSET;
		DrawLine(startX, startY + AXIS_TEXT_OFFSET, startX, bottomY + 1, PrimaryColor);
		DrawLine(startX, bottomY, startX + width, bottomY, PrimaryColor);

		string text = max.ToString("F0");
		int textX = startX - (MeasureText(text, AXIS_FONT_SIZE) / 2);
		DrawText(text, textX, startY, AXIS_FONT_SIZE, PrimaryColor);

		text = min.ToString("F0");
		textX = startX - (MeasureText(text, AXIS_FONT_SIZE) / 2);
		DrawText(text, textX, bottomY + PADDING, AXIS_FONT_SIZE, PrimaryColor);
	}

	private void DrawAverageLine(int startX, int startY, int width)
	{
		double temp = average;
		temp *= (GRAPH_HEIGHT - GRAPH_Y_PAD) / (max - min);
		temp -= min;
		temp += GRAPH_Y_PAD / 2;
		temp = startY + GRAPH_HEIGHT - temp;
		int y = (int)temp;
		DrawLine(startX + (GRAPH_X_PAD / 2), y, startX + width - (GRAPH_X_PAD / 2), y, Color.GRAY);
	}
}
