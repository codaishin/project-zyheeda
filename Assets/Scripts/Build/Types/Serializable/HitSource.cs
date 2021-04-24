public class HitSource : IHit
{
	public bool TryHit<T>(T source, out T target)
	{
		target = source;
		return true;
	}
}
