namespace TNTSim.Components;

internal sealed class ComponentGroupBuilder
{
	private readonly List<Func<int, int, Component>> components = new();
	private readonly List<int> rowStarts = new();
	private Color? color = null;

	public ComponentGroupBuilder WithColor(Color c)
	{
		color = c;
		return this;
	}

	public ComponentGroupBuilder AddCenteredText(string text, int width)
	{
		components.Add((x, y) => new CenteredText(text, x, y, width, PADDING + ((CONTROL_HEIGHT + PADDING) * (rowStarts.Count + 1))));
		return this;
	}

	public ComponentGroupBuilder AddText(string text)
	{
		components.Add((x, y) => new TextComponent(text, x, y));
		return this;
	}

	public ComponentGroupBuilder AddButton(string text, int? width = null, Action? onClick = null)
	{
		components.Add((x, y) => new Button(text, x, y, width ?? Button.GetMinWidth(text), onClick));
		return this;
	}

	public ComponentGroupBuilder AddCheckBox(string text, int? width = null, bool initial = false)
	{
		components.Add((x, y) => new CheckBox(text, x, y, width ?? CheckBox.GetMinWidth(text), initial));
		return this;
	}

	public ComponentGroupBuilder AddNumberBox(int min, int max, int? width = null, int? initial = null)
	{
		components.Add((x, y) => new NumberBox(min, max, x, y, width ?? NumberBox.GetMinWidth(min, max), initial));
		return this;
	}

	public ComponentGroupBuilder EndRow()
	{
		if (rowStarts.Count == 0 || rowStarts[^1] != components.Count)
		{
			rowStarts.Add(components.Count);
		}
		return this;
	}

	public ComponentGroup Build(int x, int y)
	{
		ComponentGroup g = new(x, y, ToComponentArray(x, y));
		if (color.HasValue)
		{
			g.PrimaryColor = color.Value;
		}
		return g;
	}

	public ComponentGroup Build(int y)
	{
		ComponentGroup g = new(0, y, ToComponentArray(0, y));
		return Build((WINDOW_WIDTH - g.Width) / 2, y);
	}

	public Component[] ToComponentArray(int x, int y)
	{
		Component[] arr = new Component[components.Count];
		int cx = x + PADDING;
		int cy = y + PADDING;
		int rowPtr = 0;
		for (int i = 0; i < components.Count; i++)
		{
			if (rowPtr < rowStarts.Count && rowStarts[rowPtr] == i)
			{
				cx = x + PADDING;
				cy += CONTROL_HEIGHT + PADDING;
				rowPtr++;
			}
			arr[i] = components[i](cx, cy);
			cx += arr[i].Width + PADDING;
			if (color.HasValue)
			{
				arr[i].PrimaryColor = color.Value;
			}
		}
		return arr;
	}
}
