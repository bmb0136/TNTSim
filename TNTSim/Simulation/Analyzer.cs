using System.Diagnostics;

namespace TNTSim.Simulation;

internal class Analyzer
{
	public double PercentComplete => currentTick / 80.0;
	public IReadOnlyList<double> MSPT => mspt;
	public IReadOnlyList<int> TNTCounts => tntCounts;

	private readonly Simulator context;
	private readonly Task task;
	private readonly double[] mspt = new double[80];
	private readonly int[] tntCounts = new int[80];
	private int currentTick = 0;

	public Analyzer(Settings settings)
	{
		context = SimulatorFactory.Create(settings);
		task = Task.Run(Analyze);
	}

	private void Analyze()
	{
		MeasureMSPTAndTNTCount();
	}

	private void MeasureMSPTAndTNTCount()
	{
		Stopwatch sw = new();
		while (currentTick < 80)
		{
			tntCounts[currentTick] = context.TNT.Count;

			sw.Restart();
			context.Tick();

			TimeSpan elapsed = sw.Elapsed;
			mspt[currentTick] = elapsed.TotalMilliseconds;

			currentTick++;
		}
	}

	public bool IsComplete() => task.IsCompleted;
}
