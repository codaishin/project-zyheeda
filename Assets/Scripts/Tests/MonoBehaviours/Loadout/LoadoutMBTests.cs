using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Routines;
using UnityEngine;
using UnityEngine.TestTools;

public class LoadoutMBTests : TestCollection
{
	class MockRoutine : IRoutine
	{
		public Action release =
			() => { };
		public Func<IEnumerator<YieldInstruction?>> getEnumerator =
			() =>
				Enumerable
					.Empty<YieldInstruction>()
					.GetEnumerator();

		public IEnumerator<YieldInstruction?> GetEnumerator() =>
			this.getEnumerator();
		IEnumerator IEnumerable.GetEnumerator() =>
			this.getEnumerator();
		public void NextSubRoutine() =>
			this.release();
	}

	class MockItemMB : MonoBehaviour, IItem
	{
		public Action<IAnimationStance, Transform> equip = (_, __) => { };
		public Action unEquip = () => { };
		public Func<GameObject, Func<IRoutine?>> getRoutineFnFor = _ => () => null;

		public void Equip(IAnimationStance animator, Transform slot) =>
			this.equip(animator, slot);
		public void UnEquip() =>
			this.unEquip();
		public Func<IRoutine?> GetRoutineFnFor(GameObject agent) =>
			this.getRoutineFnFor(agent);
	}

	[UnityTest]
	public IEnumerator CircleFirstOnStart() {
		var loadout = new GameObject().AddComponent<LoadoutMB>();
		var item = new GameObject().AddComponent<MockItemMB>();
		var slot = new GameObject();
		var animator = new GameObject().AddComponent<AnimationMB>();
		var calledAnimator = null as object;
		var calledSlot = null as Transform;

		loadout.slot = slot.transform;
		loadout.animator = animator;
		loadout.items = new Reference<IItem>[] {
			Reference<IItem>.Component(item),
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
		var items = new MockItemMB[] {
			new GameObject().AddComponent<MockItemMB>(),
			new GameObject().AddComponent<MockItemMB>(),
		};
		var slot = new GameObject();
		var animator = new GameObject().AddComponent<AnimationMB>();
		var calledAnimator = null as object;
		var calledSlot = null as Transform;
		var calledUnEquip = 0;

		loadout.slot = slot.transform;
		loadout.animator = animator;
		loadout.items = items.Select(Reference<IItem>.Component).ToArray();
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
		var items = new MockItemMB[] {
			new GameObject().AddComponent<MockItemMB>(),
			new GameObject().AddComponent<MockItemMB>(),
			new GameObject().AddComponent<MockItemMB>(),
		};
		var slot = new GameObject();
		var animator = new GameObject().AddComponent<AnimationMB>();
		var calledAnimator = null as object;
		var calledSlot = null as Transform;
		var calledUnEquip = 0;

		loadout.slot = slot.transform;
		loadout.animator = animator;
		loadout.items = items.Select(Reference<IItem>.Component).ToArray();
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
		var items = new MockItemMB[] {
			new GameObject().AddComponent<MockItemMB>(),
			new GameObject().AddComponent<MockItemMB>(),
			new GameObject().AddComponent<MockItemMB>(),
		};
		var slot = new GameObject();
		var animator = new GameObject().AddComponent<AnimationMB>();
		var calledAnimator = null as object;
		var calledSlot = null as Transform;
		var calledUnEquip = 0;

		loadout.slot = slot.transform;
		loadout.animator = animator;
		loadout.items = items.Select(Reference<IItem>.Component).ToArray();

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
		var items = new MockItemMB[] {
			new GameObject().AddComponent<MockItemMB>(),
		};
		var slot = new GameObject();
		var animator = new GameObject().AddComponent<AnimationMB>();
		var runner = new GameObject().AddComponent<RoutineRunMB>();
		var calledAgent = null as GameObject;
		var agent = new GameObject();

		loadout.slot = slot.transform;
		loadout.animator = animator;
		loadout.items = items.Select(Reference<IItem>.Component).ToArray();

		yield return new WaitForEndOfFrame();

		var count = 0;
		IRoutine getInstruction() {
			IEnumerator<YieldInstruction?> getEnumerator() {
				for (; count < 10; ++count) {
					yield return null;
				}
			}
			return new MockRoutine { getEnumerator = getEnumerator };
		}

		items[0].getRoutineFnFor = agent => {
			calledAgent = agent;
			return getInstruction;
		};

		var routineFn = loadout.GetRoutineFnFor(agent)!;

		foreach (var _ in routineFn().OrEmpty()) ;

		Assert.AreSame(agent, calledAgent);
		Assert.AreEqual(10, count);
	}

	[UnityTest]
	public IEnumerator Use3rd() {
		var loadout = new GameObject().AddComponent<LoadoutMB>();
		var items = new MockItemMB[] {
			new GameObject().AddComponent<MockItemMB>(),
			new GameObject().AddComponent<MockItemMB>(),
			new GameObject().AddComponent<MockItemMB>(),
			new GameObject().AddComponent<MockItemMB>(),
		};
		var slot = new GameObject();
		var animator = new GameObject().AddComponent<AnimationMB>();
		var runner = new GameObject().AddComponent<RoutineRunMB>();
		var calledAgent = null as GameObject;
		var agent = new GameObject();

		loadout.slot = slot.transform;
		loadout.animator = animator;
		loadout.items = items.Select(Reference<IItem>.Component).ToArray();

		yield return new WaitForEndOfFrame();

		var count = 0;
		IRoutine getInstruction() {
			IEnumerator<YieldInstruction?> getEnumerator() {
				for (; count < 10; ++count) {
					yield return null;
				}
			}
			return new MockRoutine { getEnumerator = getEnumerator };
		}

		items[2].getRoutineFnFor = agent => {
			calledAgent = agent;
			return getInstruction;
		};

		loadout.Circle();
		loadout.Circle();

		var routineFn = loadout.GetRoutineFnFor(agent)!;

		foreach (var _ in routineFn().OrEmpty()) ;

		Assert.AreSame(agent, calledAgent);
		Assert.AreEqual(10, count);
	}

	[UnityTest]
	public IEnumerator CircleInstructions() {
		var loadout = new GameObject().AddComponent<LoadoutMB>();
		var items = new MockItemMB[] {
			new GameObject().AddComponent<MockItemMB>(),
			new GameObject().AddComponent<MockItemMB>(),
		};
		var slot = new GameObject();
		var animator = new GameObject().AddComponent<AnimationMB>();
		var runner = new GameObject().AddComponent<RoutineRunMB>();
		var calledA = 0;
		var calledB = 0;
		var agent = new GameObject();

		loadout.slot = slot.transform;
		loadout.animator = animator;
		loadout.items = items.Select(Reference<IItem>.Component).ToArray();

		items[0].getRoutineFnFor = agent => {
			++calledA;
			return () => null;
		};
		items[1].getRoutineFnFor = agent => {
			++calledB;
			return () => null;
		};

		yield return new WaitForEndOfFrame();

		var routineFn = loadout.GetRoutineFnFor(agent)!;

		_ = routineFn();

		loadout.Circle();

		_ = routineFn();

		Assert.AreEqual((1, 1), (calledA, calledB));
	}

	[UnityTest]
	public IEnumerator CacheInstructions() {
		var loadout = new GameObject().AddComponent<LoadoutMB>();
		var items = new MockItemMB[] {
			new GameObject().AddComponent<MockItemMB>(),
			new GameObject().AddComponent<MockItemMB>(),
		};
		var slot = new GameObject();
		var animator = new GameObject().AddComponent<AnimationMB>();
		var runner = new GameObject().AddComponent<RoutineRunMB>();
		var called = 0;
		var agent = new GameObject();

		loadout.slot = slot.transform;
		loadout.animator = animator;
		loadout.items = items.Select(Reference<IItem>.Component).ToArray();

		items[0].getRoutineFnFor = agent => {
			++called;
			return () => null;
		};

		yield return new WaitForEndOfFrame();

		var routineFn = loadout.GetRoutineFnFor(agent)!;

		_ = routineFn();
		_ = routineFn();

		Assert.AreEqual(1, called);
	}

	[UnityTest]
	public IEnumerator AllowEmptyItems() {
		var loadout = new GameObject().AddComponent<LoadoutMB>();
		var items = new MockItemMB?[] { null, null };
		var slot = new GameObject();
		var animator = new GameObject().AddComponent<AnimationMB>();
		var runner = new GameObject().AddComponent<RoutineRunMB>();
		var agent = new GameObject();

		loadout.slot = slot.transform;
		loadout.animator = animator;
		loadout.items = items
			.Select(
				item =>
					item != null
						? Reference<IItem>.Component(item)
						: new Reference<IItem>())
			.ToArray();

		yield return new WaitForEndOfFrame();

		Assert.DoesNotThrow(() => loadout.Circle());
		Assert.DoesNotThrow(() => loadout.GetRoutineFnFor(agent));
	}
}
