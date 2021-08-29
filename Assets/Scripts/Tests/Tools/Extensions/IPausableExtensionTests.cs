using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

public class IPausableExtensionTests : TestCollection
{
	private class MockPausable : IPausable<int>
	{
		public bool Paused { get; set; }
		public int Pause => -1;
	}

	[Test]
	public void PauseIteration()
	{
		IEnumerator generate() { yield return 42; }
		var enumerator = generate();
		var pausable = new MockPausable();

		enumerator = pausable.Manage(enumerator);
		pausable.Paused = true;

		Assert.AreEqual(
			(true, -1),
			((bool, int))(enumerator.MoveNext(), enumerator.Current)
		);
	}

	[Test]
	public void PauseIterationTwice()
	{
		IEnumerator generate() { yield return 42; }
		var enumerator = generate();
		var pausable = new MockPausable();

		enumerator = pausable.Manage(enumerator);
		pausable.Paused = true;

		enumerator.MoveNext();
		Assert.AreEqual(
			(true, -1),
			((bool, int))(enumerator.MoveNext(), enumerator.Current)
		);
	}

	[Test]
	public void IterateUnpaused()
	{
		var enumerator = new int[] { 0, 1, 2 }.GetEnumerator();
		var pausable = new MockPausable();
		var result = new List<int>();

		enumerator = pausable.Manage(enumerator);
		while (enumerator.MoveNext()) result.Add((int)enumerator.Current);

		CollectionAssert.AreEqual(new int[] { 0, 1, 2 }, result);
	}

	[Test]
	public void IterateWithIntermittenedPause()
	{
		var enumerator = new int[] { 0, 1, 2 }.GetEnumerator();
		var pausable = new MockPausable();
		var result = new List<int>();

		enumerator = pausable.Manage(enumerator);
		while (enumerator.MoveNext()) {
			result.Add((int)enumerator.Current);
			pausable.Paused = !pausable.Paused;
		}

		CollectionAssert.AreEqual(new int[] { 0, -1, 1, -1, 2, -1 }, result);
	}

	[Test]
	public void WhenPausedPreventFirstIteration()
	{
		var ran = false;
		IEnumerator generate() { ran = true; yield break; }
		var enumerator = generate();
		var pausable = new MockPausable();

		enumerator = pausable.Manage(enumerator);
		pausable.Paused = true;

		enumerator.MoveNext();

		Assert.False(ran);
	}
}
