using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ItemHandleMBTests : TestCollection
{
	private class MockAnimator : IAnimationStates
	{
		public Animation.State state;
		public void Set(Animation.State state) => this.state = state;
	}

	[UnityTest]
	public IEnumerator SetAnimatorState() {
		var item = new GameObject().AddComponent<ItemHandleMB>();
		var animator = new MockAnimator();

		yield return new WaitForEndOfFrame();

		item.Equip(animator, new GameObject().transform);
		item.Set(Animation.State.WalkOrRun);

		Assert.AreEqual(Animation.State.WalkOrRun, animator.state);
	}

	[UnityTest]
	public IEnumerator SetSlot() {
		var item = new GameObject().AddComponent<ItemHandleMB>();
		var slot = new GameObject().transform;

		yield return new WaitForEndOfFrame();

		item.Equip(new MockAnimator(), slot);

		Assert.AreSame(slot, item.transform.parent);
	}

	[UnityTest]
	public IEnumerator SetSlotPositionAndRotation() {
		var item = new GameObject().AddComponent<ItemHandleMB>();
		var slot = new GameObject().transform;

		slot.position = Vector3.up;
		slot.forward = Vector3.down;

		yield return new WaitForEndOfFrame();

		item.Equip(new MockAnimator(), slot);

		Assert.AreEqual(Vector3.up, item.transform.position);
		Tools.AssertEqual(Vector3.down, item.transform.forward);
	}

	[UnityTest]
	public IEnumerator SetSlotRetainLossyScale() {
		var item = new GameObject().AddComponent<ItemHandleMB>();
		var slot = new GameObject().transform;

		slot.localScale *= 0.1f;
		item.transform.localScale *= 10f;

		yield return new WaitForEndOfFrame();

		item.Equip(new MockAnimator(), slot);

		Assert.AreEqual(Vector3.one * 10f, item.transform.lossyScale);
	}

	[UnityTest]
	public IEnumerator ResetParent() {
		var item = new GameObject().AddComponent<ItemHandleMB>();
		var slot = new GameObject().transform;

		yield return new WaitForEndOfFrame();

		item.Equip(new MockAnimator(), slot);
		item.UnEquip();

		Assert.Null(item.transform.parent);
	}

	[UnityTest]
	public IEnumerator ResetOldParent() {
		var parent = new GameObject();
		var item = new GameObject().AddComponent<ItemHandleMB>();
		var slot = new GameObject().transform;

		item.transform.parent = parent.transform;

		yield return new WaitForEndOfFrame();

		item.Equip(new MockAnimator(), slot);
		item.UnEquip();

		Assert.AreSame(parent.transform, item.transform.parent);
	}
}
