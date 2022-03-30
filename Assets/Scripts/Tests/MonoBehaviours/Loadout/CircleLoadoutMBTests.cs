using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class CircleLoadoutMBTests : TestCollection
{
	class MockLoadoutMB : MonoBehaviour, ILoadout
	{
		public Action circle = () => { };
		public void Circle() => this.circle();
	}

	[UnityTest]
	public IEnumerator CircleLoadout() {
		var loadout = new GameObject().AddComponent<MockLoadoutMB>();
		var circler = new GameObject().AddComponent<CircleLoadoutMB>();
		var called = 0;

		loadout.circle = () => ++called;
		circler.loadout = Reference<ILoadout>.Component(loadout);

		yield return new WaitForEndOfFrame();

		circler.Apply();
		circler.Apply();
		circler.Apply();

		Assert.AreEqual(3, called);
	}
}
