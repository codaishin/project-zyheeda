using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class LoadoutMBTests : TestCollection
{
	class MockStanceAnimator : MonoBehaviour, IStanceAnimation
	{
		public Action<Stance, float> set = (_, __) => { };
		public void Set(Stance layer, float weight) =>
			this.set(layer, weight);
	}

	[UnityTest]
	public IEnumerator Enable() {
		var rifle = new GameObject();
		var slot = new GameObject();
		var set = new GameObject().AddComponent<LoadoutMB>();

		set.weapon = rifle.transform;
		rifle.SetActive(false);

		yield return new WaitForEndOfFrame();

		set.Equip(slot.transform);

		Assert.True(rifle.activeSelf);
	}
	[UnityTest]
	public IEnumerator Disable() {
		var rifle = new GameObject();
		var slot = new GameObject();
		var set = new GameObject().AddComponent<LoadoutMB>();

		set.weapon = rifle.transform;
		rifle.SetActive(false);

		yield return new WaitForEndOfFrame();

		set.Equip(slot.transform);
		set.UnEquip();

		Assert.False(rifle.activeSelf);
	}

	[UnityTest]
	public IEnumerator ChildToSlot() {
		var rifle = new GameObject();
		var slot = new GameObject();
		var set = new GameObject().AddComponent<LoadoutMB>();

		set.weapon = rifle.transform;

		yield return new WaitForEndOfFrame();

		set.Equip(slot.transform);

		Assert.AreSame(slot.transform, rifle.transform.parent);
	}

	[UnityTest]
	public IEnumerator ChildToSlotPositionAndRotation() {
		var rifle = new GameObject();
		var slot = new GameObject();
		var set = new GameObject().AddComponent<LoadoutMB>();

		set.weapon = rifle.transform;
		slot.transform.forward = Vector3.down;
		slot.transform.position = Vector3.up;

		yield return new WaitForEndOfFrame();

		set.Equip(slot.transform);

		Tools.AssertEqual(Vector3.down, rifle.transform.forward);
		Assert.AreEqual(Vector3.up, rifle.transform.position);
	}

	[UnityTest]
	public IEnumerator ChildToSlotRetainLossyScale() {
		var rifle = new GameObject();
		var slot = new GameObject();
		var set = new GameObject().AddComponent<LoadoutMB>();

		set.weapon = rifle.transform;
		slot.transform.localScale *= 0.1f;
		rifle.transform.localScale = new Vector3(1, 2, 3);

		yield return new WaitForEndOfFrame();

		set.Equip(slot.transform);

		Assert.AreEqual(new Vector3(1, 2, 3), rifle.transform.lossyScale);
	}

	[UnityTest]
	public IEnumerator NullChildToSlotDoesnThrow() {
		var slot = new GameObject();
		var set = new GameObject().AddComponent<LoadoutMB>();

		set.weapon = null;

		yield return new WaitForEndOfFrame();

		Assert.DoesNotThrow(() => set.Equip(slot.transform));
	}

	[UnityTest]
	public IEnumerator ResetParent() {
		var parent = new GameObject();
		var rifle = new GameObject();
		var slot = new GameObject();
		var set = new GameObject().AddComponent<LoadoutMB>();

		rifle.transform.parent = parent.transform;
		set.weapon = rifle.transform;

		yield return new WaitForEndOfFrame();

		set.Equip(slot.transform);
		set.UnEquip();

		Assert.AreSame(parent.transform, rifle.transform.parent);
	}


	[UnityTest]
	public IEnumerator ResetPositionAndRotation() {
		var parent = new GameObject();
		var rifle = new GameObject();
		var slot = new GameObject();
		var set = new GameObject().AddComponent<LoadoutMB>();

		set.weapon = rifle.transform;
		rifle.transform.parent = parent.transform;
		parent.transform.forward = Vector3.down;
		parent.transform.position = Vector3.up;

		yield return new WaitForEndOfFrame();

		set.Equip(slot.transform);
		set.UnEquip();

		Tools.AssertEqual(Vector3.down, rifle.transform.forward);
		Assert.AreEqual(Vector3.up, rifle.transform.position);
	}

	[UnityTest]
	public IEnumerator ResetRetainLossyScale() {
		var parent = new GameObject();
		var rifle = new GameObject();
		var slot = new GameObject();
		var set = new GameObject().AddComponent<LoadoutMB>();

		set.weapon = rifle.transform;
		rifle.transform.parent = parent.transform;
		slot.transform.localScale *= 0.1f;
		rifle.transform.localScale = new Vector3(1, 2, 3);

		yield return new WaitForEndOfFrame();

		set.Equip(slot.transform);
		set.UnEquip();

		Assert.AreEqual(new Vector3(1, 2, 3), rifle.transform.lossyScale);
	}

	[UnityTest]
	public IEnumerator NullResetDoesnThrow() {
		var slot = new GameObject();
		var set = new GameObject().AddComponent<LoadoutMB>();

		set.weapon = null;

		yield return new WaitForEndOfFrame();

		set.Equip(slot.transform);

		Assert.DoesNotThrow(() => set.UnEquip());
	}

	[UnityTest]
	public IEnumerator ResetParentNull() {
		var slot = new GameObject();
		var rifle = new GameObject();
		var set = new GameObject().AddComponent<LoadoutMB>();

		set.weapon = rifle.transform;

		yield return new WaitForEndOfFrame();

		set.Equip(slot.transform);
		set.UnEquip();

		Assert.Null(rifle.transform.parent);
	}

	[UnityTest]
	public IEnumerator SetStanceWeightOne() {
		var called = (Stance.Default, 0f);
		var slot = new GameObject();
		var rifle = new GameObject();
		var set = new GameObject().AddComponent<LoadoutMB>();
		var animationLayer = new GameObject().AddComponent<MockStanceAnimator>();

		set.stance = Stance.HoldRifle;
		set.weapon = rifle.transform;
		set.SetStanceAnimator(animationLayer);
		animationLayer.set = (l, w) => called = (l, w);

		yield return new WaitForEndOfFrame();

		set.Equip(slot.transform);

		Assert.AreEqual((Stance.HoldRifle, 1f), called);
	}

	[UnityTest]
	public IEnumerator SetStanceWeightZero() {
		var called = (Stance.Default, 1f);
		var slot = new GameObject();
		var rifle = new GameObject();
		var set = new GameObject().AddComponent<LoadoutMB>();
		var animationLayer = new GameObject().AddComponent<MockStanceAnimator>();

		set.stance = Stance.HoldRifle;
		set.weapon = rifle.transform;
		set.SetStanceAnimator(animationLayer);

		yield return new WaitForEndOfFrame();

		set.Equip(slot.transform);
		animationLayer.set = (l, w) => called = (l, w);
		set.UnEquip();

		Assert.AreEqual((Stance.HoldRifle, 0f), called);
	}
}
