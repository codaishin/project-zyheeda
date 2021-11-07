using System.Collections;

public static class IPausableExtensions
{
	private static bool PausedOrNextOf<TPauseYield>(
		this IPausable<TPauseYield> pausable,
		in IEnumerator enumerator,
		out object value
	) where TPauseYield : notnull {
		if (pausable.Paused) {
			value = pausable.Pause;
			return true;
		}
		if (enumerator.MoveNext()) {
			value = enumerator.Current;
			return true;
		}
		value = new object();
		return false;
	}

	public static
	IEnumerator Manage<TPauseYield>(
		this IPausable<TPauseYield> pausable,
		IEnumerator enumerator
	) where TPauseYield : notnull {
		while (pausable.PausedOrNextOf(enumerator, out object value)) {
			yield return value;
		};
	}
}
