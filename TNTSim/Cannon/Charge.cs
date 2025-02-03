namespace TNTSim.Cannon;

internal struct Charge : ICloneable<Charge>
{
	public static readonly Charge DEFAULT = new()
	{
		tntCount = 0,
		scheduleCount = 1,
		fuse = 1,
		cancelX = true,
		cancelZ = true,
		single = false,
	};

	public int tntCount, scheduleCount, fuse;
	public bool cancelX, cancelZ;
	public bool single;

	public Charge()
	{
		scheduleCount = fuse = 1;
	}

	public Charge Clone() => new()
	{
		tntCount = tntCount,
		scheduleCount = scheduleCount,
		fuse = fuse,
		cancelX = cancelX,
		cancelZ = cancelZ,
		single = single
	};
}
