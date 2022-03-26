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
		public Func<GameObject, Func<bool>?, InstructionsFunc> getInstructions =
			(_, __) => () => new YieldInstruction[0];

		public override InstructionsFunc GetInstructionsFor(
			GameObject agent,
			Func<bool>? run = null
		) => this.getInstructions(agent, run);
	}

	[UnityTest]
	public IEnumerator RunCoroutine() {
		var called = null as GameObject;
		var agent = new GameObject();
		var comp = new GameObject().AddComponent<InstructionsMB>();
		var instructions = ScriptableObject.CreateInstance<MockCoroutineSO>();
		comp.instructionsSO = instructions;
		comp.agent = agent;

		instructions.getInstructions = (agent, run) => {
			called = agent;
			return () => new YieldInstruction[0];
		};

		yield return new WaitForEndOfFrame();

		comp.Apply();

		yield return new WaitForEndOfFrame();

		Assert.AreSame(agent, called);
	}

	[UnityTest]
	public IEnumerator RunCoroutineMultipleAtATime() {
		var called = 0;
		var agent = new GameObject();
		var comp = new GameObject().AddComponent<InstructionsMB>();
		var instructions = ScriptableObject.CreateInstance<MockCoroutineSO>();
		comp.instructionsSO = instructions;
		comp.agent = agent;

		instructions.getInstructions = (_, __) => () => {
			++called;
			return new YieldInstruction[0];
		};

		yield return new WaitForEndOfFrame();

		comp.Apply();
		comp.Apply();

		yield return new WaitForEndOfFrame();

		Assert.AreEqual(2, called);
	}

	[UnityTest]
	public IEnumerator RunCoroutineTwice() {
		var called = 0;
		var agent = new GameObject();
		var comp = new GameObject().AddComponent<InstructionsMB>();
		var instructions = ScriptableObject.CreateInstance<MockCoroutineSO>();
		comp.instructionsSO = instructions;
		comp.agent = agent;

		IEnumerable<YieldInstruction> run() {
			yield return new WaitForEndOfFrame();
			++called;
			yield return new WaitForEndOfFrame();
			++called;
		}

		instructions.getInstructions = (_, __) => run;

		yield return new WaitForEndOfFrame();

		comp.Apply();

		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		Assert.AreEqual(2, called);
	}

	[UnityTest]
	public IEnumerator RunCoroutineOnExternalRunner() {
		var called = 0;
		var agent = new GameObject();
		var comp = new GameObject().AddComponent<InstructionsMB>();
		var external = new GameObject().AddComponent<CoroutineRunnerMB>();
		var instructions = ScriptableObject.CreateInstance<MockCoroutineSO>();
		comp.instructionsSO = instructions;
		comp.agent = agent;
		comp.runner = external;

		IEnumerable<YieldInstruction> run() {
			yield return new WaitForEndOfFrame();
			++called;
			yield return new WaitForEndOfFrame();
			++called;
		}

		instructions.getInstructions = (_, __) => run;

		yield return new WaitForEndOfFrame();

		comp.Apply();

		yield return new WaitForEndOfFrame();

		external.StopAllCoroutines();

		yield return new WaitForEndOfFrame();

		Assert.AreEqual(1, called);
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
		var called = 0;
		var calledOther = 0;
		var agent = new GameObject();
		var comp = new GameObject().AddComponent<InstructionsMB>();
		var instructions = ScriptableObject.CreateInstance<MockCoroutineSO>();
		comp.instructionsSO = instructions;
		comp.agent = agent;
		comp.overrideMode = OverrideMode.Own;

		IEnumerable<YieldInstruction> run() {
			yield return new WaitForEndOfFrame();
			++called;
			yield return new WaitForEndOfFrame();
			++called;
		}

		instructions.getInstructions = (_, __) => run;

		IEnumerator otherRoutine() {
			yield return new WaitForEndOfFrame();
			++calledOther;
			yield return new WaitForEndOfFrame();
			++calledOther;
		}

		yield return new WaitForEndOfFrame();

		comp.StartCoroutine(otherRoutine());
		comp.Apply();
		comp.Apply();

		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		Assert.AreEqual((2, 2), (called, calledOther));
	}

	[UnityTest]
	public IEnumerator OverrideOwnOnExternal() {
		var called = 0;
		var calledOther = 0;
		var agent = new GameObject();
		var comp = new GameObject().AddComponent<InstructionsMB>();
		var external = new GameObject().AddComponent<CoroutineRunnerMB>();
		var instructions = ScriptableObject.CreateInstance<MockCoroutineSO>();
		comp.instructionsSO = instructions;
		comp.agent = agent;
		comp.overrideMode = OverrideMode.Own;
		comp.runner = external;

		IEnumerable<YieldInstruction> run() {
			yield return new WaitForEndOfFrame();
			++called;
			yield return new WaitForEndOfFrame();
			++called;
		}

		instructions.getInstructions = (_, __) => run;

		IEnumerator otherRoutine() {
			yield return new WaitForEndOfFrame();
			++calledOther;
			yield return new WaitForEndOfFrame();
			++calledOther;
		}

		yield return new WaitForEndOfFrame();

		external.StartCoroutine(otherRoutine());
		comp.Apply();
		comp.Apply();

		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		Assert.AreEqual((2, 2), (called, calledOther));
	}

	[UnityTest]
	public IEnumerator Release() {
		var runChecks = new List<bool>();
		var agent = new GameObject();
		var comp = new GameObject().AddComponent<InstructionsMB>();
		var external = new GameObject().AddComponent<CoroutineRunnerMB>();
		var instructions = ScriptableObject.CreateInstance<MockCoroutineSO>();
		comp.instructionsSO = instructions;
		comp.agent = agent;
		comp.overrideMode = OverrideMode.Own;
		comp.runner = external;

		InstructionsFunc getInstructions(GameObject _, Func<bool>? runCheck) {
			IEnumerable<YieldInstruction> instructions() {
				bool keepRunning;
				do {
					yield return new WaitForEndOfFrame();
					keepRunning = runCheck!();
					runChecks.Add(keepRunning);
				} while (keepRunning);
			}
			return instructions;
		}

		instructions.getInstructions = getInstructions;

		yield return new WaitForEndOfFrame();

		comp.Apply();

		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		comp.Release();

		yield return new WaitForEndOfFrame();

		CollectionAssert.AreEqual(
			new bool[] { true, true, true, false },
			runChecks
		);
	}

	[UnityTest]
	public IEnumerator IgnoreNullRoutine() {
		var agent = new GameObject();
		var comp = new GameObject().AddComponent<InstructionsMB>();
		var instructions = ScriptableObject.CreateInstance<MockCoroutineSO>();
		comp.instructionsSO = instructions;
		comp.agent = agent;

		instructions.getInstructions = (_, __) => () => null;

		yield return new WaitForEndOfFrame();

		comp.Apply();

		yield return new WaitForEndOfFrame();

		// we are good here, if no exception was thrown during update
	}

	[UnityTest]
	public IEnumerator NoOverrideWhenNullRoutine() {
		var called = 0;
		var agent = new GameObject();
		var comp = new GameObject().AddComponent<InstructionsMB>();
		var instructions = ScriptableObject.CreateInstance<MockCoroutineSO>();
		comp.instructionsSO = instructions;
		comp.agent = agent;
		comp.overrideMode = OverrideMode.All;

		instructions.getInstructions = (_, __) => () => null;

		IEnumerator<YieldInstruction> otherRoutine() {
			yield return new WaitForEndOfFrame();
			++called;
			yield return new WaitForEndOfFrame();
			++called;
			yield return new WaitForEndOfFrame();
			++called;
		}

		yield return new WaitForEndOfFrame();

		comp.StartCoroutine(otherRoutine());
		comp.Apply();

		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		Assert.AreEqual(3, called);
	}
}
