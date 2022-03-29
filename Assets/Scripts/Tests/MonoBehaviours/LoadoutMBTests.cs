using System;
using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class LoadoutMBTests : TestCollection
{
	class MockItemHandleMB : MonoBehaviour, IItemHandle
	{
		public Action<object, Transform> equip =
			(_, ___) => { };
		public Action unEquip =
			() => { };
		public Action use =
			() => { };
		public Action stopUsing =
			() => { };

		public void Equip<TAnimator>(TAnimator animator, Transform slot)
			where TAnimator : IAnimationStance, IAnimationStates =>
			this.equip(animator, slot);
		public void UnEquip() =>
			this.unEquip();
		public void Use() =>
			this.use();
		public void StopUsing() =>
			this.stopUsing();
	}

	[UnityTest]
	public IEnumerator CircleFirstOnStart() {
		var loadout = new GameObject().AddComponent<LoadoutMB>();
		var item = new GameObject().AddComponent<MockItemHandleMB>();
		var slot = new GameObject();
		var animator = new GameObject().AddComponent<AnimationMB>();
		var calledAnimator = null as object;
		var calledSlot = null as Transform;

		loadout.slot = slot.transform;
		loadout.animator = animator;
		loadout.items = new Reference<IItemHandle>[] {
			Reference<IItemHandle>.Component(item),
		};
		item.equip = (a, s) => {
			calledAnimator = a;
			calledSlot = s;
		};

		yield return new WaitForEndOfFrame();

		Assert.AreSame(animator, calledAnimator);
		Assert.AreSame(slot.transform, calledSlot);
	}

	[UnityTest]
	public IEnumerator Circle2nd() {
		var loadout = new GameObject().AddComponent<LoadoutMB>();
		var items = new MockItemHandleMB[] {
			new GameObject().AddComponent<MockItemHandleMB>(),
			new GameObject().AddComponent<MockItemHandleMB>(),
		};
		var slot = new GameObject();
		var animator = new GameObject().AddComponent<AnimationMB>();
		var calledAnimator = null as object;
		var calledSlot = null as Transform;
		var calledUnEquip = 0;

		loadout.slot = slot.transform;
		loadout.animator = animator;
		loadout.items = items.Select(Reference<IItemHandle>.Component).ToArray();
		items[0].unEquip = () => ++calledUnEquip;
		items[1].equip = (a, s) => {
			calledAnimator = a;
			calledSlot = s;
		};

		yield return new WaitForEndOfFrame();

		loadout.Circle();

		Assert.AreEqual(1, calledUnEquip);
		Assert.AreSame(animator, calledAnimator);
		Assert.AreSame(slot.transform, calledSlot);
	}

	[UnityTest]
	public IEnumerator Circle3rd() {
		var loadout = new GameObject().AddComponent<LoadoutMB>();
		var items = new MockItemHandleMB[] {
			new GameObject().AddComponent<MockItemHandleMB>(),
			new GameObject().AddComponent<MockItemHandleMB>(),
			new GameObject().AddComponent<MockItemHandleMB>(),
		};
		var slot = new GameObject();
		var animator = new GameObject().AddComponent<AnimationMB>();
		var calledAnimator = null as object;
		var calledSlot = null as Transform;
		var calledUnEquip = 0;

		loadout.slot = slot.transform;
		loadout.animator = animator;
		loadout.items = items.Select(Reference<IItemHandle>.Component).ToArray();
		items[1].unEquip = () => ++calledUnEquip;
		items[2].equip = (a, s) => {
			calledAnimator = a;
			calledSlot = s;
		};

		yield return new WaitForEndOfFrame();

		loadout.Circle();
		loadout.Circle();

		Assert.AreEqual(1, calledUnEquip);
		Assert.AreSame(animator, calledAnimator);
		Assert.AreSame(slot.transform, calledSlot);
	}

	[UnityTest]
	public IEnumerator CircleBackTo1st() {
		var loadout = new GameObject().AddComponent<LoadoutMB>();
		var items = new MockItemHandleMB[] {
			new GameObject().AddComponent<MockItemHandleMB>(),
			new GameObject().AddComponent<MockItemHandleMB>(),
			new GameObject().AddComponent<MockItemHandleMB>(),
		};
		var slot = new GameObject();
		var animator = new GameObject().AddComponent<AnimationMB>();
		var calledAnimator = null as object;
		var calledSlot = null as Transform;
		var calledUnEquip = 0;

		loadout.slot = slot.transform;
		loadout.animator = animator;
		loadout.items = items.Select(Reference<IItemHandle>.Component).ToArray();

		yield return new WaitForEndOfFrame();

		items[2].unEquip = () => ++calledUnEquip;
		items[0].equip = (a, s) => {
			calledAnimator = a;
			calledSlot = s;
		};

		loadout.Circle();
		loadout.Circle();
		loadout.Circle();

		Assert.AreEqual(1, calledUnEquip);
		Assert.AreSame(animator, calledAnimator);
		Assert.AreSame(slot.transform, calledSlot);
	}

	[UnityTest]
	public IEnumerator Use1st() {
		var loadout = new GameObject().AddComponent<LoadoutMB>();
		var items = new MockItemHandleMB[] {
			new GameObject().AddComponent<MockItemHandleMB>(),
		};
		var slot = new GameObject();
		var animator = new GameObject().AddComponent<AnimationMB>();
		var calledUse = 0;

		loadout.slot = slot.transform;
		loadout.animator = animator;
		loadout.items = items.Select(Reference<IItemHandle>.Component).ToArray();

		yield return new WaitForEndOfFrame();

		items[0].use = () => ++calledUse;

		loadout.Use();

		Assert.AreEqual(1, calledUse);
	}

	[UnityTest]
	public IEnumerator Use3rd() {
		var loadout = new GameObject().AddComponent<LoadoutMB>();
		var items = new MockItemHandleMB[] {
			new GameObject().AddComponent<MockItemHandleMB>(),
			new GameObject().AddComponent<MockItemHandleMB>(),
			new GameObject().AddComponent<MockItemHandleMB>(),
			new GameObject().AddComponent<MockItemHandleMB>(),
		};
		var slot = new GameObject();
		var animator = new GameObject().AddComponent<AnimationMB>();
		var calledUse = 0;

		loadout.slot = slot.transform;
		loadout.animator = animator;
		loadout.items = items.Select(Reference<IItemHandle>.Component).ToArray();

		yield return new WaitForEndOfFrame();

		loadout.Circle();
		loadout.Circle();

		items[2].use = () => ++calledUse;

		loadout.Use();

		Assert.AreEqual(1, calledUse);
	}

	[UnityTest]
	public IEnumerator AllowEmptyItems() {
		var loadout = new GameObject().AddComponent<LoadoutMB>();
		var items = new MockItemHandleMB?[] { null, null };
		var slot = new GameObject();
		var animator = new GameObject().AddComponent<AnimationMB>();

		loadout.slot = slot.transform;
		loadout.animator = animator;
		loadout.items = items
			.Select(
				item =>
					item != null
						? Reference<IItemHandle>.Component(item)
						: new Reference<IItemHandle>())
			.ToArray();

		yield return new WaitForEndOfFrame();

		Assert.DoesNotThrow(() => loadout.Circle());
		Assert.DoesNotThrow(() => loadout.Use());
	}
}
