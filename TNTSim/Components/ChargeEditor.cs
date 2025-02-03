namespace TNTSim.Components;

internal sealed class ChargeEditor : ComponentGroup
{
	public static readonly int WIDTH = new ComponentGroup(0, 0, GetComponents(0, 0)).Width;
	public static readonly int HEIGHT = new ComponentGroup(0, 0, GetComponents(0, 0)).Height;

	public Charge Charge => charge;
	private Charge charge = new();
	private readonly NumberBox tntCountBox, scheduleCountBox, fuseBox;
	private readonly CheckBox cancelXCheck, cancelZCheck, singleCheck;

	public ChargeEditor(int x, int y) : base(x, y, GetComponents(x, y))
	{
		tntCountBox = GetComponent<NumberBox>(1);
		scheduleCountBox = GetComponent<NumberBox>(3);
		fuseBox = GetComponent<NumberBox>(5);

		cancelXCheck = GetComponent<CheckBox>(7);
		cancelZCheck = GetComponent<CheckBox>(8);
		singleCheck = GetComponent<CheckBox>(9);
	}

	public override void UpdateAndDraw()
	{
		base.UpdateAndDraw();
		charge.tntCount = tntCountBox.Value;
		charge.scheduleCount = scheduleCountBox.Value;
		charge.fuse = fuseBox.Value;

		charge.cancelX = cancelXCheck.Value;
		charge.cancelZ = cancelZCheck.Value;
		charge.single = singleCheck.Value;
		if (!(charge.cancelX && charge.cancelZ))
		{
			charge.fuse = fuseBox.Value = 1;
		}
	}

	private static Component[] GetComponents(int x, int y) => new ComponentGroupBuilder()
		.AddText("TNT Count")
		.AddNumberBox(0, DROPPER_COUNT)
		.AddText("Schedule Count")
		.AddNumberBox(1, DROPPER_COUNT)
		.AddText("Fuse Timer*")
		.AddNumberBox(1, 80)
		.EndRow()
		.AddText("TNT Settings")
		.AddCheckBox("Cancel X", initial: true)
		.AddCheckBox("Cancel Z", initial: true)
		.AddCheckBox("Single")
		.ToComponentArray(x, y);

	public void SetCharge(Charge newCharge)
	{
		tntCountBox.Value = newCharge.tntCount;
		scheduleCountBox.Value = newCharge.scheduleCount;
		fuseBox.Value = newCharge.fuse;

		cancelXCheck.Value = newCharge.cancelX;
		cancelZCheck.Value = newCharge.cancelZ;
		singleCheck.Value = newCharge.single;
	}
}
