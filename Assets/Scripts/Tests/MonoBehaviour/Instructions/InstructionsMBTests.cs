using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class InstructionsMBTests : TestCollection
{
	class MockCoroutineSO : BaseInstructionsSO<Transform>
	{
		public int times = 10;

		protected override Transform GetConcreteAgent(GameObject agent) =>
			agent.transform;
		protected override CoroutineInstructions Instructions(Transform agent) =>
			() => this.MoveUpEachFrame(agent);

		private IEnumerable<YieldInstruction> MoveUpEachFrame(Transform transform) {
			for (int i = 0; i < this.times; ++i) {
				yield return new WaitForEndOfFrame();
				transform.position += Vector3.up;
			}
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
	public IEnumerator OverrideAll() {
		var calledOther = false;
		var agent = new GameObject();
		var comp = new GameObject().AddComponent<InstructionsMB>();
		comp.instructionsSO = ScriptableObject.CreateInstance<MockCoroutineSO>();
		comp.agent = agent;
		comp.overrideMode = OverrideMode.All;

		IEnumerator otherRoutine() {
			yield return new WaitForEndOfFrame();
			calledOther = true;
		}

		yield return new WaitForEndOfFrame();

		comp.StartCoroutine(otherRoutine());
		comp.Begin();

		yield return new WaitForEndOfFrame();

		Assert.False(calledOther);
	}

	[UnityTest]
	public IEnumerator OverrideAllOnExternal() {
		var calledOther = false;
		var agent = new GameObject();
		var comp = new GameObject().AddComponent<InstructionsMB>();
		var external = new GameObject().AddComponent<CoroutineRunnerMB>();
		comp.instructionsSO = ScriptableObject.CreateInstance<MockCoroutineSO>();
		comp.agent = agent;
		comp.overrideMode = OverrideMode.All;
		comp.runner = external;

		IEnumerator otherRoutine() {
			yield return new WaitForEndOfFrame();
			calledOther = true;
		}

		yield return new WaitForEndOfFrame();

		external.StartCoroutine(otherRoutine());
		comp.Begin();

		yield return new WaitForEndOfFrame();

		Assert.False(calledOther);
	}

	[UnityTest]
	public IEnumerator OverrideOwn() {
		var calledOther = false;
		var agent = new GameObject();
		var comp = new GameObject().AddComponent<InstructionsMB>();
		comp.instructionsSO = ScriptableObject.CreateInstance<MockCoroutineSO>();
		comp.agent = agent;
		comp.overrideMode = OverrideMode.Own;

		IEnumerator otherRoutine() {
			yield return new WaitForEndOfFrame();
			calledOther = true;
		}

		yield return new WaitForEndOfFrame();

		comp.StartCoroutine(otherRoutine());
		comp.Begin();
		comp.Begin();

		yield return new WaitForEndOfFrame();

		Assert.AreEqual(
			(Vector3.up, true),
			(agent.transform.position, calledOther)
		);
	}
	[UnityTest]
	public IEnumerator OverrideOwnOnExternal() {
		var calledOther = false;
		var agent = new GameObject();
		var comp = new GameObject().AddComponent<InstructionsMB>();
		var external = new GameObject().AddComponent<CoroutineRunnerMB>();
		comp.instructionsSO = ScriptableObject.CreateInstance<MockCoroutineSO>();
		comp.agent = agent;
		comp.overrideMode = OverrideMode.Own;
		comp.runner = external;

		IEnumerator otherRoutine() {
			yield return new WaitForEndOfFrame();
			calledOther = true;
		}

		yield return new WaitForEndOfFrame();

		external.StartCoroutine(otherRoutine());
		comp.Begin();
		comp.Begin();

		yield return new WaitForEndOfFrame();

		Assert.AreEqual(
			(Vector3.up, true),
			(agent.transform.position, calledOther)
		);
	}
}
