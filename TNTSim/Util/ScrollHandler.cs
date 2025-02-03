namespace TNTSim.Util;

internal sealed class ScrollHandler
{
	private readonly float amountPerTick;
	private float amount = 0;
	private int scrolled = 0;

	public ScrollHandler(float amountPerTick)
	{
		this.amountPerTick = amountPerTick;
	}

	public void Update(Func<bool>? incrementCondition = null)
	{
		if (incrementCondition?.Invoke() ?? true)
		{
			amount += GetMouseWheelMoveV().Y;
		}
		else
		{
			amount = 0;
			scrolled = 0;
		}

		if (Math.Abs(amount) >= amountPerTick)
		{
			scrolled = amount < 0 ? -1 : 1;
			amount = 0;
		}
	}

	public bool HasScrolledOneTick(out int sign)
	{
		sign = scrolled;
		scrolled = 0;
		return sign != 0;
	}
}
