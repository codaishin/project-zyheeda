public class HitSource : IHit
{
	public bool Try<T>(T source, out T target)
	{
		target = source;
		return true;
	}
}
