namespace LibTNT;

public static class TNTMath
{
	public static Vec3 GetVelocity(Vec3 explosionCenter, Vec3 entityPos)
	{
		double distanceNormalized = (double)(entityPos.DistanceTo(explosionCenter) / 8f);
		// Exit if distance is too high
		if (distanceNormalized > 1.0D)
		{
			return default;
		}
		// Get direction vector, different for tnt for some reason
		Vec3 dir = entityPos - explosionCenter;
		// Exit if direction vector is 0
		if (dir.Length() == 0.0D)
		{
			return default;
		}
		// Normalize direction vector
		dir *= 1 / dir.Length();
		// Velocity magnitude is (1 - distance/2power) * exposure (exposure is always one bc we are in the air)
		return dir * (1.0D - distanceNormalized);
	}
}
