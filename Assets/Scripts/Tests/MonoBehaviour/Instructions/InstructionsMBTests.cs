using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class InstructionsMBTests : TestCollection
{
	struct MockData { }

	class MockCoroutineSO : BaseInstructionsSO<Transform, MockData>
	{
		public int times = 10;

		protected override Transform GetConcreteAgent(GameObject agent) {
			return agent.transform;
		}

		protected override CoroutineInstructions Instructions(Transform agent) {
			return () => this.MoveUpEachFrame(agent);
		}

		protected override MockData GetPluginData(GameObject agent) {
			return default;
		}

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

		comp.Apply();

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

		comp.Apply();
		comp.Apply();

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

		comp.Apply();

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

		comp.Apply();

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
		comp.Apply();

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
		comp.Apply();

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
		comp.Apply();
		comp.Apply();

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
		comp.Apply();
		comp.Apply();

		yield return new WaitForEndOfFrame();

		Assert.AreEqual(
			(Vector3.up, true),
			(agent.transform.position, calledOther)
		);
	}

	class MockPluginSO : BaseInstructionsPluginSO<MockData>
	{
		public Func<GameObject, Action<MockData>> getOnBegin = _ => _ => { };
		public Func<GameObject, Action<MockData>> getOnEnd = _ => _ => { };

		public override Action<MockData> GetOnBegin(GameObject agent) {
			return this.getOnBegin(agent);
		}

		public override Action<MockData> GetOnEnd(GameObject agent) {
			return this.getOnEnd(agent);
		}
	}

	[UnityTest]
	public IEnumerator Release() {
		var calledEnd = 0;
		var agent = new GameObject();
		var comp = new GameObject().AddComponent<InstructionsMB>();
		var external = new GameObject().AddComponent<CoroutineRunnerMB>();
		var plugin = ScriptableObject.CreateInstance<MockPluginSO>();
		var instructions = ScriptableObject.CreateInstance<MockCoroutineSO>();
		comp.instructionsSO = instructions;
		comp.agent = agent;
		comp.overrideMode = OverrideMode.Own;
		comp.runner = external;

		plugin.getOnEnd = _ => _ => ++calledEnd;
		instructions.plugins = new MockPluginSO[] { plugin };

		yield return new WaitForEndOfFrame();

		comp.Apply();

		yield return new WaitForEndOfFrame();

		comp.Release();

		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		Assert.AreEqual((Vector3.up * 2, 1), (agent.transform.position, calledEnd));
	}
}
