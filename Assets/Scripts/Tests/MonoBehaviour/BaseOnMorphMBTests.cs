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
}
