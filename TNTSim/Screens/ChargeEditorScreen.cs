namespace TNTSim.Screens;

internal static class ChargeEditorScreen
{
	private const int START_Y = CONTROL_HEIGHT + PADDING + PADDING;
	private const int CHARGE_LABEL_WIDTH = 100;
	private const string TITLE = "Charge Editor";
	private const string DEFAULTS_TEXT = "Preset: Empty";
	private const string NUKE_DEFAULTS_TEXT = "Preset: Nuclear Explosion";
	private static readonly int TITLE_X = (WINDOW_WIDTH - MeasureText(TITLE, FONT_SIZE)) / 2;
	private static readonly int START_X = (WINDOW_WIDTH - CHARGE_LABEL_WIDTH - ChargeEditor.WIDTH) / 2;

	private static readonly CenteredText[] CHARGE_LABELS = Enumerable
		.Range(0, 5)
		.Select(i => new CenteredText($"Charge #{i + 1}", START_X, START_Y + i * (ChargeEditor.HEIGHT + PADDING), CHARGE_LABEL_WIDTH, ChargeEditor.HEIGHT))
		.ToArray();
	private static readonly ChargeEditor[] CHARGE_EDITORS = Enumerable.Range(0, 5)
		.Select(i => new ChargeEditor(START_X + CHARGE_LABEL_WIDTH, CHARGE_LABELS[i].Y))
		.ToArray();
	private static readonly Button DEFAULTS_BUTTON = new(DEFAULTS_TEXT, (WINDOW_WIDTH - Button.GetMinWidth(DEFAULTS_TEXT)) / 2, CHARGE_EDITORS[^1].GetBottomSide() + PADDING, Button.GetMinWidth(DEFAULTS_TEXT), () => shouldLoadDefaults = true);
	private static readonly Button NUKE_DEFAULTS_BUTTON = new(NUKE_DEFAULTS_TEXT, (WINDOW_WIDTH - Button.GetMinWidth(NUKE_DEFAULTS_TEXT)) / 2, DEFAULTS_BUTTON.GetBottomSide() + PADDING, Button.GetMinWidth(NUKE_DEFAULTS_TEXT), () => shouldLoadNukeDefaults = true);

	private static bool first = true;
	private static bool shouldLoadDefaults = false;
	private static bool shouldLoadNukeDefaults = false;

	public static void UpdateAndDraw(ref CannonSettings settings)
	{
		if (first)
		{
			first = false;
			for (int i = 0; i < 5; i++)
			{
				CHARGE_EDITORS[i].SetCharge(settings.GetCharge(i));
			}
		}

		if (shouldLoadDefaults)
		{
			shouldLoadDefaults = false;
			for (int i = 0; i < 5; i++)
			{
				settings.SetCharge(i, Charge.DEFAULT);
				CHARGE_EDITORS[i].SetCharge(Charge.DEFAULT);
			}
		}

		if (shouldLoadNukeDefaults)
		{
			shouldLoadNukeDefaults = false;
			for (int i = 0; i < 5; i++)
			{
				settings.SetCharge(i, NukeSettings.CHARGES[i]);
				CHARGE_EDITORS[i].SetCharge(NukeSettings.CHARGES[i]);
			}
		}

		DrawText(TITLE, TITLE_X, 2 * PADDING, FONT_SIZE, Color.GRAY);
		DrawLine(0, CONTROL_HEIGHT + PADDING, WINDOW_WIDTH, CONTROL_HEIGHT + PADDING, Color.GRAY);

		for (int i = 0; i < 5; i++)
		{
			CHARGE_LABELS[i].UpdateAndDraw();
			CHARGE_EDITORS[i].UpdateAndDraw();
			settings.SetCharge(i, CHARGE_EDITORS[i].Charge);
		}

		DEFAULTS_BUTTON.UpdateAndDraw();
		NUKE_DEFAULTS_BUTTON.UpdateAndDraw();

		DrawText("*Mutally exclusive with any random momentum", PADDING, WINDOW_HEIGHT - FONT_SIZE - PADDING, FONT_SIZE, Color.RED);
	}
}
