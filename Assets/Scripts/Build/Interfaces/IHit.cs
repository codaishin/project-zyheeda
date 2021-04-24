public interface IHit
{
	bool TryHit<TSource, TTarget>(TSource source, out TTarget target);
}
