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

	[UnityTest]
	public IEnumerator GetUseAfterSeconds() {
		var item = new GameObject().AddComponent<ItemMB>();
		item.useAfterSeconds = 4f;

		yield return new WaitForEndOfFrame();

		Assert.AreEqual(item.useAfterSeconds, item.UseAfterSeconds);
	}

	[UnityTest]
	public IEnumerator GetLeaveActiveStateAfterSeconds() {
		var item = new GameObject().AddComponent<ItemMB>();
		item.resetAfterSeconds = 3f;

		yield return new WaitForEndOfFrame();

		Assert.AreEqual(
			item.resetAfterSeconds,
			item.LeaveActiveStateAfterSeconds
		);
	}
}
