using System.Diagnostics;
using System.Numerics;

namespace TNTSim.Screens;

internal static class SimPreviewScreen
{
	private const float FOV = 90f;
	private const string RUNNING = "Running";
	private const string PAUSED = "Paused";
	private const string CONTROLS = "WASD - Move\nSpace - Up\nShift - Down\nArrows - Rotate\nP - Pause/Play\nT - Step 1 tick\nEscape - Exit";
	private static readonly int RUNNING_X = (WINDOW_WIDTH - MeasureText(RUNNING, FONT_SIZE)) / 2;
	private static readonly int PAUSED_X = (WINDOW_WIDTH - MeasureText(PAUSED, FONT_SIZE)) / 2;
	private static readonly Vector2 CONTROLS_V = new Vector2(WINDOW_WIDTH - PADDING, WINDOW_HEIGHT - PADDING) - MeasureTextEx(GetFontDefault(), CONTROLS, FONT_SIZE, 1.0f);

	private static readonly Stopwatch TIMER = new();
	private static Camera3D camera;
	private static Simulator? current = null;
	private static Settings currentSettings;
	private static double lastMSPT = 0;
	static float cameraYaw, cameraPitch;

	public static void Start(Settings settings)
	{
		currentSettings = settings;
		TIMER.Reset();
		camera = new()
		{
			FovY = FOV,
			Position = new Vector3(25, (float)settings.simulatorSettings.payloadY + 20, 25),
			Target = default,
			Up = Vector3.UnitY,
			Projection = CameraProjection.CAMERA_PERSPECTIVE
		};
		current = SimulatorFactory.Create(settings);
		cameraPitch = MathF.PI / 4f;
		cameraYaw = 5f * MathF.PI / 4f;
	}

	public static bool UpdateAndDraw()
	{
		if (current == null)
		{
			TraceLog(TraceLogLevel.LOG_WARNING, "Attempted to update simulation when there is no current simulation");
			return false;
		}
		BeginMode3D(camera);

		DrawGrid(10, 100);

		if (TIMER.ElapsedMilliseconds >= 50)
		{
			TimeSpan start = TIMER.Elapsed;
			current.Tick();
			TimeSpan end = TIMER.Elapsed;
			lastMSPT = (end - start).TotalMilliseconds;
			TIMER.Restart();
		}
		DrawEntities();

		EndMode3D();

		if (TIMER.IsRunning)
		{
			DrawText(RUNNING, RUNNING_X, WINDOW_HEIGHT - PADDING - FONT_SIZE, FONT_SIZE, Color.GREEN);
		}
		else
		{
			DrawText(PAUSED, PAUSED_X, WINDOW_HEIGHT - PADDING - FONT_SIZE, FONT_SIZE, Color.RED);
		}
		DrawText(CONTROLS, (int)CONTROLS_V.X, (int)CONTROLS_V.Y, FONT_SIZE, Color.BLACK);

		DrawText($"MSPT: {lastMSPT}ms", PADDING, WINDOW_HEIGHT - FONT_SIZE - PADDING, FONT_SIZE, Color.BLACK);
		DrawText($"TNT: {current.TNT.Count}", PADDING, WINDOW_HEIGHT - FONT_SIZE - PADDING - CONTROL_HEIGHT - PADDING, FONT_SIZE, Color.BLACK);
		DrawText($"FPS: {(int)(1 / GetFrameTime())}", PADDING, WINDOW_HEIGHT - FONT_SIZE - PADDING - (2 * (CONTROL_HEIGHT + PADDING)), FONT_SIZE, Color.BLACK);

		UpdateCameraControls();
		UpdateSimulationControls();
		if (IsKeyPressed(KeyboardKey.KEY_ESCAPE) || IsKeyPressed(KeyboardKey.KEY_TAB))
		{
			current = null;
			return false;
		}
		return true;
	}

	private static void DrawEntities()
	{
		if (current is null)
		{
			return;
		}
		foreach (TNT tnt in current.TNT)
		{
			Vec3 offset = tnt.velocity * (TIMER.ElapsedTicks / (50.0 * TimeSpan.TicksPerMillisecond));
			Vec3 pos = tnt.position + offset;

			Vector2 onScreen = GetWorldToScreen(pos, camera);
			if (onScreen.X is < -100 or >= (WINDOW_WIDTH + 100) || onScreen.Y is < -100 or >= (WINDOW_HEIGHT + 100))
			{
				continue;
			}

			DrawCube(pos, 0.98f, 0.98f, 0.98f, tnt.fuse / 5 % 2 == 0 ? Color.WHITE : Color.RED);
			if (currentSettings.showVelocity)
			{
				DrawLine3D(pos, pos + tnt.velocity, Color.GREEN);
			}
		}
		foreach (Vec3 exp in current.Explosions)
		{
			Vector2 onScreen = GetWorldToScreen(exp, camera);
			if (onScreen.X is < -100 or >= (WINDOW_WIDTH + 100) || onScreen.Y is < -100 or >= (WINDOW_HEIGHT + 100))
			{
				continue;
			}
			DrawCircle3D(exp, EXPLOSION_SIZE, Vector3.UnitX, 90f, Color.ORANGE);
		}
	}

	private static void UpdateCameraControls()
	{
		if (current == null)
		{
			TraceLog(TraceLogLevel.LOG_WARNING, "Attempted to update simulation when there is no current simulation");
			return;
		}
		float moveSpeed = 50f * GetFrameTime();
		float rotSpeed = 3f * GetFrameTime();

		bool space = IsKeyDown(KeyboardKey.KEY_SPACE);
		bool shift = IsKeyDown(KeyboardKey.KEY_LEFT_SHIFT) || IsKeyDown(KeyboardKey.KEY_RIGHT_SHIFT);
		bool w = IsKeyDown(KeyboardKey.KEY_W);
		bool a = IsKeyDown(KeyboardKey.KEY_A);
		bool s = IsKeyDown(KeyboardKey.KEY_S);
		bool d = IsKeyDown(KeyboardKey.KEY_D);
		bool up = IsKeyDown(KeyboardKey.KEY_UP);
		bool left = IsKeyDown(KeyboardKey.KEY_LEFT);
		bool down = IsKeyDown(KeyboardKey.KEY_DOWN);
		bool right = IsKeyDown(KeyboardKey.KEY_RIGHT);

		if (up && !down)
		{
			cameraPitch -= rotSpeed;
		}
		if (down && !up)
		{
			cameraPitch += rotSpeed;
		}

		if (left && !right)
		{
			cameraYaw += rotSpeed;
		}
		if (right && !left)
		{
			cameraYaw -= rotSpeed;
		}

		camera.Up = Vector3.UnitY;
		cameraPitch = float.Clamp(cameraPitch, -(MathF.PI / 2) + 0.001f, (MathF.PI / 2) - 0.001f);
#if DEBUG
		cameraYaw -= MathF.Tau * MathF.Floor(cameraYaw / MathF.Tau); // Wrap at 2pi so it looks nicer in debugger
#endif
		Quaternion q = Quaternion.CreateFromYawPitchRoll(cameraYaw, cameraPitch, 0);
		Vector3 fwd = Vector3.Transform(Vector3.UnitZ, q);
		fwd /= fwd.Length();
		Vector3 side = Vector3.Cross(fwd, Vector3.UnitY);

		if (space && !shift)
		{
			camera.Position += Vector3.UnitY * moveSpeed;
		}
		else if (shift && !space)
		{
			camera.Position -= Vector3.UnitY * moveSpeed;
		}
		if (w && !s)
		{
			camera.Position += fwd * moveSpeed;
		}
		if (s && !w)
		{
			camera.Position -= fwd * moveSpeed;
		}
		if (a && !d)
		{
			camera.Position -= side * moveSpeed;
		}
		if (d && !a)
		{
			camera.Position += side * moveSpeed;
		}

		camera.Target = fwd + camera.Position;
	}

	private static void UpdateSimulationControls()
	{
		if (current == null)
		{
			TraceLog(TraceLogLevel.LOG_WARNING, "Attempted to update simulation when there is no current simulation");
			return;
		}
		if (IsKeyPressed(KeyboardKey.KEY_P))
		{
			if (TIMER.IsRunning)
			{
				TIMER.Stop();
			}
			else
			{
				TIMER.Reset();
				TIMER.Start();
			}
		}
		if (IsKeyPressed(KeyboardKey.KEY_T))
		{
			TIMER.Reset();
			TIMER.Start();
			current.Tick();
			TIMER.Stop();
			lastMSPT = TIMER.Elapsed.TotalMilliseconds;
		}
	}
}
