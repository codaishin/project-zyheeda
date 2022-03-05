using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class InstructionsMBTests : TestCollection
{
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

	class MockCoroutinePluginSO : BaseInstructionsPluginSO
	{
		public Func<GameObject, Action> getOnBegin = _ => () => { };
		public Func<GameObject, Action> getOnEnd = _ => () => { };

		public override Action GetOnBegin(GameObject agent) =>
			this.getOnBegin(agent);
		public override Action GetOnEnd(GameObject agent) =>
			this.getOnEnd(agent);
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
	public IEnumerator OnBegin() {
		var called = new List<GameObject>();
		var agent = new GameObject();
		var comp = new GameObject().AddComponent<InstructionsMB>();
		var pluginASO = ScriptableObject.CreateInstance<MockCoroutinePluginSO>();
		var pluginBSO = ScriptableObject.CreateInstance<MockCoroutinePluginSO>();
		var instructionsSO = ScriptableObject.CreateInstance<MockCoroutineSO>();
		comp.instructionsSO = instructionsSO;
		comp.agent = agent;

		instructionsSO.plugins = new MockCoroutinePluginSO[] {
			pluginASO,
			pluginBSO
		};
		pluginASO.getOnBegin = a => () => called.Add(a);
		pluginBSO.getOnBegin = a => () => called.Add(a);

		yield return new WaitForEndOfFrame();

		comp.Begin();

		CollectionAssert.AreEqual(new GameObject[] { agent, agent }, called);
	}

	[UnityTest]
	public IEnumerator OnEnd() {
		var called = new List<GameObject>();
		var agent = new GameObject();
		var comp = new GameObject().AddComponent<InstructionsMB>();
		var pluginASO = ScriptableObject.CreateInstance<MockCoroutinePluginSO>();
		var pluginBSO = ScriptableObject.CreateInstance<MockCoroutinePluginSO>();
		var instructionsSO = ScriptableObject.CreateInstance<MockCoroutineSO>();
		comp.instructionsSO = instructionsSO;
		comp.agent = agent;

		instructionsSO.plugins = new MockCoroutinePluginSO[] {
			pluginASO,
			pluginBSO
		};
		instructionsSO.times = 2;
		pluginASO.getOnEnd = a => () => called.Add(a);
		pluginBSO.getOnEnd = a => () => called.Add(a);

		yield return new WaitForEndOfFrame();

		comp.Begin();

		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		CollectionAssert.AreEqual(new GameObject[] { agent, agent }, called);
	}

	[UnityTest]
	public IEnumerator OnEndNotBeforeLastYield() {
		var called = new List<GameObject>();
		var agent = new GameObject();
		var comp = new GameObject().AddComponent<InstructionsMB>();
		var pluginASO = ScriptableObject.CreateInstance<MockCoroutinePluginSO>();
		var pluginBSO = ScriptableObject.CreateInstance<MockCoroutinePluginSO>();
		var instructionsSO = ScriptableObject.CreateInstance<MockCoroutineSO>();
		comp.instructionsSO = instructionsSO;
		comp.agent = agent;

		instructionsSO.plugins = new MockCoroutinePluginSO[] {
			pluginASO,
			pluginBSO
		};
		instructionsSO.times = 2;
		pluginASO.getOnEnd = a => () => called.Add(a);
		pluginBSO.getOnEnd = a => () => called.Add(a);

		yield return new WaitForEndOfFrame();

		comp.Begin();

		yield return new WaitForEndOfFrame();

		CollectionAssert.IsEmpty(called);
	}



	// [UnityTest]
	// public IEnumerator OnEndNotBeforeLastYield() {
	// 	var called = false;
	// 	var agent = new GameObject();
	// 	var comp = new GameObject().AddComponent<InstructionsMB>();
	// 	var external = new GameObject().AddComponent<CoroutineRunnerMB>();
	// 	var instructionsSO = ScriptableObject.CreateInstance<MockCoroutineSO>();
	// 	comp.instructionsSO = instructionsSO;
	// 	comp.agent = agent;
	// 	instructionsSO.times = 2;

	// 	yield return new WaitForEndOfFrame();

	// 	comp.onEnd!.AddListener(() => called = true);
	// 	comp.Begin();

	// 	yield return new WaitForEndOfFrame();

	// 	Assert.False(called);
	// }

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
