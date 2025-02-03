using System.Diagnostics.CodeAnalysis;

namespace LibTNT;

public struct Vec3B : IComparable<Vec3B>
{
	private const int SIZE = 16;
	private const double RANGE = SIZE * 127;

	public sbyte X, Y, Z;

	public static Vec3B FromPosition(Vec3 position)
	{
		return new()
		{
			X = (sbyte)(double.Clamp(position.X, -RANGE, RANGE) / SIZE),
			Y = (sbyte)(double.Clamp(position.Y, -RANGE, RANGE) / SIZE),
			Z = (sbyte)(double.Clamp(position.Z, -RANGE, RANGE) / SIZE),
		};
	}

	public static Vec3B operator +(Vec3B a, Vec3B b)
	{
		int x = a.X + b.X;
		int y = a.Y + b.Y;
		int z = a.Z + b.Z;
		sbyte newX = (sbyte)int.Clamp(x, sbyte.MinValue, sbyte.MaxValue);
		sbyte newY = (sbyte)int.Clamp(y, sbyte.MinValue, sbyte.MaxValue);
		sbyte newZ = (sbyte)int.Clamp(z, sbyte.MinValue, sbyte.MaxValue);
		return new()
		{
			X = newX,
			Y = newY,
			Z = newZ
		};
	}

	public static bool operator ==(Vec3B a, Vec3B b) => a.X == b.X && a.Y == b.Y && a.Z == b.Z;
	public static bool operator !=(Vec3B a, Vec3B b) => a.X != b.X || a.Y != b.Y || a.Z != b.Z;

	public override readonly bool Equals([NotNullWhen(true)] object? obj) => obj is Vec3B other && this == other;
	public override readonly int GetHashCode() => X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();

	public readonly int CompareTo(Vec3B other)
	{
		int dx = X - other.X;
		int dy = Y - other.Y;
		int dz = Z - other.Z;
		if (dx != 0)
		{
			return dx;
		}
		if (dy != 0)
		{
			return dy;
		}
		return dz;
	}
}
