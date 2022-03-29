using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class RunInstructionsMBTests : TestCollection
{
	class MockCoroutineSO : ScriptableObject, IInstructions
	{
		public Func<GameObject, InstructionsFunc> getInstructions =
			_ => _ => new YieldInstruction[0];

		public InstructionsFunc GetInstructionsFor(GameObject agent) =>
			this.getInstructions(agent);
	}

	[UnityTest]
	public IEnumerator RunCoroutine() {
		var called = null as GameObject;
		var agent = new GameObject();
		var comp = new GameObject().AddComponent<RunInstructionsMB>();
		var instructions = ScriptableObject.CreateInstance<MockCoroutineSO>();
		var runner = new GameObject().AddComponent<CoroutineRunnerMB>();
		comp.instructions = Reference<IInstructions>.ScriptableObject(instructions);
		comp.agent = agent;

		instructions.getInstructions = agent => {
			called = agent;
			return _ => new YieldInstruction[0];
		};

		yield return new WaitForEndOfFrame();

		comp.Apply(runner);

		yield return new WaitForEndOfFrame();

		Assert.AreSame(agent, called);
	}

	[UnityTest]
	public IEnumerator RunCoroutineMultipleAtATime() {
		var called = 0;
		var agent = new GameObject();
		var comp = new GameObject().AddComponent<RunInstructionsMB>();
		var instructions = ScriptableObject.CreateInstance<MockCoroutineSO>();
		var runner = new GameObject().AddComponent<CoroutineRunnerMB>();
		comp.instructions = Reference<IInstructions>.ScriptableObject(instructions);
		comp.agent = agent;

		instructions.getInstructions = _ => _ => {
			++called;
			return new YieldInstruction[0];
		};

		yield return new WaitForEndOfFrame();

		comp.Apply(runner);
		comp.Apply(runner);

		yield return new WaitForEndOfFrame();

		Assert.AreEqual(2, called);
	}

	[UnityTest]
	public IEnumerator RunCoroutineTwice() {
		var called = 0;
		var agent = new GameObject();
		var comp = new GameObject().AddComponent<RunInstructionsMB>();
		var instructions = ScriptableObject.CreateInstance<MockCoroutineSO>();
		var runner = new GameObject().AddComponent<CoroutineRunnerMB>();
		comp.instructions = Reference<IInstructions>.ScriptableObject(instructions);
		comp.agent = agent;

		IEnumerable<YieldInstruction> run(Func<bool>? runCheck) {
			yield return new WaitForEndOfFrame();
			++called;
			yield return new WaitForEndOfFrame();
			++called;
		}

		instructions.getInstructions = _ => run;

		yield return new WaitForEndOfFrame();

		comp.Apply(runner);

		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		Assert.AreEqual(2, called);
	}

	[UnityTest]
	public IEnumerator OverrideAll() {
		var calledOther = false;
		var agent = new GameObject();
		var comp = new GameObject().AddComponent<RunInstructionsMB>();
		var instructions = ScriptableObject.CreateInstance<MockCoroutineSO>();
		var runner = new GameObject().AddComponent<CoroutineRunnerMB>();
		comp.instructions = Reference<IInstructions>.ScriptableObject(instructions);
		comp.agent = agent;

		IEnumerator otherRoutine() {
			yield return new WaitForEndOfFrame();
			calledOther = true;
		}

		yield return new WaitForEndOfFrame();

		runner.StartCoroutine(otherRoutine());
		comp.Apply(runner);

		yield return new WaitForEndOfFrame();

		Assert.False(calledOther);
	}

	[UnityTest]
	public IEnumerator Release() {
		var runChecks = new List<bool>();
		var agent = new GameObject();
		var comp = new GameObject().AddComponent<RunInstructionsMB>();
		var runner = new GameObject().AddComponent<CoroutineRunnerMB>();
		var instructions = ScriptableObject.CreateInstance<MockCoroutineSO>();
		comp.instructions = Reference<IInstructions>.ScriptableObject(instructions);
		comp.agent = agent;

		InstructionsFunc getInstructions(GameObject _) {
			IEnumerable<YieldInstruction> instructions(Func<bool>? runCheck) {
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

		comp.Apply(runner);

		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		comp.Release(runner);

		yield return new WaitForEndOfFrame();

		CollectionAssert.AreEqual(
			new bool[] { true, true, true, false },
			runChecks
		);
	}

	[UnityTest]
	public IEnumerator ReleaseOnlyOwn() {
		var runChecks = new List<bool>();
		var agent = new GameObject();
		var compA = new GameObject().AddComponent<RunInstructionsMB>();
		var compB = new GameObject().AddComponent<RunInstructionsMB>();
		var runner = new GameObject().AddComponent<CoroutineRunnerMB>();
		var instructions = ScriptableObject.CreateInstance<MockCoroutineSO>();
		compA.instructions = Reference<IInstructions>
			.ScriptableObject(instructions);
		compA.agent = agent;
		compB.instructions = Reference<IInstructions>
			.ScriptableObject(instructions);
		compB.agent = agent;

		InstructionsFunc getInstructions(GameObject _) {
			IEnumerable<YieldInstruction> instructions(Func<bool>? runCheck) {
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

		compA.Apply(runner);
		compB.Apply(runner);

		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		compA.Release(runner);

		yield return new WaitForEndOfFrame();

		CollectionAssert.AreEqual(
			new bool[] { true, true, true, true },
			runChecks
		);
	}

	[UnityTest]
	public IEnumerator IgnoreNullRoutine() {
		var agent = new GameObject();
		var comp = new GameObject().AddComponent<RunInstructionsMB>();
		var instructions = ScriptableObject.CreateInstance<MockCoroutineSO>();
		var runner = new GameObject().AddComponent<CoroutineRunnerMB>();
		comp.instructions = Reference<IInstructions>.ScriptableObject(instructions);
		comp.agent = agent;

		instructions.getInstructions = _ => _ => null;

		yield return new WaitForEndOfFrame();

		comp.Apply(runner);

		yield return new WaitForEndOfFrame();

		// we are good here, if no exception was thrown during update
	}

	[UnityTest]
	public IEnumerator NoOverrideWhenNullRoutine() {
		var called = 0;
		var agent = new GameObject();
		var comp = new GameObject().AddComponent<RunInstructionsMB>();
		var instructions = ScriptableObject.CreateInstance<MockCoroutineSO>();
		var runner = new GameObject().AddComponent<CoroutineRunnerMB>();
		comp.instructions = Reference<IInstructions>.ScriptableObject(instructions);
		comp.agent = agent;

		instructions.getInstructions = _ => _ => null;

		IEnumerator<YieldInstruction> otherRoutine() {
			yield return new WaitForEndOfFrame();
			++called;
			yield return new WaitForEndOfFrame();
			++called;
			yield return new WaitForEndOfFrame();
			++called;
		}

		yield return new WaitForEndOfFrame();

		runner.StartCoroutine(otherRoutine());
		comp.Apply(runner);

		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		Assert.AreEqual(3, called);
	}
}
