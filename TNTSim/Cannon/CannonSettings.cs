namespace TNTSim.Cannon;

internal struct CannonSettings : ICloneable<CannonSettings>
{
	public Charge charge1, charge2, charge3, charge4, charge5;
	public Breadboard schedulingBoard, continuationBoard;

	public void SetCharge(int i, Charge newCharge) => _ = i switch
	{
		0 => charge1 = newCharge,
		1 => charge2 = newCharge,
		2 => charge3 = newCharge,
		3 => charge4 = newCharge,
		4 => charge5 = newCharge,
		_ => default
	};

	public readonly Charge GetCharge(int i) => i switch
	{
		0 => charge1,
		1 => charge2,
		2 => charge3,
		3 => charge4,
		4 => charge5,
		_ => default
	};

	public void LoadDefaults()
	{
		for (int i = 0; i < 5; i++)
		{
			SetCharge(i, NukeSettings.CHARGES[i].Clone());
		}

		schedulingBoard = NukeSettings.SCHED_BREADBOARD.Clone();

		continuationBoard = default;
		for (int i = 0; i < 5; i++)
		{
			continuationBoard[i, i] = Connection.INPUT;
			continuationBoard[i, i + 1] = Connection.NEXT_CHARGE_OUT;
		}
	}

	public CannonSettings Clone() => new()
	{
		charge1 = charge1.Clone(),
		charge2 = charge2.Clone(),
		charge3 = charge3.Clone(),
		charge4 = charge4.Clone(),
		charge5 = charge5.Clone(),
		schedulingBoard = schedulingBoard.Clone(),
		continuationBoard = continuationBoard.Clone()
	};
}
