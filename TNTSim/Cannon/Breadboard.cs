using System.Diagnostics.CodeAnalysis;

namespace TNTSim.Cannon;

internal struct Breadboard : ICloneable<Breadboard>
{
	// Store 9x6 3-bit values (162 bits)
	// Rows = Charge # (row 6 is after charge 5)
	// Columns = One rail (9 because power goes out 8 from the powered rail)
	private long data0, data1, data2;

	public Connection this[int slice, int charge]
	{
		readonly get
		{
			int i = 4 * (charge + (6 * slice));
			int j = i % 64;
			return (Connection)((i / 64) switch
			{
				0 => ((data0 & (0b1111L << j)) >> j) & 0b1111L,
				1 => ((data1 & (0b1111L << j)) >> j) & 0b1111L,
				2 => ((data2 & (0b1111L << j)) >> j) & 0b1111L,
				_ => 0
			});
		}
		set
		{
			int i = 4 * (charge + (6 * slice));
			int j = i % 64;
			long mask = ~(0b1111L << j);
			long data = ((long)value) << j;
			_ = (i / 64) switch
			{
				0 => data0 = (data0 & mask) | data,
				1 => data1 = (data1 & mask) | data,
				2 => data2 = (data2 & mask) | data,
				_ => 0
			};
		}
	}

	public Breadboard Clone() => new()
	{
		data0 = data0,
		data1 = data1,
		data2 = data2,
	};


	public enum Connection : byte
	{
		NONE = 0,
		INPUT = 1,
		NEXT_CHARGE = 2,
		NEXT_SLICE = 3,
		PREV_CHARGE = 4,
		PREV_SLICE = 5,
		NEXT_CHARGE_OUT = 6,
		NEXT_SLICE_OUT = 7,
		PREV_CHARGE_OUT = 8,
		PREV_SLICE_OUT = 9,
		IN_AND_OUT = 10
	}

	public static bool operator ==(Breadboard a, Breadboard b) =>
		a.data0 == b.data0 &&
		a.data1 == b.data1 &&
		a.data2 == b.data2;

	public static bool operator !=(Breadboard a, Breadboard b) => !(a == b);

	public override readonly bool Equals([NotNullWhen(true)] object? obj) => obj is Breadboard other && this == other;

	public override readonly int GetHashCode() => data0.GetHashCode() ^ data1.GetHashCode() ^ data2.GetHashCode();
}

internal static class ConnectionExt
{
	public static Connection Next(this Connection c) => c switch
	{
		Connection.NONE => Connection.INPUT,
		Connection.INPUT => Connection.NEXT_CHARGE,
		Connection.NEXT_CHARGE => Connection.NEXT_SLICE,
		Connection.NEXT_SLICE => Connection.PREV_CHARGE,
		Connection.PREV_CHARGE => Connection.PREV_SLICE,
		Connection.PREV_SLICE => Connection.NEXT_CHARGE_OUT,
		Connection.NEXT_CHARGE_OUT => Connection.NEXT_SLICE_OUT,
		Connection.NEXT_SLICE_OUT => Connection.PREV_CHARGE_OUT,
		Connection.PREV_CHARGE_OUT => Connection.PREV_SLICE_OUT,
		Connection.PREV_SLICE_OUT => Connection.IN_AND_OUT,
		Connection.IN_AND_OUT => Connection.NONE,
		_ => Connection.NONE,
	};

	public static Connection Previous(this Connection c) => c switch
	{
		Connection.NONE => Connection.IN_AND_OUT,
		Connection.INPUT => Connection.NONE,
		Connection.NEXT_CHARGE => Connection.INPUT,
		Connection.NEXT_SLICE => Connection.NEXT_CHARGE,
		Connection.PREV_CHARGE => Connection.NEXT_SLICE,
		Connection.PREV_SLICE => Connection.PREV_CHARGE,
		Connection.NEXT_CHARGE_OUT => Connection.PREV_SLICE,
		Connection.NEXT_SLICE_OUT => Connection.NEXT_CHARGE_OUT,
		Connection.PREV_CHARGE_OUT => Connection.NEXT_SLICE_OUT,
		Connection.PREV_SLICE_OUT => Connection.PREV_CHARGE_OUT,
		Connection.IN_AND_OUT => Connection.PREV_SLICE_OUT,
		_ => Connection.NONE,
	};
}