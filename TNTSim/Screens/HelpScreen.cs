using System.Text;

namespace TNTSim.Screens;

internal static class HelpScreen
{
	private const string TITLE = "Help";
	private const int START_Y = CONTROL_HEIGHT + (PADDING * 10);
	private const int START_X = 50;
	private const int MAX_WIDTH = WINDOW_WIDTH - (2 * START_X);
	private static readonly string TEXT = string.Join("\n", new string[]
	{
		"Welcome to TNTSim, the high-performance Minecraft TNT physics simulation designed for cubicmeter's Orbital Strike Cannon v2",
		"",
		"With this tool, you can rapidly prototype payloads that you can use to destroy your enemies WITHOUT needing a NASA supercomputer!",
		"All of these settings should be transerable verbatim to the game.",
		"Numerical inputs can be changed by clicking and dragging up or down, or scrolling the mouse wheel up or down. They can be reset with right click.",
		"",
		"-- Charge Menu --",
		"Allows for the creation of up to 5 charges. A charge is a configurable batch of TNT, changing the parameters of a charge defines the size and shape of the resulting TNT.",
		"Selecting 'Single TNT' will make the cannon create 1 TNT instead of 8.",
		"If you do not need a charge, set its TNT Count to zero.",
		"The 'Schedule Count' paramter is used for the breadboards. See below for more info",
		"",
		"-- Breadboard Menu --",
		"Allows for the modification of both of the cannon's breadboards. A breadboard (in real life) is an electronics prototyping device, allowing one to quickly connect devices to build circuits.",
		"For the cannon, it is similar; breadboards are used to customize the order in which charges are created (aka 'scheduled') using rails and observers.",
		"Use left click or the scroll wheel to edit the 'observers' in either breadboard.",
		"Arrows represent an observer whose output is facing in the direction of the arrow (the 'face' will watch the block behind the arrow).",
		"An 'O' is an input; it represents an observer whose output is facing down (the 'face' is on top), and watches for input from the slice it is on.",
		"Arrows + an 'X' is an output; it represents the same observer as the arrow, but with another observer below it facing down that powers the bottom line of rails.",
		"The 'O' + 'X' represents two observers facing down, it passes the input straight down to the bottom line of rails.",
		"",
		"-- Scheduling Algorithm --",
		$"* NOTE: If you can read C# code, then read {nameof(SimulatorFactory)}.cs for the exact details. *",
		"Create TNT for the current charge",
		"Decrement its schedule count by 0.5 (each item represents 2 iterations for some reason).",
		"If the schedule count is greater than 0, follow the scheduling breadboard. If this fails, see below.",
		"Otherwise, reset the current charge's schedule count and follow the cotinuation breadboard.",
		"(The current charge is now the charge that the breadboard led to)",
	});
	private static readonly string[] PARTS;
	private static readonly int TITLE_X = (WINDOW_WIDTH - MeasureText(TITLE, FONT_SIZE)) / 2;
	private static readonly int MAX_SCROLL;
	private static int scroll = 0;


	static HelpScreen()
	{
		List<string> parts = new();
		StringBuilder sb = new();

		foreach (string line in TEXT.Split('\n'))
		{
			foreach (string word in line.Split(' ', StringSplitOptions.RemoveEmptyEntries))
			{
				string str = sb.ToString();
				int w = MeasureText(str, FONT_SIZE) + MeasureText(word, FONT_SIZE);
				if (w > MAX_WIDTH)
				{
					parts.Add(str);
					sb.Clear();
				}
				sb.Append(word).Append(' ');
			}
			parts.Add(sb.ToString());
			sb.Clear();
		}

		if (sb.Length > 0)
		{
			parts.Add(sb.ToString());
			sb.Clear();
		}

		PARTS = parts.ToArray();

		MAX_SCROLL = ((PARTS.Length + 1) * (FONT_SIZE + PADDING)) - (WINDOW_HEIGHT - START_Y);
	}

	public static void UpdateAndDraw()
	{
		for (int i = 0; i < PARTS.Length; i++)
		{
			DrawText(PARTS[i], START_X, START_Y + (i * (FONT_SIZE + PADDING)) - scroll, FONT_SIZE, Color.GRAY);
		}

		float wheel = GetMouseWheelMoveV().Y;
		if (Math.Abs(wheel) > 0.1f)
		{
			scroll -= (int)(5 * wheel);
		}
		scroll = Math.Clamp(scroll, 0, MAX_SCROLL);

		DrawRectangle(0, 0, WINDOW_WIDTH, CONTROL_HEIGHT + PADDING, Color.RAYWHITE);
		DrawText(TITLE, TITLE_X, 2 * PADDING, FONT_SIZE, Color.GRAY);
		DrawLine(0, CONTROL_HEIGHT + PADDING, WINDOW_WIDTH, CONTROL_HEIGHT + PADDING, Color.GRAY);
	}
}
