InitWindow(WINDOW_WIDTH, WINDOW_HEIGHT, "TNT Sim");
SetExitKey(KeyboardKey.KEY_NULL);

SetTargetFPS(Enumerable.Range(0, GetMonitorCount()).Max(GetMonitorRefreshRate));

Screen screen = Screen.CHARGES;
CannonSettings settings = new();
settings.LoadDefaults();

bool nextTabPressed = false;
bool prevTabPressed = false;
const string NEXT_TEXT = "Next Page >";
int NEXT_TEXT_W = MeasureText(NEXT_TEXT, FONT_SIZE);
Button nextTabButton = new(NEXT_TEXT, WINDOW_WIDTH - (3 * PADDING) - NEXT_TEXT_W, PADDING / 2, NEXT_TEXT_W + (2 * PADDING), () => nextTabPressed = true);
const string PREV_TEXT = "< Previous Page";
int PREV_TEXT_W = MeasureText(PREV_TEXT, FONT_SIZE);
Button prevTabButton = new(PREV_TEXT, PADDING, PADDING / 2, PREV_TEXT_W + (2 * PADDING), () => prevTabPressed = true);

while (!WindowShouldClose())
{
	BeginDrawing();
	ClearBackground(Color.RAYWHITE);

	switch (screen)
	{
		case Screen.CHARGES:
			ChargeEditorScreen.UpdateAndDraw(ref settings);
			break;
		case Screen.BREADBOARDS:
			BreadboardEditorScreen.UpdateAndDraw(ref settings);
			break;
		case Screen.SIMULATION:
			SimulationScreen.UpdateAndDraw(ref settings);
			break;
		case Screen.HELP:
			HelpScreen.UpdateAndDraw();
			break;
		default:
			screen = Screen.CHARGES;
			break;
	}

	nextTabButton.UpdateAndDraw();
	prevTabButton.UpdateAndDraw();
	Screen oldScreen = screen;
	if (nextTabPressed)
	{
		nextTabPressed = false;
		screen = screen.Next();
	}
	if (prevTabPressed)
	{
		prevTabPressed = false;
		screen = screen.Previous();
		
	}
	if (IsKeyPressed(KeyboardKey.KEY_TAB))
	{
		bool shift = IsKeyDown(KeyboardKey.KEY_LEFT_SHIFT) || IsKeyDown(KeyboardKey.KEY_RIGHT_SHIFT);
		screen = shift ? screen.Previous() : screen.Next();
	}
	if (oldScreen != screen && IsCursorHidden())
	{
		EnableCursor();
	}

	EndDrawing();
}

CloseWindow();
