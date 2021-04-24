public interface IHit
{
	bool TryHit<T>(T source, out T target);
}
