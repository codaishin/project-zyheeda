using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BaseMorphMBTests : TestCollection
{
	private class MockMorph : BaseMorphMB<GameObject, Transform>
	{
		public override Transform DoMorph(GameObject seed) => seed.transform;
	}

	[Test]
	public void OnMorphNull()
	{
		var morph = new GameObject("morph").AddComponent<MockMorph>();

		Assert.Null(morph.onMorph);
	}

	[UnityTest]
	public IEnumerator InitAfterStart()
	{
		var morph = new GameObject("morph").AddComponent<MockMorph>();

		yield return new WaitForEndOfFrame();

		Assert.NotNull(morph.onMorph);
	}

	[UnityTest]
	public IEnumerator NotInitWhenNotNull()
	{
		var morph = new GameObject("morph").AddComponent<MockMorph>();
		var onMorph = new MockMorph.MorphEvent();
		morph.onMorph = onMorph;

		yield return new WaitForEndOfFrame();

		Assert.AreSame(onMorph, morph.onMorph);
	}

	[UnityTest]
	public IEnumerator OnMorph()
	{
		var called = default(Transform);
		var morph = new GameObject("morph").AddComponent<MockMorph>();
		var obj = new GameObject("obj");


		yield return new WaitForEndOfFrame();

		morph.onMorph.AddListener(t => called = t);
		morph.Morph(obj);

		Assert.AreSame(called, obj.transform);
	}
}
