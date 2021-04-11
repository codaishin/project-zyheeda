public interface IHit
{
	bool TryHit<TTarget>(out TTarget target);
}
