using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TargetingSOTests : TestCollection
{
	private class MockHitterSO : BaseHitSO
	{
		public Func<object, object?> tryHit = _ => null;

		public override Maybe<T> Try<T>(T source) {
			object? hit = this.tryHit(source);
			return hit != null
				? Maybe.Some((T)hit)
				: Maybe.None<T>();
		}

		public override Maybe<Vector3> TryPoint(Transform source) {
			throw new NotImplementedException();
		}
	}

	private CharacterSheetMB DefaultSheet
		=> new GameObject("Default").AddComponent<CharacterSheetMB>();

	[UnityTest]
	public IEnumerator AddTarget() {
		var targets = new List<CharacterSheetMB>();
		var target = new GameObject("target").AddComponent<CharacterSheetMB>();
		var singleTarget = ScriptableObject.CreateInstance<TargetingSO>();
		var hitter = ScriptableObject.CreateInstance<MockHitterSO>();
		singleTarget.hitter = hitter;
		singleTarget.selectTarget = ScriptableObject.CreateInstance<ChannelSO>();
		singleTarget.cancelSelect = ScriptableObject.CreateInstance<ChannelSO>();
		hitter.tryHit = _ => target;

		yield return new WaitForEndOfFrame();

		var routine = singleTarget
			.Select(this.DefaultSheet, targets)
			.GetEnumerator();
		routine.MoveNext();
		((ChannelSO)singleTarget.selectTarget).Raise();

		CollectionAssert.AreEqual(new CharacterSheetMB[] { target }, targets);
	}

	[UnityTest]
	public IEnumerator InjectSourceInHitTry() {
		var targets = new List<CharacterSheetMB>();
		var source = new GameObject("target").AddComponent<CharacterSheetMB>();
		var singleTarget = ScriptableObject.CreateInstance<TargetingSO>();
		var hitter = ScriptableObject.CreateInstance<MockHitterSO>();
		singleTarget.hitter = hitter;
		singleTarget.selectTarget = ScriptableObject.CreateInstance<ChannelSO>();
		singleTarget.cancelSelect = ScriptableObject.CreateInstance<ChannelSO>();
		hitter.tryHit = s => s;

		yield return new WaitForEndOfFrame();

		var routine = singleTarget
			.Select(source, targets)
			.GetEnumerator();
		routine.MoveNext();
		((ChannelSO)singleTarget.selectTarget).Raise();

		CollectionAssert.AreEqual(new CharacterSheetMB[] { source }, targets);
	}

	[UnityTest]
	public IEnumerator DontAddTargetWhenNoHit() {
		var targets = new List<CharacterSheetMB>();
		var target = new GameObject("target").AddComponent<CharacterSheetMB>();
		var singleTarget = ScriptableObject.CreateInstance<TargetingSO>();
		var hitter = ScriptableObject.CreateInstance<MockHitterSO>();
		singleTarget.hitter = hitter;
		singleTarget.selectTarget = ScriptableObject.CreateInstance<ChannelSO>();
		singleTarget.cancelSelect = ScriptableObject.CreateInstance<ChannelSO>();
		hitter.tryHit = _ => null;

		yield return new WaitForEndOfFrame();

		var routine = singleTarget
			.Select(this.DefaultSheet, targets)
			.GetEnumerator();
		routine.MoveNext();
		((ChannelSO)singleTarget.selectTarget).Raise();

		CollectionAssert.IsEmpty(targets);
	}

	[UnityTest]
	public IEnumerator DontAddTargetWhenNoSelectTargetRaise() {
		var targets = new List<CharacterSheetMB>();
		var target = new GameObject("target").AddComponent<CharacterSheetMB>();
		var singleTarget = ScriptableObject.CreateInstance<TargetingSO>();
		var hitter = ScriptableObject.CreateInstance<MockHitterSO>();
		singleTarget.hitter = hitter;
		singleTarget.selectTarget = ScriptableObject.CreateInstance<ChannelSO>();
		singleTarget.cancelSelect = ScriptableObject.CreateInstance<ChannelSO>();
		hitter.tryHit = _ => target;

		yield return new WaitForEndOfFrame();

		var routine = singleTarget
			.Select(this.DefaultSheet, targets)
			.GetEnumerator();
		routine.MoveNext();

		CollectionAssert.IsEmpty(targets);
	}

	[UnityTest]
	public IEnumerator YieldUntilSelectTargetRaise() {
		var yields = new List<bool>();
		var target = new GameObject("target").AddComponent<CharacterSheetMB>();
		var singleTarget = ScriptableObject.CreateInstance<TargetingSO>();
		var hitter = ScriptableObject.CreateInstance<MockHitterSO>();
		singleTarget.hitter = hitter;
		singleTarget.selectTarget = ScriptableObject.CreateInstance<ChannelSO>();
		singleTarget.cancelSelect = ScriptableObject.CreateInstance<ChannelSO>();
		hitter.tryHit = _ => target;

		yield return new WaitForEndOfFrame();

		var routine = singleTarget
			.Select(this.DefaultSheet, new List<CharacterSheetMB>())
			.GetEnumerator();
		yields.Add(routine.MoveNext());
		yields.Add(routine.MoveNext());
		((ChannelSO)singleTarget.selectTarget).Raise();
		yields.Add(routine.MoveNext());

		CollectionAssert.AreEqual(new bool[] { true, true, false }, yields);
	}

	[UnityTest]
	public IEnumerator YieldUntilHit() {
		var yields = new List<bool>();
		var target = new GameObject("target").AddComponent<CharacterSheetMB>();
		var singleTarget = ScriptableObject.CreateInstance<TargetingSO>();
		var hitter = ScriptableObject.CreateInstance<MockHitterSO>();
		singleTarget.hitter = hitter;
		singleTarget.selectTarget = ScriptableObject.CreateInstance<ChannelSO>();
		singleTarget.cancelSelect = ScriptableObject.CreateInstance<ChannelSO>();
		hitter.tryHit = _ => null;

		yield return new WaitForEndOfFrame();

		var routine = singleTarget
			.Select(this.DefaultSheet, new List<CharacterSheetMB>())
			.GetEnumerator();
		yields.Add(routine.MoveNext());
		((ChannelSO)singleTarget.selectTarget).Raise();
		yields.Add(routine.MoveNext());
		((ChannelSO)singleTarget.selectTarget).Raise();
		yields.Add(routine.MoveNext());
		hitter.tryHit = _ => target;
		((ChannelSO)singleTarget.selectTarget).Raise();
		yields.Add(routine.MoveNext());

		CollectionAssert.AreEqual(new bool[] { true, true, true, false }, yields);
	}

	[UnityTest]
	public IEnumerator SelectTargetListenerRemovedAfterRoutine() {
		var target = new GameObject("target").AddComponent<CharacterSheetMB>();
		var targets = new List<CharacterSheetMB>();
		var singleTarget = ScriptableObject.CreateInstance<TargetingSO>();
		var hitter = ScriptableObject.CreateInstance<MockHitterSO>();
		singleTarget.hitter = hitter;
		singleTarget.selectTarget = ScriptableObject.CreateInstance<ChannelSO>();
		singleTarget.cancelSelect = ScriptableObject.CreateInstance<ChannelSO>();
		hitter.tryHit = _ => target;

		yield return new WaitForEndOfFrame();

		var routine = singleTarget
			.Select(this.DefaultSheet, targets)
			.GetEnumerator();
		routine.MoveNext();
		routine.MoveNext();
		((ChannelSO)singleTarget.selectTarget).Raise();
		routine.MoveNext();
		((ChannelSO)singleTarget.selectTarget).Raise();

		CollectionAssert.AreEqual(new CharacterSheetMB[] { target }, targets);
	}

	[UnityTest]
	public IEnumerator CancelSelectClearsTargets() {
		var targets = new List<CharacterSheetMB>();
		var target = new GameObject("target").AddComponent<CharacterSheetMB>();
		var singleTarget = ScriptableObject.CreateInstance<TargetingSO>();
		var hitter = ScriptableObject.CreateInstance<MockHitterSO>();
		singleTarget.hitter = hitter;
		singleTarget.selectTarget = ScriptableObject.CreateInstance<ChannelSO>();
		singleTarget.cancelSelect = ScriptableObject.CreateInstance<ChannelSO>();
		hitter.tryHit = _ => target;

		yield return new WaitForEndOfFrame();

		var routine = singleTarget
			.Select(this.DefaultSheet, targets)
			.GetEnumerator();
		routine.MoveNext();
		((ChannelSO)singleTarget.selectTarget).Raise();
		((ChannelSO)singleTarget.cancelSelect).Raise();

		CollectionAssert.IsEmpty(targets);
	}

	[UnityTest]
	public IEnumerator YieldUntilCancel() {
		var yields = new List<bool>();
		var target = new GameObject("target").AddComponent<CharacterSheetMB>();
		var singleTarget = ScriptableObject.CreateInstance<TargetingSO>();
		var hitter = ScriptableObject.CreateInstance<MockHitterSO>();
		singleTarget.hitter = hitter;
		singleTarget.selectTarget = ScriptableObject.CreateInstance<ChannelSO>();
		singleTarget.cancelSelect = ScriptableObject.CreateInstance<ChannelSO>();

		yield return new WaitForEndOfFrame();

		var routine = singleTarget
			.Select(this.DefaultSheet, new List<CharacterSheetMB>())
			.GetEnumerator();
		yields.Add(routine.MoveNext());
		yields.Add(routine.MoveNext());
		yields.Add(routine.MoveNext());
		((ChannelSO)singleTarget.cancelSelect).Raise();
		yields.Add(routine.MoveNext());

		CollectionAssert.AreEqual(new bool[] { true, true, true, false }, yields);
	}

	[UnityTest]
	public IEnumerator CancelSelectListenerRemovedAfterRoutine() {
		var targets = new List<CharacterSheetMB>();
		var target = new GameObject("target").AddComponent<CharacterSheetMB>();
		var singleTarget = ScriptableObject.CreateInstance<TargetingSO>();
		var hitter = ScriptableObject.CreateInstance<MockHitterSO>();
		singleTarget.hitter = hitter;
		singleTarget.selectTarget = ScriptableObject.CreateInstance<ChannelSO>();
		singleTarget.cancelSelect = ScriptableObject.CreateInstance<ChannelSO>();

		yield return new WaitForEndOfFrame();

		var routine = singleTarget
			.Select(this.DefaultSheet, targets)
			.GetEnumerator();
		routine.MoveNext();
		routine.MoveNext();
		((ChannelSO)singleTarget.cancelSelect).Raise();
		routine.MoveNext();

		targets.Add(target);
		((ChannelSO)singleTarget.cancelSelect).Raise();

		CollectionAssert.AreEqual(new CharacterSheetMB[] { target }, targets);
	}

	[UnityTest]
	public IEnumerator AddMultipleTargets() {
		var i = 0;
		var selectedTargets = new List<CharacterSheetMB>();
		var targets = new CharacterSheetMB[] {
			new GameObject("targetA").AddComponent<CharacterSheetMB>(),
			new GameObject("targetB").AddComponent<CharacterSheetMB>(),
			new GameObject("targetC").AddComponent<CharacterSheetMB>(),
		};
		var singleTarget = ScriptableObject.CreateInstance<TargetingSO>();
		var hitter = ScriptableObject.CreateInstance<MockHitterSO>();
		singleTarget.hitter = hitter;
		singleTarget.selectTarget = ScriptableObject.CreateInstance<ChannelSO>();
		singleTarget.cancelSelect = ScriptableObject.CreateInstance<ChannelSO>();
		hitter.tryHit = _ => targets[i++];

		yield return new WaitForEndOfFrame();

		var routine = singleTarget
			.Select(this.DefaultSheet, selectedTargets, maxCount: 3)
			.GetEnumerator();
		routine.MoveNext();
		((ChannelSO)singleTarget.selectTarget).Raise();
		routine.MoveNext();
		((ChannelSO)singleTarget.selectTarget).Raise();
		routine.MoveNext();
		((ChannelSO)singleTarget.selectTarget).Raise();
		routine.MoveNext();

		CollectionAssert.AreEqual(targets, selectedTargets);
	}

	[UnityTest]
	public IEnumerator CancelSelectRemovesLastTarget() {
		var i = 0;
		var selectedTargets = new List<CharacterSheetMB>();
		var targets = new CharacterSheetMB[] {
			new GameObject("targetA").AddComponent<CharacterSheetMB>(),
			new GameObject("targetB").AddComponent<CharacterSheetMB>(),
			new GameObject("targetC").AddComponent<CharacterSheetMB>(),
			new GameObject("targetD").AddComponent<CharacterSheetMB>(),
		};
		var singleTarget = ScriptableObject.CreateInstance<TargetingSO>();
		var hitter = ScriptableObject.CreateInstance<MockHitterSO>();
		singleTarget.hitter = hitter;
		singleTarget.selectTarget = ScriptableObject.CreateInstance<ChannelSO>();
		singleTarget.cancelSelect = ScriptableObject.CreateInstance<ChannelSO>();
		hitter.tryHit = _ => targets[i++];

		yield return new WaitForEndOfFrame();

		var routine = singleTarget
			.Select(this.DefaultSheet, selectedTargets, maxCount: 4)
			.GetEnumerator();
		routine.MoveNext();
		((ChannelSO)singleTarget.selectTarget).Raise();
		routine.MoveNext();
		((ChannelSO)singleTarget.selectTarget).Raise();
		routine.MoveNext();
		((ChannelSO)singleTarget.selectTarget).Raise();
		routine.MoveNext();
		((ChannelSO)singleTarget.cancelSelect).Raise();
		routine.MoveNext();

		CollectionAssert.AreEqual(targets.Take(2), selectedTargets);
	}

	[UnityTest]
	public IEnumerator YieldUntilFullySelectedWithIntermediateCancel() {
		var yields = new List<bool>();
		var selectedTargets = new List<CharacterSheetMB>();
		var target = new GameObject("target").AddComponent<CharacterSheetMB>();
		var singleTarget = ScriptableObject.CreateInstance<TargetingSO>();
		var hitter = ScriptableObject.CreateInstance<MockHitterSO>();
		singleTarget.hitter = hitter;
		singleTarget.selectTarget = ScriptableObject.CreateInstance<ChannelSO>();
		singleTarget.cancelSelect = ScriptableObject.CreateInstance<ChannelSO>();
		hitter.tryHit = _ => target;

		yield return new WaitForEndOfFrame();

		var routine = singleTarget
			.Select(this.DefaultSheet, selectedTargets, maxCount: 3)
			.GetEnumerator();
		yields.Add(routine.MoveNext());
		((ChannelSO)singleTarget.selectTarget).Raise();
		yields.Add(routine.MoveNext());
		((ChannelSO)singleTarget.selectTarget).Raise();
		yields.Add(routine.MoveNext());
		((ChannelSO)singleTarget.cancelSelect).Raise();
		yields.Add(routine.MoveNext());
		((ChannelSO)singleTarget.selectTarget).Raise();
		yields.Add(routine.MoveNext());
		((ChannelSO)singleTarget.selectTarget).Raise();
		yields.Add(routine.MoveNext());

		CollectionAssert.AreEqual(
			new bool[] { true, true, true, true, true, false },
			yields
		);
	}

	[UnityTest]
	public IEnumerator TerminateRoutinePrematurely() {
		var yields = new List<bool>();
		var target = new GameObject("target").AddComponent<CharacterSheetMB>();
		var singleTarget = ScriptableObject.CreateInstance<TargetingSO>();
		var hitter = ScriptableObject.CreateInstance<MockHitterSO>();
		singleTarget.hitter = hitter;
		singleTarget.selectTarget = ScriptableObject.CreateInstance<ChannelSO>();
		singleTarget.cancelSelect = ScriptableObject.CreateInstance<ChannelSO>();
		singleTarget.doubleSelectFinishes = true;
		hitter.tryHit = _ => target;

		yield return new WaitForEndOfFrame();

		var routine = singleTarget
			.Select(this.DefaultSheet, new List<CharacterSheetMB>(), maxCount: 10)
			.GetEnumerator();
		yields.Add(routine.MoveNext());
		((ChannelSO)singleTarget.selectTarget).Raise();
		yields.Add(routine.MoveNext());
		((ChannelSO)singleTarget.selectTarget).Raise();
		yields.Add(routine.MoveNext());

		CollectionAssert.AreEqual(new bool[] { true, true, false }, yields);
	}

	[UnityTest]
	public IEnumerator TerminateRoutinePrematurelyNoDoubles() {
		var targets = new List<CharacterSheetMB>();
		var target = new GameObject("target").AddComponent<CharacterSheetMB>();
		var singleTarget = ScriptableObject.CreateInstance<TargetingSO>();
		var hitter = ScriptableObject.CreateInstance<MockHitterSO>();
		singleTarget.hitter = hitter;
		singleTarget.selectTarget = ScriptableObject.CreateInstance<ChannelSO>();
		singleTarget.cancelSelect = ScriptableObject.CreateInstance<ChannelSO>();
		singleTarget.doubleSelectFinishes = true;
		hitter.tryHit = _ => target;

		yield return new WaitForEndOfFrame();

		var routine = singleTarget
			.Select(this.DefaultSheet, targets, maxCount: 10)
			.GetEnumerator();
		routine.MoveNext();
		((ChannelSO)singleTarget.selectTarget).Raise();
		routine.MoveNext();
		((ChannelSO)singleTarget.selectTarget).Raise();
		routine.MoveNext();

		CollectionAssert.AreEqual(new CharacterSheetMB[] { target }, targets);
	}
}
