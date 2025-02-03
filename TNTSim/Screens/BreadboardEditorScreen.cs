namespace TNTSim.Screens;

internal static class BreadboardEditorScreen
{
	private const int START_Y = CONTROL_HEIGHT + PADDING + PADDING;
	private const int SIDE_PADDING = 150;
	private const int BREADBOARD_WIDTH = WINDOW_WIDTH - (2 * SIDE_PADDING);
	private const int BUTTON_SIZE = BREADBOARD_WIDTH / 9;
	private const int BREADBOARD_WIDTH_REAL = BUTTON_SIZE * 9;
	private const int BREADBOARD_X = (WINDOW_WIDTH - BREADBOARD_WIDTH_REAL) / 2;
	private const string TITLE = "Breadboard Editor";
	private const string SCHEDULING_TEXT = "Scheduling Breadboard";
	private const string CONTINUATION_TEXT = "Continuation Breadboard";
	private const string DEFAULTS_TEXT = "Preset: Empty";
	private const string NUKE_DEFAULTS_TEXT = "Preset: Nuclear Explosion";
	private static readonly int TITLE_X = (WINDOW_WIDTH - MeasureText(TITLE, FONT_SIZE)) / 2;
	private static readonly int SCHEDULING_X = (WINDOW_WIDTH - MeasureText(SCHEDULING_TEXT, FONT_SIZE)) / 2;
	private static readonly int CONTINUATION_X = (WINDOW_WIDTH - MeasureText(CONTINUATION_TEXT, FONT_SIZE)) / 2;
	private static readonly int DEFAULTS_X = (WINDOW_WIDTH - Button.GetMinWidth(DEFAULTS_TEXT)) / 2;
	private static readonly int NUKE_DEFAULTS_X = (WINDOW_WIDTH - Button.GetMinWidth(NUKE_DEFAULTS_TEXT)) / 2;

	private static readonly BreadboardEditor schedEditor = new(BREADBOARD_X, START_Y + CONTROL_HEIGHT, BREADBOARD_WIDTH_REAL, BUTTON_SIZE * 6);
	private static readonly BreadboardEditor contEditor = new(BREADBOARD_X, schedEditor.GetBottomSide() + CONTROL_HEIGHT, BREADBOARD_WIDTH_REAL, BUTTON_SIZE * 6);
	private static readonly Button defaultsButton = new(DEFAULTS_TEXT, DEFAULTS_X, contEditor.GetBottomSide(), Button.GetMinWidth(DEFAULTS_TEXT), () => loadDefaults = true);
	private static readonly Button nukeDefaultsButton = new(NUKE_DEFAULTS_TEXT, NUKE_DEFAULTS_X, defaultsButton.GetBottomSide() + PADDING, Button.GetMinWidth(NUKE_DEFAULTS_TEXT), () => loadNukeDefaults = true);

	private static bool first = true;
	private static bool loadDefaults = false;
	private static bool loadNukeDefaults = false;

	public static void UpdateAndDraw(ref CannonSettings settings)
	{
		if (first)
		{
			first = false;
			schedEditor.Load(settings.schedulingBoard);
			contEditor.Load(settings.continuationBoard);
		}

		if (loadDefaults)
		{
			loadDefaults = false;
			schedEditor.Load(default);
			Breadboard cont = new();
			for (int i = 0; i < 5; i++)
			{
				cont[i, i] = Connection.INPUT;
				cont[i, i + 1] = Connection.NEXT_CHARGE_OUT;
			}
			contEditor.Load(cont);
		}

		if (loadNukeDefaults)
		{
			loadNukeDefaults = false;
			schedEditor.Load(NukeSettings.SCHED_BREADBOARD);
			Breadboard cont = new();
			for (int i = 0; i < 5; i++)
			{
				cont[i, i] = Connection.INPUT;
				cont[i, i + 1] = Connection.NEXT_CHARGE_OUT;
			}
			contEditor.Load(cont);
		}

		DrawText(TITLE, TITLE_X, 2 * PADDING, FONT_SIZE, Color.GRAY);
		DrawLine(0, CONTROL_HEIGHT + PADDING, WINDOW_WIDTH, CONTROL_HEIGHT + PADDING, Color.GRAY);

		DrawText(SCHEDULING_TEXT, SCHEDULING_X, START_Y + PADDING, FONT_SIZE, Color.GRAY);
		schedEditor.UpdateAndDraw();
		settings.schedulingBoard = schedEditor.Breadboard;
		DrawText(CONTINUATION_TEXT, CONTINUATION_X, schedEditor.GetBottomSide() + PADDING, FONT_SIZE, Color.GRAY);
		contEditor.UpdateAndDraw();
		settings.continuationBoard = contEditor.Breadboard;

		defaultsButton.UpdateAndDraw();
		nukeDefaultsButton.UpdateAndDraw();
	}
}
