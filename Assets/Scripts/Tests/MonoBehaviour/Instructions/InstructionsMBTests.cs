using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class InstructionsMBTests : TestCollection
{
	public IEnumerable<YieldInstruction> RepeatAfter(
		Func<YieldInstruction> getYield,
		Action action
	) {
		for (int i = 0; i < int.MaxValue; ++i) {
			yield return getYield();
			action();
		}
	}

	class MockCoroutineSO : BaseInstructionsSO
	{
		public int times = 10;

		private IEnumerable<YieldInstruction> MoveUpEachFrame(Transform transform) {
			for (int i = 0; i < this.times; ++i) {
				yield return new WaitForEndOfFrame();
				transform.position += Vector3.up;
			}
		}

		public override CoroutineInstructions InstructionsFor(GameObject agent) {
			Transform transform = agent.GetComponent<Transform>();
			return () => this.MoveUpEachFrame(transform);
		}
	}

	[UnityTest]
	public IEnumerator RunCoroutineOnce() {
		var agent = new GameObject();
		var comp = new GameObject().AddComponent<InstructionsMB>();
		comp.instructionsSO = ScriptableObject.CreateInstance<MockCoroutineSO>();
		comp.agent = agent;

		yield return new WaitForEndOfFrame();

		comp.Begin();

		yield return new WaitForEndOfFrame();

		Assert.AreEqual(Vector3.up, agent.transform.position);
	}

	[UnityTest]
	public IEnumerator RunCoroutineOneAtATime() {
		var agent = new GameObject();
		var comp = new GameObject().AddComponent<InstructionsMB>();
		comp.instructionsSO = ScriptableObject.CreateInstance<MockCoroutineSO>();
		comp.agent = agent;

		yield return new WaitForEndOfFrame();

		comp.Begin();
		comp.Begin();

		yield return new WaitForEndOfFrame();

		Assert.AreEqual(Vector3.up * 2, agent.transform.position);
	}

	[UnityTest]
	public IEnumerator RunCoroutineMultipleAtATime() {
		var agent = new GameObject();
		var comp = new GameObject().AddComponent<InstructionsMB>();
		comp.instructionsSO = ScriptableObject.CreateInstance<MockCoroutineSO>();
		comp.agent = agent;

		yield return new WaitForEndOfFrame();

		comp.Begin();
		comp.Begin();

		yield return new WaitForEndOfFrame();

		Assert.AreEqual(Vector3.up * 2, agent.transform.position);
	}

	[UnityTest]
	public IEnumerator RunCoroutineTwice() {
		var agent = new GameObject();
		var comp = new GameObject().AddComponent<InstructionsMB>();
		comp.instructionsSO = ScriptableObject.CreateInstance<MockCoroutineSO>();
		comp.agent = agent;

		yield return new WaitForEndOfFrame();

		comp.Begin();

		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		Assert.AreEqual(Vector3.up * 2, agent.transform.position);
	}

	[UnityTest]
	public IEnumerator CancelCoroutine() {
		var agent = new GameObject();
		var comp = new GameObject().AddComponent<InstructionsMB>();
		comp.instructionsSO = ScriptableObject.CreateInstance<MockCoroutineSO>();
		comp.agent = agent;

		yield return new WaitForEndOfFrame();

		comp.Begin();

		yield return new WaitForEndOfFrame();

		comp.Cancel();

		yield return new WaitForEndOfFrame();

		Assert.AreEqual(Vector3.up, agent.transform.position);
	}

	[UnityTest]
	public IEnumerator RunCoroutineOnExternalRunner() {
		var agent = new GameObject();
		var comp = new GameObject().AddComponent<InstructionsMB>();
		var external = new GameObject().AddComponent<CoroutineRunnerMB>();
		comp.instructionsSO = ScriptableObject.CreateInstance<MockCoroutineSO>();
		comp.agent = agent;
		comp.runner = external;

		yield return new WaitForEndOfFrame();

		comp.Begin();

		yield return new WaitForEndOfFrame();

		external.StopAllCoroutines();

		yield return new WaitForEndOfFrame();

		Assert.AreEqual(Vector3.up, agent.transform.position);
	}

	[UnityTest]
	public IEnumerator CancelCoroutineOnExternalRunner() {
		var agent = new GameObject();
		var comp = new GameObject().AddComponent<InstructionsMB>();
		var external = new GameObject().AddComponent<CoroutineRunnerMB>();
		comp.instructionsSO = ScriptableObject.CreateInstance<MockCoroutineSO>();
		comp.agent = agent;

		yield return new WaitForEndOfFrame();

		comp.Begin();

		yield return new WaitForEndOfFrame();

		comp.Cancel();

		yield return new WaitForEndOfFrame();

		Assert.AreEqual(Vector3.up, agent.transform.position);
	}

	[UnityTest]
	public IEnumerator OnBegin() {
		var called = false;
		var agent = new GameObject();
		var comp = new GameObject().AddComponent<InstructionsMB>();
		var external = new GameObject().AddComponent<CoroutineRunnerMB>();
		comp.instructionsSO = ScriptableObject.CreateInstance<MockCoroutineSO>();
		comp.agent = agent;

		yield return new WaitForEndOfFrame();

		comp.onBegin!.AddListener(() => called = true);
		comp.Begin();

		Assert.True(called);
	}

	[UnityTest]
	public IEnumerator OnEnd() {
		var called = false;
		var agent = new GameObject();
		var comp = new GameObject().AddComponent<InstructionsMB>();
		var external = new GameObject().AddComponent<CoroutineRunnerMB>();
		var instructionsSO = ScriptableObject.CreateInstance<MockCoroutineSO>();
		comp.instructionsSO = instructionsSO;
		comp.agent = agent;
		instructionsSO.times = 2;

		yield return new WaitForEndOfFrame();

		comp.onEnd!.AddListener(() => called = true);
		comp.Begin();

		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		Assert.True(called);
	}

	[UnityTest]
	public IEnumerator OnEndNotBeforeLastYield() {
		var called = false;
		var agent = new GameObject();
		var comp = new GameObject().AddComponent<InstructionsMB>();
		var external = new GameObject().AddComponent<CoroutineRunnerMB>();
		var instructionsSO = ScriptableObject.CreateInstance<MockCoroutineSO>();
		comp.instructionsSO = instructionsSO;
		comp.agent = agent;
		instructionsSO.times = 2;

		yield return new WaitForEndOfFrame();

		comp.onEnd!.AddListener(() => called = true);
		comp.Begin();

		yield return new WaitForEndOfFrame();

		Assert.False(called);
	}
}
