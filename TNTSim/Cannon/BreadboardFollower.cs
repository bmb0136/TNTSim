namespace TNTSim.Cannon;

internal static class BreadboardFollower
{
	// Caching follow breadboard calculations because they're expensive
	private static readonly Dictionary<Breadboard, int[]> FOLLOW_CACHE = new();

	/// <summary>
	/// Gets the index of the next charge after following the breadboard's pattern
	/// </summary>
	public static int FollowBreadboard(int startCharge, Breadboard breadboard)
	{
		if (FOLLOW_CACHE.TryGetValue(breadboard, out int[]? arr))
		{
			if (arr[startCharge] is >= 0 and < 6)
			{
				return arr[startCharge];
			}
		}

		for (int startSlice = 0; startSlice < 9; startSlice++)
		{
			if (breadboard[startSlice, startCharge] == Connection.INPUT)
			{
				int r = FollowFrom(startSlice, startCharge, breadboard);
				if (!FOLLOW_CACHE.ContainsKey(breadboard))
				{
					FOLLOW_CACHE[breadboard] = new int[6] { -1, -1, -1, -1, -1, -1 };
				}
				FOLLOW_CACHE[breadboard][startCharge] = r;
				return r;
			}
		}
		return -1;
	}

	private static int FollowFrom(int s, int c, Breadboard breadboard, List<(int s, int c)>? seen = null)
	{
		seen ??= new();
		if (c is < 0 or >= 6 || s is < 0 or >= 9)
		{
			return -1;
		}
		if (seen.Contains((s, c)))
		{
			return -1;
		}

		if (s > 0)
		{
			if (breadboard[s - 1, c] == Connection.PREV_SLICE_OUT)
			{
				return c;
			}
			if (breadboard[s - 1, c] == Connection.PREV_SLICE)
			{
				seen.Add((s, c));
				int r = FollowFrom(s - 1, c, breadboard, seen);
				if (r is >= 0 and <= 6)
				{
					return r;
				}
				seen.RemoveAt(seen.Count - 1);
			}
		}
		if (s <= 9)
		{
			if (breadboard[s + 1, c] == Connection.NEXT_SLICE_OUT)
			{
				return c;
			}
			if (breadboard[s + 1, c] == Connection.NEXT_SLICE)
			{
				seen.Add((s, c));
				int r = FollowFrom(s + 1, c, breadboard, seen);
				if (r is >= 0 and <= 6)
				{
					return r;
				}
				seen.RemoveAt(seen.Count - 1);
			}
		}
		if (c > 0)
		{
			if (breadboard[s, c - 1] == Connection.PREV_CHARGE_OUT)
			{
				return c - 1;
			}
			if (breadboard[s, c - 1] == Connection.PREV_CHARGE)
			{
				seen.Add((s, c));
				int r = FollowFrom(s, c - 1, breadboard, seen);
				if (r is >= 0 and <= 6)
				{
					return r;
				}
				seen.RemoveAt(seen.Count - 1);
			}
		}
		if (c <= 6)
		{
			if (breadboard[s, c + 1] == Connection.NEXT_CHARGE_OUT)
			{
				return c + 1;
			}
			if (breadboard[s, c + 1] == Connection.NEXT_CHARGE)
			{
				seen.Add((s, c));
				int r = FollowFrom(s, c + 1, breadboard, seen);
				if (r is >= 0 and <= 6)
				{
					return r;
				}
				seen.RemoveAt(seen.Count - 1);
			}
		}
		return -1;
	}
}
