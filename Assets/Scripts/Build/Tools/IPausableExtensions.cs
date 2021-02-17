using System.Collections;

public static class IPausableExtensions
{
	private static
	bool PausedOrNextOf<TPauseYield>(this IPausable<TPauseYield> pausable,
	                                 in IEnumerator enumerator,
	                                 out object value)
	{
		if (pausable.Paused) {
			value = pausable.Pause;
			return true;
		}
		if (enumerator.MoveNext()) {
			value = enumerator.Current;
			return true;
		}
		value = default;
		return false;
	}

	public static
	IEnumerator Manage<TPauseYield>(
		this IPausable<TPauseYield> pausable,
		IEnumerator enumerator
	) {
		while (pausable.PausedOrNextOf(enumerator, out object value)) {
			yield return value;
		};
	}
}
