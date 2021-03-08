using System;
using System.Collections;
using NUnit.Framework;

public class FinalizableTests : TestCollection
{
	[Test]
	public void EnumerateWrapped()
	{
		var called = 0;
		IEnumerator get() {
			++called;
			yield return null;
			++called;
			yield return null;
		}

		var managed = new Finalizable { wrapped = get() };

		managed.MoveNext();
		managed.MoveNext();
		managed.MoveNext();

		Assert.AreEqual(2, called);
	}

	[Test]
	public void MoveNext()
	{
		IEnumerator get() {
			yield return null;
			yield return null;
		}

		var managed = new Finalizable { wrapped = get() };

		Assert.AreEqual(
			(true, true, false),
			(managed.MoveNext(), managed.MoveNext(), managed.MoveNext())
		);
	}

	[Test]
	public void Current()
	{
		IEnumerator get() {
			yield return 3;
			yield return "hello";
		}

		var managed = new Finalizable { wrapped = get() };

		managed.MoveNext();
		var a = (int)managed.Current;
		managed.MoveNext();
		var b = (string)managed.Current;

		Assert.AreEqual((3, "hello"), (a, b));
	}

	[Test]
	public void OnFinalize()
	{
		var count = 0;
		var called = 0;
		IEnumerator get() {
			++count;
			yield return null;
			++count;
			yield return null;
		}

		var managed = new Finalizable { wrapped = get() };

		managed.OnFinalize += () => called = count;

		managed.MoveNext();
		managed.MoveNext();
		managed.MoveNext();

		Assert.AreEqual(2, called);
	}

	[Test]
	public void OnFinalizeSetToNull()
	{
		var called = 0;
		IEnumerator get() {
			yield return null;
			yield return null;
		}

		var managed = new Finalizable { wrapped = get() };

		managed.OnFinalize += () => ++called;

		managed.MoveNext();
		managed.MoveNext();
		managed.MoveNext();

		managed.wrapped = get();

		managed.MoveNext();
		managed.MoveNext();
		managed.MoveNext();

		Assert.AreEqual(1, called);
	}

	private class MockWrappee : IEnumerator
	{
		public bool MoveNext() => throw new NotImplementedException();
		public object Current => throw new NotImplementedException();
		public void Reset() => throw new NotImplementedException();
	}

	[Test]
	public void Reset()
	{
		IEnumerator get() { yield break; }
		var enumerator = get();
		var managed = new Finalizable{ wrapped = enumerator };

		Assert.Throws<NotSupportedException>(() => managed.Reset());
	}
}
