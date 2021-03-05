using System.Collections;

public static class IEnumeratorExtensions
{
	public static IEnumerator Concat(this IEnumerator fst, IEnumerator snd)
	{
		while (fst.MoveNext()) {
			yield return fst.Current;
		}
		while (snd.MoveNext()) {
			yield return snd.Current;
		}
	}
}
