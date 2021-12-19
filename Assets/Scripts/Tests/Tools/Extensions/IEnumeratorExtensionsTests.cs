using System.Collections;
using NUnit.Framework;

public class IEnumeratorExtensionsTests : TestCollection
{
	[Test]
	public void Run() {
		var count = 0;
		IEnumerator get() {
			++count;
			yield return null;
		}
		var enumerator = get().Concat(get());

		Assert.AreEqual(
			(true, true, false, 2),
			(enumerator.MoveNext(), enumerator.MoveNext(), enumerator.MoveNext(), count)
		);
	}

	[Test]
	public void Current() {
		IEnumerator getStr() {
			yield return "a";
		}
		IEnumerator getInt() {
			yield return -4;
			yield return 7;
		}
		var enumerator = getStr().Concat(getInt());

		enumerator.MoveNext();
		var a = (string)enumerator.Current;
		enumerator.MoveNext();
		var b = (int)enumerator.Current;
		enumerator.MoveNext();
		var c = (int)enumerator.Current;

		Assert.AreEqual(("a", -4, 7), (a, b, c));
	}
}
