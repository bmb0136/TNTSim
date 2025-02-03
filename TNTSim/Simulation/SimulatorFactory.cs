namespace TNTSim.Simulation;

internal static class SimulatorFactory
{
	public static Simulator Create(Settings settings)
	{
		List<TNT> payload = CreatePayload(settings);

		Simulator context = new(settings.simulatorSettings, payload);

		TickOnceAndCap(context);

		return context;
	}

	/// <summary>
	/// Spawns the TNT of the payload
	/// </summary>
	private static List<TNT> CreatePayload(Settings settings)
	{
		List<TNT> list = new();

		// Make changes on this, so we can keep the original
		CannonSettings copy = settings.cannonSettings.Clone();

		for (int i = 0; i < 5; i++)
		{
			Charge c = copy.GetCharge(i);
			c.scheduleCount *= 2; // For some reason the cannon creates 2x the schedule count
			copy.SetCharge(i, c);
		}

		uint tntOrder = 0;
		int index = 0;
		while (index is >= 0 and < 5)
		{
			Charge charge = copy.GetCharge(index);
			int count = charge.tntCount * (charge.single ? 1 : 8);
			for (int i = 0; i < count; i++)
			{
				double theta = Random.Shared.NextDouble() * Math.Tau;
				if (settings.evenSpacing)
				{
					theta = i * Math.Tau / count;
				}
				double vx = charge.cancelX ? 0 : (0.02f * Math.Sin(theta));
				double vz = charge.cancelZ ? 0 : (0.02f * Math.Cos(theta));

				TNT tnt = new(tntOrder++)
				{
					// Random momentum and fuse timer are mutually exclusive
					fuse = 80 - (charge.cancelX && charge.cancelZ ? charge.fuse : 1),
					position = new(0, settings.simulatorSettings.payloadY, 0)
				};

				if (!charge.cancelX)
				{
					tnt.velocity += new Vec3(vx, 0, 0);
				}
				if (!charge.cancelZ)
				{
					tnt.velocity += new Vec3(0, 0, vz);
				}

				list.Add(tnt);
			}

			charge.scheduleCount--;
			copy.SetCharge(index, charge);
			if (charge.scheduleCount > 0)
			{
				int oldIndex = index;
				index = BreadboardFollower.FollowBreadboard(index, settings.cannonSettings.schedulingBoard);
				// If found then use it
				if (index is >= 0 and < 5)
				{
					continue;
				}
				index = oldIndex; // Else use continuation board
			}

			// Reset for next use
			copy.SetCharge(index, settings.cannonSettings.GetCharge(index));
			index = BreadboardFollower.FollowBreadboard(index, settings.cannonSettings.continuationBoard);
		}

		return list;
	}

	/// <summary>
	/// Simulates the teleportation of the payload. Steps 1 tick, then cancels velocity greater than 10m/s (component-wise)
	/// </summary>
	private static void TickOnceAndCap(Simulator context)
	{
		context.Tick(true);
		context.ModifyEntitiesInOrder((TNT tnt) =>
		{
			if (Math.Abs(tnt.velocity.X) > 10)
			{
				tnt.velocity.X = 0;
			}
			if (Math.Abs(tnt.velocity.Y) > 10)
			{
				tnt.velocity.Y = 0;
			}
			if (Math.Abs(tnt.velocity.Z) > 10)
			{
				tnt.velocity.Z = 0;
			}
		});
	}
}