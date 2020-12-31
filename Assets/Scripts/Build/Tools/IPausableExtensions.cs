using System.Collections;

public static class IPausableExtensions
{
	public static
	IEnumerator Manage<TPauseYield>(this IPausable<TPauseYield> pausable,
	                                IEnumerator enumerator)
	{
		while (true) {
			if (pausable.Paused) {
				yield return pausable.Pause;
			} else if (enumerator.MoveNext()) {
				yield return enumerator.Current;
			} else {
				yield break;
			}
		};
	}
}
