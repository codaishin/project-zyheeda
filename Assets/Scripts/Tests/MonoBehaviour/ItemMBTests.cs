using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ItemMBTests : TestCollection
{
	[UnityTest]
	public IEnumerator GetStance() {
		var item = new GameObject().AddComponent<ItemMB>();
		item.idleStance = Animation.Stance.HoldRifle;

		yield return new WaitForEndOfFrame();

		Assert.AreEqual(item.idleStance, item.IdleStance);
	}
	[UnityTest]
	public IEnumerator GetState() {
		var item = new GameObject().AddComponent<ItemMB>();
		item.activeState = Animation.State.ShootRifle;

		yield return new WaitForEndOfFrame();

		Assert.AreEqual(item.activeState, item.ActiveState);
	}
}
