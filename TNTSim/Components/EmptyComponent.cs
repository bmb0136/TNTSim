namespace TNTSim.Components;

internal sealed class EmptyComponent : Component
{
	public static readonly EmptyComponent INSTANCE = new();

	private EmptyComponent() : base(0, 0, 0, 0) { }

	public override void UpdateAndDraw() { }
}
