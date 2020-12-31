using System.Collections;

public static class IPausableExtensions
{
	public static
	IEnumerator Manage<T>(this IPausable<T> pausable, IEnumerator enumerator)
	{
		bool notEmpty = enumerator.MoveNext();
		while (notEmpty) {
			if (pausable.Paused) {
				yield return pausable.Pause;
			} else {
				yield return enumerator.Current;
				notEmpty = enumerator.MoveNext();
			}
		}
	}
}