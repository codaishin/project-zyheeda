using System;
using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class LoadoutCircleMBTests : TestCollection
{
	class MockLoadoutMB : MonoBehaviour, ILoadout
	{
		public Action<Transform> assignTo = _ => { };
		public Action reset = () => { };
		public Action<IStanceAnimation> setAnimationLayer = _ => { };

		public void Equip(Transform slot) =>
			this.assignTo(slot);
		public void UnEquip() =>
			this.reset();
		public void SetStanceAnimator(IStanceAnimation animationLayer) =>
			this.setAnimationLayer(animationLayer);
	}

	class MockStanceAnimatorMB : MonoBehaviour, IStanceAnimation
	{
		public void Set(Stance layer, float weight) {
			throw new NotImplementedException();
		}
	}

	[UnityTest]
	public IEnumerator FirstOnStart() {
		var called = null as Transform;
		var set = new GameObject().AddComponent<MockLoadoutMB>();
		var slot = new GameObject();
		var loadout = new GameObject().AddComponent<LoadoutCircleMB>();
		loadout.slot = slot.transform;
		loadout.loadouts = new Reference<ILoadout>[] {
			Reference<ILoadout>.PointToComponent(set),
		};

		set.assignTo = s => called = s;

		yield return new WaitForEndOfFrame();

		Assert.AreSame(slot.transform, called);
	}

	[UnityTest]
	public IEnumerator ApplySnd() {
		var called = null as Transform;
		var loadouts = new MockLoadoutMB[] {
			new GameObject().AddComponent<MockLoadoutMB>(),
			new GameObject().AddComponent<MockLoadoutMB>(),
		};
		var slot = new GameObject();
		var loadout = new GameObject().AddComponent<LoadoutCircleMB>();
		loadout.slot = slot.transform;
		loadout.loadouts = loadouts
			.Select(Reference<ILoadout>.PointToComponent)
			.ToArray();

		loadouts[1].assignTo = s => called = s;

		yield return new WaitForEndOfFrame();

		loadout.Apply();

		Assert.AreSame(slot.transform, called);
	}

	[UnityTest]
	public IEnumerator ResetFistWhenApplyingSnd() {
		var called = 0;
		var loadouts = new MockLoadoutMB[] {
			new GameObject().AddComponent<MockLoadoutMB>(),
			new GameObject().AddComponent<MockLoadoutMB>(),
		};
		var slot = new GameObject();
		var loadout = new GameObject().AddComponent<LoadoutCircleMB>();
		loadout.slot = slot.transform;
		loadout.loadouts = loadouts
			.Select(Reference<ILoadout>.PointToComponent)
			.ToArray();

		loadouts[0].reset = () => ++called;

		yield return new WaitForEndOfFrame();

		loadout.Apply();

		Assert.AreEqual(1, called);
	}

	[UnityTest]
	public IEnumerator Apply4th() {
		var called = null as Transform;
		var loadouts = new MockLoadoutMB[] {
			new GameObject().AddComponent<MockLoadoutMB>(),
			new GameObject().AddComponent<MockLoadoutMB>(),
			new GameObject().AddComponent<MockLoadoutMB>(),
			new GameObject().AddComponent<MockLoadoutMB>(),
		};
		var slot = new GameObject();
		var loadout = new GameObject().AddComponent<LoadoutCircleMB>();
		loadout.slot = slot.transform;
		loadout.loadouts = loadouts
			.Select(Reference<ILoadout>.PointToComponent)
			.ToArray();

		loadouts[3].assignTo = s => called = s;

		yield return new WaitForEndOfFrame();

		loadout.Apply();
		loadout.Apply();
		loadout.Apply();

		Assert.AreSame(slot.transform, called);
	}

	[UnityTest]
	public IEnumerator Reset3rdWhenApplying4th() {
		var called = 0;
		var loadouts = new MockLoadoutMB[] {
			new GameObject().AddComponent<MockLoadoutMB>(),
			new GameObject().AddComponent<MockLoadoutMB>(),
			new GameObject().AddComponent<MockLoadoutMB>(),
			new GameObject().AddComponent<MockLoadoutMB>(),
		};
		var slot = new GameObject();
		var loadout = new GameObject().AddComponent<LoadoutCircleMB>();
		loadout.slot = slot.transform;
		loadout.loadouts = loadouts
			.Select(Reference<ILoadout>.PointToComponent)
			.ToArray();

		loadouts[2].reset = () => ++called;

		yield return new WaitForEndOfFrame();

		loadout.Apply();
		loadout.Apply();
		loadout.Apply();

		Assert.AreEqual(1, called);
	}

	[UnityTest]
	public IEnumerator ApplyFirstAgain() {
		var called = null as Transform;
		var loadouts = new MockLoadoutMB[] {
			new GameObject().AddComponent<MockLoadoutMB>(),
			new GameObject().AddComponent<MockLoadoutMB>(),
			new GameObject().AddComponent<MockLoadoutMB>(),
		};
		var slot = new GameObject();
		var loadout = new GameObject().AddComponent<LoadoutCircleMB>();
		loadout.slot = slot.transform;
		loadout.loadouts = loadouts
			.Select(Reference<ILoadout>.PointToComponent)
			.ToArray();

		loadouts[0].assignTo = s => called = s;

		yield return new WaitForEndOfFrame();

		loadout.Apply();
		loadout.Apply();
		loadout.Apply();

		Assert.AreSame(slot.transform, called);
	}

	[UnityTest]
	public IEnumerator loadoutstanceAnimator() {
		var called = null as IStanceAnimation;
		var animation = new GameObject().AddComponent<MockStanceAnimatorMB>();
		var set = new GameObject().AddComponent<MockLoadoutMB>();
		var slot = new GameObject();
		var loadout = new GameObject().AddComponent<LoadoutCircleMB>();
		loadout.slot = slot.transform;
		loadout.stanceAnimator =
			Reference<IStanceAnimation>.PointToComponent(animation);
		loadout.loadouts = new Reference<ILoadout>[] {
			Reference<ILoadout>.PointToComponent(set),
		};

		set.setAnimationLayer = a => called = a;

		yield return new WaitForEndOfFrame();

		Assert.AreSame(animation, called);
	}

	[UnityTest]
	public IEnumerator MissingStanceAnimatorIgnored() {
		var called = 0;
		var set = new GameObject().AddComponent<MockLoadoutMB>();
		var slot = new GameObject();
		var loadout = new GameObject().AddComponent<LoadoutCircleMB>();
		loadout.slot = slot.transform;
		loadout.loadouts = new Reference<ILoadout>[] {
			Reference<ILoadout>.PointToComponent(set),
		};

		set.setAnimationLayer = _ => ++called;

		yield return new WaitForEndOfFrame();

		Assert.AreEqual(0, called);
	}
}
