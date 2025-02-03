namespace LibTNT;

public struct Vec3
{
	public double X, Y, Z;

	public Vec3(double x, double y, double z)
	{
		X = x;
		Y = y;
		Z = z;
	}

	public readonly double Dot(Vec3 v) => (X * v.X) + (Y * v.Y) + (Z * v.Z);

	public readonly double SquareDistanceTo(Vec3 v)
	{
		double dx = X - v.X;
		double dy = Y - v.Y;
		double dz = Z - v.Z;
		return (dx * dx) + (dy * dy) + (dz * dz);
	}

	public readonly double SquareLength() => (X * X) + (Y * Y) + (Z * Z);

	public readonly double Length() => Math.Sqrt(SquareLength());

	public Vec3 Normalize()
	{
		double l = Length();
		X /= l;
		Y /= l;
		Z /= l;
		return this;
	}

	public Vec3 NormalizeFast()
	{
		// Math.ReciprocalSqrtEstimate only works on ARM for some reason
		// So use the Quake 3 method
		float number = (float)SquareLength();
		const float THREE_HALFS = 3f / 2f;

		float x2 = number * 0.5f;
		float y = number;
		unchecked
		{
			int i = BitConverter.SingleToInt32Bits(y); // Avoiding unsafe code
			i = 0x5F3759DF - (i >>> 1);
			y = BitConverter.Int32BitsToSingle(i); // Avoiding unsafe code
			y *= THREE_HALFS - (x2 * y * y);
			// Slightly better accuracy for release mode
#if !DEBUG
			y *= THREE_HALFS - (x2 * y * y);
#endif
		}

		X *= y;
		Y *= y;
		Z *= y;
		return this;
	}

	public readonly double DistanceTo(Vec3 v) => Math.Sqrt(SquareDistanceTo(v));

	public static Vec3 operator *(Vec3 v, double x)
	{
		v.X *= x;
		v.Y *= x;
		v.Z *= x;
		return v;
	}

	public static Vec3 operator /(Vec3 v, double x)
	{
		v.X /= x;
		v.Y /= x;
		v.Z /= x;
		return v;
	}

	public static Vec3 operator +(Vec3 v1, Vec3 v2) => new(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
	public static Vec3 operator -(Vec3 v1, Vec3 v2) => new(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
	public static Vec3 operator -(Vec3 v)
	{
		v.X = -v.X;
		v.Y = -v.Y;
		v.Z = -v.Z;
		return v;
	}

	public static implicit operator System.Numerics.Vector3(Vec3 v) => new((float)v.X, (float)v.Y, (float)v.Z);
}
