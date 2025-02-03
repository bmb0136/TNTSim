namespace TNTSim.Screens;

internal static class SimulationScreen
{
	private const int START_Y = CONTROL_HEIGHT + PADDING + PADDING;
	private const string TITLE = "Simulator";
	private static readonly int TITLE_X = (WINDOW_WIDTH - MeasureText(TITLE, FONT_SIZE)) / 2;
	private static readonly ComponentGroup SETTINGS = new ComponentGroupBuilder()
		.AddButton("KABOOM!", onClick: () => SetSubscreen(Subscreen.PREVIEW))
		.AddButton("Analyze", onClick: () => SetSubscreen(Subscreen.ANALYZER))
		.EndRow()
		.AddText("Payload Y")
		.AddNumberBox(0, 319, initial: 255)
		.EndRow()
		.AddCheckBox("Even Spacing")
		.EndRow()
		.AddCheckBox("Show Velocity", initial: true)
		.EndRow()
		.Build(START_Y);

	private static Subscreen subscreen = Subscreen.MAIN;
	private static bool first = false;

	public static void UpdateAndDraw(ref CannonSettings cannonSettings)
	{
		switch (subscreen)
		{
			case Subscreen.MAIN:
				SETTINGS.UpdateAndDraw();
				break;
			case Subscreen.PREVIEW:
				if (first)
				{
					Settings settings = GetSimSettings(cannonSettings);
					SimPreviewScreen.Start(settings);
					first = false;
				}
				else if (!SimPreviewScreen.UpdateAndDraw())
				{
					subscreen = Subscreen.MAIN;
				}
				break;
			case Subscreen.ANALYZER:
				if (first)
				{
					Settings settings = GetSimSettings(cannonSettings);
					SimAnalyzerScreen.Start(settings);
					first = false;
				}
				else if (!SimAnalyzerScreen.UpdateAndDraw())
				{
					subscreen = Subscreen.MAIN;
				}
				break;
		}

		DrawRectangle(0, 0, WINDOW_WIDTH, CONTROL_HEIGHT + PADDING, Color.RAYWHITE);
		DrawText(TITLE, TITLE_X, 2 * PADDING, FONT_SIZE, Color.GRAY);
		DrawLine(0, CONTROL_HEIGHT + PADDING, WINDOW_WIDTH, CONTROL_HEIGHT + PADDING, Color.GRAY);
	}

	private static Settings GetSimSettings(CannonSettings cannonSettings) => new()
	{
		cannonSettings = cannonSettings,
		simulatorSettings = new()
		{
			payloadY = SETTINGS.GetComponent<NumberBox>(3).Value
		},
		evenSpacing = SETTINGS.GetComponent<CheckBox>(4).Value,
		showVelocity = SETTINGS.GetComponent<CheckBox>(5).Value,
	};

	private static void SetSubscreen(Subscreen scr)
	{
		subscreen = scr;
		first = true;
	}

	private enum Subscreen
	{
		MAIN,
		PREVIEW,
		ANALYZER
	}
}
