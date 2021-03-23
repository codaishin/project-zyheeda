using System.Linq;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BaseOnMorphMBTests : TestCollection
{
	private class MockComponent : MonoBehaviour {}
	private class MockOnMorph : BaseOnMorphMB<GameObject, MockComponent>
	{
		public override bool TryMorph(GameObject seed, out MockComponent morph)
		{
			return seed.TryGetComponent(out morph);
		}
	}

	[Test]
	public void CallbacksDefaultNull()
	{
		var onMorph = new GameObject("morpher").AddComponent<MockOnMorph>();

		Assert.AreEqual(
			(default(MockOnMorph.SeedEvent), default(MockOnMorph.MorphEvent), default(MockOnMorph.SeedEvent)),
			(onMorph.onSuccessSeed, onMorph.onSuccessMorph, onMorph.onFailSeed)
		);
	}

	[UnityTest]
	public IEnumerator CallbacksInitOnStart()
	{
		var onMorph = new GameObject("morpher").AddComponent<MockOnMorph>();

		yield return new WaitForEndOfFrame();

		Assert.False(
			new object[] {
				onMorph.onFailSeed,
				onMorph.onSuccessMorph,
				onMorph.onSuccessSeed,
			}.Any(o => o == null)
		);
	}

	[UnityTest]
	public IEnumerator CallbacksInitOnlyWhenNull()
	{
		var onMorph = new GameObject("morpher").AddComponent<MockOnMorph>();
		onMorph.onFailSeed = new MockOnMorph.SeedEvent();
		onMorph.onSuccessSeed = new MockOnMorph.SeedEvent();
		onMorph.onSuccessMorph = new MockOnMorph.MorphEvent();
		var callbacks = (onMorph.onFailSeed, onMorph.onSuccessSeed, onMorph.onSuccessMorph);

		yield return new WaitForEndOfFrame();

		Assert.AreEqual(callbacks, (onMorph.onFailSeed, onMorph.onSuccessSeed, onMorph.onSuccessMorph ));
	}

	[UnityTest]
	public IEnumerator OnSuccessSeed()
	{
		var called = default(GameObject);
		var onMorph = new GameObject("morpher").AddComponent<MockOnMorph>();
		var seed = new GameObject("target").AddComponent<MockComponent>().gameObject;

		yield return new WaitForEndOfFrame();

		onMorph.onSuccessSeed.AddListener(s => called = s);
		onMorph.Morph(seed);

		Assert.AreSame(seed, called);
	}

	[UnityTest]
	public IEnumerator OnFailSeed()
	{
		var called = default(GameObject);
		var onMorph = new GameObject("morpher").AddComponent<MockOnMorph>();
		var seed = new GameObject("target");

		yield return new WaitForEndOfFrame();

		onMorph.onFailSeed.AddListener(s => called = s);
		onMorph.Morph(seed);

		Assert.AreSame(seed, called);
	}

	[UnityTest]
	public IEnumerator OnSuccessMorph()
	{
		var called = default(MockComponent);
		var onMorph = new GameObject("morpher").AddComponent<MockOnMorph>();
		var seed = new GameObject("target").AddComponent<MockComponent>();

		yield return new WaitForEndOfFrame();

		onMorph.onSuccessMorph.AddListener(m => called = m);
		onMorph.Morph(seed.gameObject);

		Assert.AreSame(seed, called);
	}

	[UnityTest]
	public IEnumerator NoSuccessCallbacks()
	{
		var called = 0;
		var onMorph = new GameObject("morpher").AddComponent<MockOnMorph>();
		var seed = new GameObject("target");

		yield return new WaitForEndOfFrame();

		onMorph.onSuccessMorph.AddListener(_ => ++called);
		onMorph.onSuccessSeed.AddListener(_ => ++called);
		onMorph.Morph(seed);

		Assert.AreEqual(0, called);
	}

	[UnityTest]
	public IEnumerator NoFailCallback()
	{
		var called = 0;
		var onMorph = new GameObject("morpher").AddComponent<MockOnMorph>();
		var seed = new GameObject("target").AddComponent<MockComponent>().gameObject;

		yield return new WaitForEndOfFrame();

		onMorph.onFailSeed.AddListener(_ => ++called);
		onMorph.Morph(seed);

		Assert.AreEqual(0, called);
	}
}
