using System.Diagnostics.CodeAnalysis;

namespace LibTNT;

public sealed class TNT(uint order)
{
	private const double CENTER_OFFSET = 0.98F * 0.0625D;


	public bool Removed { get; private set; } = false;
	public readonly uint order = order;
	public Vec3 position, velocity;
	public int fuse = 80;
	public bool loaded;
	public Vec3B spatialBucket;

	public void Tick(Simulator simulator, bool firstTick = false)
	{
		loaded = true;
		velocity.Y -= 0.04;

		position += velocity;
		Vec3B oldBucket = spatialBucket;
		Vec3B newBucket = Vec3B.FromPosition(position);
		if (newBucket != oldBucket)
		{
			simulator.MoveToSpatialBucket(this, newBucket);
			spatialBucket = newBucket;
		}

		if (firstTick)
		{
			velocity = default;
		}

		velocity *= 0.98;

		// If on ground
		if (position.Y <= 0)
		{
			position.Y = 0;
			velocity.X *= 0.7;
			velocity.Z *= 0.7;
			velocity.Y = 0;
		}

		fuse--;
		if (fuse <= 0)
		{
			Removed = true;
			Explode(simulator);
		}
	}

	private void Explode(Simulator simulator)
	{
		Vec3 center = position + new Vec3(0, CENTER_OFFSET, 0);
		simulator.LogExplosion(center);
		uint thisID = order;
		for (int i = 0; i < 27; i++)
		{
			Vec3B offset = new()
			{
				X = (sbyte)((i % 3) - 1),
				Y = (sbyte)((i / 3 % 3) - 1),
				Z = (sbyte)((i / 9) - 1),
			};
			simulator.ModifyEntitiesInBucket(spatialBucket + offset, (TNT other) =>
			{
				if (other.order == thisID || !other.loaded)
				{
					return;
				}
				other.velocity += TNTMath.GetVelocity(center, other.position);
			});
		}
	}

	public static bool operator ==(TNT a, TNT b) => a.order == b.order;
	public static bool operator !=(TNT a, TNT b) => a.order != b.order;

	public override bool Equals([NotNullWhen(true)] object? obj) => obj is TNT other && order == other.order;
	public override int GetHashCode() => order.GetHashCode();
}
