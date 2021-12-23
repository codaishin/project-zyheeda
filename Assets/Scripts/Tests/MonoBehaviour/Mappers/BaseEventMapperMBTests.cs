using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BaseEventMapperMBTests : TestCollection
{
	private class MockMapperMB : BaseEventMapperMB<string, int>
	{
		public override Maybe<int> Map(string value) =>
			int.TryParse(value, out int result)
				? Maybe.Some(result)
				: Maybe.None<int>();
	}

	private class MockVoidMapperMB : BaseEventMapperMB<int>
	{
		public int? result;

		public override Maybe<int> Map() =>
			this.result != null
				? Maybe.Some(result.Value)
				: Maybe.None<int>();
	}

	[UnityTest]
	public IEnumerator ApplySome() {
		var called = 0;
		var mapper = new GameObject("obj").AddComponent<MockMapperMB>();

		yield return new WaitForEndOfFrame();

		mapper.onValueMapped.AddListener(v => called = v);

		yield return new WaitForEndOfFrame();

		mapper.Apply("42");

		Assert.AreEqual(42, called);
	}

	[UnityTest]
	public IEnumerator ApplyNone() {
		var called = 0;
		var mapper = new GameObject("obj").AddComponent<MockMapperMB>();

		yield return new WaitForEndOfFrame();

		mapper.onValueMapped.AddListener(_ => ++called);

		yield return new WaitForEndOfFrame();

		mapper.Apply("a");

		Assert.AreEqual(0, called);
	}

	[UnityTest]
	public IEnumerator ApplySomeVoid() {
		var called = 0;
		var mapper = new GameObject("obj").AddComponent<MockVoidMapperMB>();

		yield return new WaitForEndOfFrame();

		mapper.result = 42;
		mapper.onValueMapped.AddListener(v => called = v);

		yield return new WaitForEndOfFrame();

		mapper.Apply();

		Assert.AreEqual(42, called);
	}

	[UnityTest]
	public IEnumerator ApplyNoneVoid() {
		var called = 0;
		var mapper = new GameObject("obj").AddComponent<MockVoidMapperMB>();

		yield return new WaitForEndOfFrame();

		mapper.result = null;
		mapper.onValueMapped.AddListener(_ => ++called);

		yield return new WaitForEndOfFrame();

		mapper.Apply();

		Assert.AreEqual(0, called);
	}
}
