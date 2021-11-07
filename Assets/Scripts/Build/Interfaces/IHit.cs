public interface IHit
{
	Maybe<T> Try<T>(T source) where T : notnull;
}
