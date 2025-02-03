namespace TNTSim;

internal interface ICloneable<out T> where T : ICloneable<T>
{
	public T Clone();
}
