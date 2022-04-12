using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ItemActionTemplateMBTests : TestCollection
{
	private class MockAnimateStates : IAnimationStates, IAnimationStance
	{
		public Action<Animation.State> setState = _ => { };
		public Action<Animation.Stance, bool> setStance = (_, __) => { };

		public void Set(Animation.State state) =>
			this.setState(state);
		public void Set(Animation.Stance stance, bool value) =>
			this.setStance(stance, value);
	}

	[UnityTest]
	public IEnumerator SetAnimatorStance() {
		var item = new GameObject().AddComponent<ItemActionTemplateMB>();
		var animator = new MockAnimateStates();
		var called = ((Animation.Stance)(-1), false);

		item.idleStance = Animation.Stance.HoldRifle;
		animator.setStance = (s, v) => called = (s, v);

		yield return new WaitForEndOfFrame();

		item.Equip(animator, new GameObject().transform);

		Assert.AreEqual((Animation.Stance.HoldRifle, true), called);
	}

	[UnityTest]
	public IEnumerator UnsetAnimatorStance() {
		var item = new GameObject().AddComponent<ItemActionTemplateMB>();
		var animator = new MockAnimateStates();
		var called = ((Animation.Stance)(-1), true);

		item.idleStance = Animation.Stance.HoldRifle;

		yield return new WaitForEndOfFrame();

		item.Equip(animator, new GameObject().transform);

		animator.setStance = (s, v) => called = (s, v);

		item.UnEquip();

		Assert.AreEqual((Animation.Stance.HoldRifle, false), called);
	}

	[UnityTest]
	public IEnumerator SetSlot() {
		var item = new GameObject().AddComponent<ItemActionTemplateMB>();
		var slot = new GameObject().transform;

		yield return new WaitForEndOfFrame();

		item.Equip(new MockAnimateStates(), slot);

		Assert.AreSame(slot, item.transform.parent);
	}

	[UnityTest]
	public IEnumerator Enable() {
		var item = new GameObject().AddComponent<ItemActionTemplateMB>();
		var slot = new GameObject().transform;

		item.gameObject.SetActive(false);

		yield return new WaitForEndOfFrame();

		item.Equip(new MockAnimateStates(), slot);

		Assert.True(item.gameObject.activeSelf);
	}

	[UnityTest]
	public IEnumerator SetSlotPositionAndRotation() {
		var item = new GameObject().AddComponent<ItemActionTemplateMB>();
		var slot = new GameObject().transform;

		slot.position = Vector3.up;
		slot.forward = Vector3.down;

		yield return new WaitForEndOfFrame();

		item.Equip(new MockAnimateStates(), slot);

		Assert.AreEqual(Vector3.up, item.transform.position);
		Tools.AssertEqual(Vector3.down, item.transform.forward);
	}

	[UnityTest]
	public IEnumerator SetSlotRetainLossyScale() {
		var item = new GameObject().AddComponent<ItemActionTemplateMB>();
		var slot = new GameObject().transform;

		slot.localScale *= 0.1f;
		item.transform.localScale *= 10f;

		yield return new WaitForEndOfFrame();

		item.Equip(new MockAnimateStates(), slot);

		Assert.AreEqual(Vector3.one * 10f, item.transform.lossyScale);
	}

	[UnityTest]
	public IEnumerator ResetParent() {
		var item = new GameObject().AddComponent<ItemActionTemplateMB>();
		var slot = new GameObject().transform;

		yield return new WaitForEndOfFrame();

		item.Equip(new MockAnimateStates(), slot);
		item.UnEquip();

		Assert.Null(item.transform.parent);
	}

	[UnityTest]
	public IEnumerator Disable() {
		var item = new GameObject().AddComponent<ItemActionTemplateMB>();
		var slot = new GameObject().transform;

		yield return new WaitForEndOfFrame();

		item.Equip(new MockAnimateStates(), slot);
		item.UnEquip();

		Assert.False(item.gameObject.activeSelf);
	}

	[UnityTest]
	public IEnumerator ResetOldParent() {
		var parent = new GameObject();
		var item = new GameObject().AddComponent<ItemActionTemplateMB>();
		var slot = new GameObject().transform;

		item.transform.parent = parent.transform;

		yield return new WaitForEndOfFrame();

		item.Equip(new MockAnimateStates(), slot);
		item.UnEquip();

		Assert.AreSame(parent.transform, item.transform.parent);
	}
}
