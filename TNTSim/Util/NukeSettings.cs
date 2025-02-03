namespace TNTSim.Util;

internal static class NukeSettings
{
	public static readonly Charge[] CHARGES = new Charge[5]
	{
		new() { tntCount = 24, cancelX = false, cancelZ = false, single = false, fuse = 1, scheduleCount = 1 },
		new() { tntCount = 1, cancelX = true, cancelZ = true, single = true, fuse = 80, scheduleCount = 10 },
		new() { tntCount = 24, cancelX = false, cancelZ = false, single = false, fuse = 1, scheduleCount = 1 },
		new() { tntCount = 2, cancelX = true, cancelZ = true, single = true, fuse = 15, scheduleCount = 16 },
		new() { tntCount = 4, cancelX = true, cancelZ = true, single = false, fuse = 16, scheduleCount = 1 }
	};
	public static readonly Breadboard SCHED_BREADBOARD = new();

	static NukeSettings()
	{
		SCHED_BREADBOARD[0, 0] = Connection.PREV_CHARGE_OUT;
		SCHED_BREADBOARD[0, 1] = Connection.INPUT;
		SCHED_BREADBOARD[1, 2] = Connection.PREV_CHARGE_OUT;
		SCHED_BREADBOARD[1, 3] = Connection.INPUT;
	}
}
