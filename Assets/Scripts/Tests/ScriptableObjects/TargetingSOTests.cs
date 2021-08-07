using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TargetingSOTests : TestCollection
{
	private class MockHitter : IHit
	{
		public Func<object, (object, bool)> tryHit = _ => (null, false);

		public bool Try<T>(T source, out T target) {
			(object hit, bool success) = this.tryHit(source);
			target = hit == null ? default : (T)hit;
			return success;
		}
	}

	private class MockHitterSO : BaseHitSO
	{
		public MockHitter hit = new TargetingSOTests.MockHitter();
		public override IHit Hit => this.hit;
	}

	[UnityTest]
	public IEnumerator AddTarget() {
		var targets = new List<CharacterSheetMB>();
		var target = new GameObject("target").AddComponent<CharacterSheetMB>();
		var singleTarget = ScriptableObject.CreateInstance<TargetingSO>();
		var hitter = ScriptableObject.CreateInstance<MockHitterSO>();
		singleTarget.hitter = hitter;
		singleTarget.selectTarget = ScriptableObject.CreateInstance<EventSO>();
		singleTarget.cancelSelect = ScriptableObject.CreateInstance<EventSO>();
		hitter.hit.tryHit = _ => (target, true);

		yield return new WaitForEndOfFrame();

		var routine = singleTarget
			.Select(default, targets)
			.GetEnumerator();
		routine.MoveNext();
		singleTarget.selectTarget.Raise();

		CollectionAssert.AreEqual(new CharacterSheetMB[] { target }, targets);
	}

	[UnityTest]
	public IEnumerator InjectSourceInHitTry() {
		var targets = new List<CharacterSheetMB>();
		var source = new GameObject("target").AddComponent<CharacterSheetMB>();
		var singleTarget = ScriptableObject.CreateInstance<TargetingSO>();
		var hitter = ScriptableObject.CreateInstance<MockHitterSO>();
		singleTarget.hitter = hitter;
		singleTarget.selectTarget = ScriptableObject.CreateInstance<EventSO>();
		singleTarget.cancelSelect = ScriptableObject.CreateInstance<EventSO>();
		hitter.hit.tryHit = s => (s, true);

		yield return new WaitForEndOfFrame();

		var routine = singleTarget
			.Select(source, targets)
			.GetEnumerator();
		routine.MoveNext();
		singleTarget.selectTarget.Raise();

		CollectionAssert.AreEqual(new CharacterSheetMB[] { source }, targets);
	}

	[UnityTest]
	public IEnumerator DontAddTargetWhenNoHit() {
		var targets = new List<CharacterSheetMB>();
		var target = new GameObject("target").AddComponent<CharacterSheetMB>();
		var singleTarget = ScriptableObject.CreateInstance<TargetingSO>();
		var hitter = ScriptableObject.CreateInstance<MockHitterSO>();
		singleTarget.hitter = hitter;
		singleTarget.selectTarget = ScriptableObject.CreateInstance<EventSO>();
		singleTarget.cancelSelect = ScriptableObject.CreateInstance<EventSO>();
		hitter.hit.tryHit = _ => (null, false);

		yield return new WaitForEndOfFrame();

		var routine = singleTarget
			.Select(default, targets)
			.GetEnumerator();
		routine.MoveNext();
		singleTarget.selectTarget.Raise();

		CollectionAssert.IsEmpty(targets);
	}

	[UnityTest]
	public IEnumerator DontAddTargetWhenNoSelectTargetRaise() {
		var targets = new List<CharacterSheetMB>();
		var target = new GameObject("target").AddComponent<CharacterSheetMB>();
		var singleTarget = ScriptableObject.CreateInstance<TargetingSO>();
		var hitter = ScriptableObject.CreateInstance<MockHitterSO>();
		singleTarget.hitter = hitter;
		singleTarget.selectTarget = ScriptableObject.CreateInstance<EventSO>();
		singleTarget.cancelSelect = ScriptableObject.CreateInstance<EventSO>();
		hitter.hit.tryHit = _ => (target, true);

		yield return new WaitForEndOfFrame();

		var routine = singleTarget
			.Select(default, targets)
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
		singleTarget.selectTarget = ScriptableObject.CreateInstance<EventSO>();
		singleTarget.cancelSelect = ScriptableObject.CreateInstance<EventSO>();
		hitter.hit.tryHit = _ => (target, true);

		yield return new WaitForEndOfFrame();

		var routine = singleTarget
			.Select(default, new List<CharacterSheetMB>())
			.GetEnumerator();
		yields.Add(routine.MoveNext());
		yields.Add(routine.MoveNext());
		singleTarget.selectTarget.Raise();
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
		singleTarget.selectTarget = ScriptableObject.CreateInstance<EventSO>();
		singleTarget.cancelSelect = ScriptableObject.CreateInstance<EventSO>();
		hitter.hit.tryHit = _ => (null, false);

		yield return new WaitForEndOfFrame();

		var routine = singleTarget
			.Select(default, new List<CharacterSheetMB>())
			.GetEnumerator();
		yields.Add(routine.MoveNext());
		singleTarget.selectTarget.Raise();
		yields.Add(routine.MoveNext());
		singleTarget.selectTarget.Raise();
		yields.Add(routine.MoveNext());
		hitter.hit.tryHit = _ => (target, true);
		singleTarget.selectTarget.Raise();
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
		singleTarget.selectTarget = ScriptableObject.CreateInstance<EventSO>();
		singleTarget.cancelSelect = ScriptableObject.CreateInstance<EventSO>();
		hitter.hit.tryHit = _ => (target, true);

		yield return new WaitForEndOfFrame();

		var routine = singleTarget
			.Select(default, targets)
			.GetEnumerator();
		routine.MoveNext();
		routine.MoveNext();
		singleTarget.selectTarget.Raise();
		routine.MoveNext();
		singleTarget.selectTarget.Raise();

		CollectionAssert.AreEqual(new CharacterSheetMB[] { target }, targets);
	}

	[UnityTest]
	public IEnumerator CancelSelectClearsTargets() {
		var targets = new List<CharacterSheetMB>();
		var target = new GameObject("target").AddComponent<CharacterSheetMB>();
		var singleTarget = ScriptableObject.CreateInstance<TargetingSO>();
		var hitter = ScriptableObject.CreateInstance<MockHitterSO>();
		singleTarget.hitter = hitter;
		singleTarget.selectTarget = ScriptableObject.CreateInstance<EventSO>();
		singleTarget.cancelSelect = ScriptableObject.CreateInstance<EventSO>();
		hitter.hit.tryHit = _ => (target, true);

		yield return new WaitForEndOfFrame();

		var routine = singleTarget
			.Select(default, targets)
			.GetEnumerator();
		routine.MoveNext();
		singleTarget.selectTarget.Raise();
		singleTarget.cancelSelect.Raise();

		CollectionAssert.IsEmpty(targets);
	}

	[UnityTest]
	public IEnumerator YieldUntilCancel() {
		var yields = new List<bool>();
		var target = new GameObject("target").AddComponent<CharacterSheetMB>();
		var singleTarget = ScriptableObject.CreateInstance<TargetingSO>();
		var hitter = ScriptableObject.CreateInstance<MockHitterSO>();
		singleTarget.hitter = hitter;
		singleTarget.selectTarget = ScriptableObject.CreateInstance<EventSO>();
		singleTarget.cancelSelect = ScriptableObject.CreateInstance<EventSO>();

		yield return new WaitForEndOfFrame();

		var routine = singleTarget
			.Select(default, new List<CharacterSheetMB>())
			.GetEnumerator();
		yields.Add(routine.MoveNext());
		yields.Add(routine.MoveNext());
		yields.Add(routine.MoveNext());
		singleTarget.cancelSelect.Raise();
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
		singleTarget.selectTarget = ScriptableObject.CreateInstance<EventSO>();
		singleTarget.cancelSelect = ScriptableObject.CreateInstance<EventSO>();

		yield return new WaitForEndOfFrame();

		var routine = singleTarget
			.Select(default, targets)
			.GetEnumerator();
		routine.MoveNext();
		routine.MoveNext();
		singleTarget.cancelSelect.Raise();
		routine.MoveNext();

		targets.Add(target);
		singleTarget.cancelSelect.Raise();

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
		singleTarget.selectTarget = ScriptableObject.CreateInstance<EventSO>();
		singleTarget.cancelSelect = ScriptableObject.CreateInstance<EventSO>();
		hitter.hit.tryHit = _ => (targets[i++], true);

		yield return new WaitForEndOfFrame();

		var routine = singleTarget
			.Select(default, selectedTargets, maxCount: 3)
			.GetEnumerator();
		routine.MoveNext();
		singleTarget.selectTarget.Raise();
		routine.MoveNext();
		singleTarget.selectTarget.Raise();
		routine.MoveNext();
		singleTarget.selectTarget.Raise();
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
		singleTarget.selectTarget = ScriptableObject.CreateInstance<EventSO>();
		singleTarget.cancelSelect = ScriptableObject.CreateInstance<EventSO>();
		hitter.hit.tryHit = _ => (targets[i++], true);

		yield return new WaitForEndOfFrame();

		var routine = singleTarget
			.Select(default, selectedTargets, maxCount: 4)
			.GetEnumerator();
		routine.MoveNext();
		singleTarget.selectTarget.Raise();
		routine.MoveNext();
		singleTarget.selectTarget.Raise();
		routine.MoveNext();
		singleTarget.selectTarget.Raise();
		routine.MoveNext();
		singleTarget.cancelSelect.Raise();
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
		singleTarget.selectTarget = ScriptableObject.CreateInstance<EventSO>();
		singleTarget.cancelSelect = ScriptableObject.CreateInstance<EventSO>();
		hitter.hit.tryHit = _ => (target, true);

		yield return new WaitForEndOfFrame();

		var routine = singleTarget
			.Select(default, selectedTargets, maxCount: 3)
			.GetEnumerator();
		yields.Add(routine.MoveNext());
		singleTarget.selectTarget.Raise();
		yields.Add(routine.MoveNext());
		singleTarget.selectTarget.Raise();
		yields.Add(routine.MoveNext());
		singleTarget.cancelSelect.Raise();
		yields.Add(routine.MoveNext());
		singleTarget.selectTarget.Raise();
		yields.Add(routine.MoveNext());
		singleTarget.selectTarget.Raise();
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
		singleTarget.selectTarget = ScriptableObject.CreateInstance<EventSO>();
		singleTarget.cancelSelect = ScriptableObject.CreateInstance<EventSO>();
		singleTarget.doubleSelectFinishes = true;
		hitter.hit.tryHit = _ => (target, true);

		yield return new WaitForEndOfFrame();

		var routine = singleTarget
			.Select(default, new List<CharacterSheetMB>(), maxCount: 10)
			.GetEnumerator();
		yields.Add(routine.MoveNext());
		singleTarget.selectTarget.Raise();
		yields.Add(routine.MoveNext());
		singleTarget.selectTarget.Raise();
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
		singleTarget.selectTarget = ScriptableObject.CreateInstance<EventSO>();
		singleTarget.cancelSelect = ScriptableObject.CreateInstance<EventSO>();
		singleTarget.doubleSelectFinishes = true;
		hitter.hit.tryHit = _ => (target, true);

		yield return new WaitForEndOfFrame();

		var routine = singleTarget
			.Select(default, targets, maxCount: 10)
			.GetEnumerator();
		routine.MoveNext();
		singleTarget.selectTarget.Raise();
		routine.MoveNext();
		singleTarget.selectTarget.Raise();
		routine.MoveNext();

		CollectionAssert.AreEqual(new CharacterSheetMB[] { target }, targets);
	}
}
