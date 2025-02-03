namespace TNTSim.Components;

internal class ComponentGroup : Component
{
	private readonly Component[] components;

	public ComponentGroup(int x, int y, params Component[] components) : base(x, y, GetWidth(components), GetHeight(components)) => this.components = components;

	public override void UpdateAndDraw()
	{
		DrawOutline(false);
		foreach (Component item in components)
		{
			item.UpdateAndDraw();
		}
	}

	public T GetComponent<T>(int index) where T : Component =>
		components[index] as T ?? throw new ArgumentException($"Component was not a {typeof(T)}");

	private static int GetWidth(Component[] components)
	{
		int minX = components.Min(static c => c.X) - PADDING;
		int maxX = components.Max(static c => c.GetRightSide()) + PADDING;
		return maxX - minX;
	}

	private static int GetHeight(Component[] components)
	{
		int minY = components.Min(static c => c.Y) - PADDING;
		int maxY = components.Max(static c => c.GetBottomSide()) + PADDING;
		return maxY - minY;
	}
}
