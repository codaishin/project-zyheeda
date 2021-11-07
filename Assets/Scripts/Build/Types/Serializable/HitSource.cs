public class HitSource : IHit
{
	public Maybe<T> Try<T>(T source) where T : notnull => Maybe.Some(source);
}
