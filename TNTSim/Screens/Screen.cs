namespace TNTSim.Screens;

internal enum Screen
{
	CHARGES = 0,
	BREADBOARDS = 1,
	SIMULATION = 2,
	HELP = 3,
}

internal static class ScreenExt
{
	public static Screen Next(this Screen s) => s switch
	{
		Screen.CHARGES => Screen.BREADBOARDS,
		Screen.BREADBOARDS => Screen.SIMULATION,
		Screen.SIMULATION => Screen.HELP,
		Screen.HELP => Screen.CHARGES,
		_ => Screen.CHARGES
	};

	public static Screen Previous(this Screen s) => s switch
	{
		Screen.CHARGES => Screen.HELP,
		Screen.BREADBOARDS => Screen.CHARGES,
		Screen.SIMULATION => Screen.BREADBOARDS,
		Screen.HELP => Screen.SIMULATION,
		_ => Screen.CHARGES
	};
}