using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class InstructionsRunMBTests : TestCollection
{
	class MockGetInstructions : IInstructions
	{
		public Func<Func<bool>, IEnumerator<YieldInstruction?>?> getInstructions =
			_ => null;
		public IEnumerator<YieldInstruction?>? GetInstructions(Func<bool> run) =>
			this.getInstructions(run);
	}

	[UnityTest]
	public IEnumerator ApplyInstructions() {
		var called = 0;
		var handle = new GameObject().AddComponent<InstructionsRunMB>();
		var source = new MockGetInstructions();

		IEnumerator<YieldInstruction> getInstructions(Func<bool> _) {
			yield return new WaitForEndOfFrame();
			++called;
			yield return new WaitForEndOfFrame();
			++called;
			yield return new WaitForEndOfFrame();
			++called;
		}

		source.getInstructions = getInstructions;

		yield return new WaitForEndOfFrame();

		handle.Apply(source);

		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		Assert.AreEqual(3, called);
	}

	[UnityTest]
	public IEnumerator ApplyInstructionsRunCallbackTrue() {
		var called = false;
		var handle = new GameObject().AddComponent<InstructionsRunMB>();
		var source = new MockGetInstructions();

		IEnumerator<YieldInstruction> getInstructions(Func<bool> run) {
			yield return new WaitForEndOfFrame();
			called = run();
		}

		source.getInstructions = getInstructions;

		yield return new WaitForEndOfFrame();

		handle.Apply(source);

		yield return new WaitForEndOfFrame();

		Assert.True(called);
	}

	[UnityTest]
	public IEnumerator ReleaseInstructions() {
		var called = 0;
		var handle = new GameObject().AddComponent<InstructionsRunMB>();
		var source = new MockGetInstructions();

		IEnumerator<YieldInstruction> getInstructions(Func<bool> run) {
			while (run()) {
				yield return new WaitForEndOfFrame();
				++called;
			}
		}

		source.getInstructions = getInstructions;

		yield return new WaitForEndOfFrame();

		handle.Apply(source);

		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		handle.Release(source);

		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		Assert.AreEqual(3, called);
	}

	[UnityTest]
	public IEnumerator DoNotReleaseInstructionsThatIsNotRunning() {
		var called = 0;
		var handle = new GameObject().AddComponent<InstructionsRunMB>();
		var sourceA = new MockGetInstructions();
		var sourceB = new MockGetInstructions();

		IEnumerator<YieldInstruction> getInstructions(Func<bool> run) {
			while (run()) {
				yield return new WaitForEndOfFrame();
				++called;
			}
		}

		sourceA.getInstructions = getInstructions;

		yield return new WaitForEndOfFrame();

		handle.Apply(sourceA);

		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		handle.Release(sourceB);

		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		Assert.AreEqual(4, called);
	}

	[UnityTest]
	public IEnumerator OnlyRunLatest() {
		var called = 0;
		var handle = new GameObject().AddComponent<InstructionsRunMB>();
		var source = new MockGetInstructions();

		IEnumerator<YieldInstruction> getInstructions(Func<bool> run) {
			called = 0;
			while (run()) {
				yield return new WaitForEndOfFrame();
				++called;
			}
		}

		source.getInstructions = getInstructions;

		yield return new WaitForEndOfFrame();

		handle.Apply(source);

		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		handle.Apply(source);

		yield return new WaitForEndOfFrame();

		Assert.AreEqual(1, called);
	}

	[UnityTest]
	public IEnumerator IgnoreNullRoutine() {
		var called = 0;
		var handle = new GameObject().AddComponent<InstructionsRunMB>();
		var sourceA = new MockGetInstructions();
		var sourceB = new MockGetInstructions();

		IEnumerator<YieldInstruction> getInstructions(Func<bool> run) {
			called = 0;
			while (run()) {
				yield return new WaitForEndOfFrame();
				++called;
			}
		}

		sourceA.getInstructions = getInstructions;
		sourceB.getInstructions = _ => null;

		yield return new WaitForEndOfFrame();

		handle.Apply(sourceA);

		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		handle.Apply(sourceB);

		yield return new WaitForEndOfFrame();

		Assert.AreEqual(3, called);
	}

	[UnityTest]
	public IEnumerator NullRoutineDoesNotInterfereWithSubsequentRelease() {
		var called = 0;
		var handle = new GameObject().AddComponent<InstructionsRunMB>();
		var sourceA = new MockGetInstructions();
		var sourceB = new MockGetInstructions();

		IEnumerator<YieldInstruction> getInstructions(Func<bool> run) {
			called = 0;
			while (run()) {
				yield return new WaitForEndOfFrame();
				++called;
			}
		}

		sourceA.getInstructions = getInstructions;
		sourceB.getInstructions = _ => null;

		yield return new WaitForEndOfFrame();

		handle.Apply(sourceA);

		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		handle.Apply(sourceB);
		handle.Release(sourceA);

		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		Assert.AreEqual(3, called);
	}

	[UnityTest]
	public IEnumerator NullRoutineDoesNotInterfereWithPreviousRelease() {
		var called = 0;
		var handle = new GameObject().AddComponent<InstructionsRunMB>();
		var sourceA = new MockGetInstructions();
		var sourceB = new MockGetInstructions();

		IEnumerator<YieldInstruction> getInstructions(Func<bool> run) {
			called = 0;
			while (run()) {
				yield return new WaitForEndOfFrame();
				++called;
			}
		}

		sourceA.getInstructions = getInstructions;
		sourceB.getInstructions = _ => null;

		yield return new WaitForEndOfFrame();

		handle.Apply(sourceA);

		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		handle.Release(sourceA);
		handle.Apply(sourceB);

		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		Assert.AreEqual(3, called);
	}

	[UnityTest]
	public IEnumerator NotInterferingWithOtherCoroutines() {
		var called = 0;
		var handle = new GameObject().AddComponent<InstructionsRunMB>();
		var source = new MockGetInstructions();

		IEnumerator<YieldInstruction> getInstructions(Func<bool> run) {
			yield break;
		}

		IEnumerator<YieldInstruction> otherCoroutine() {
			yield return new WaitForEndOfFrame();
			++called;
		}

		source.getInstructions = getInstructions;

		yield return new WaitForEndOfFrame();

		handle.StartCoroutine(otherCoroutine());
		handle.Apply(source);

		yield return new WaitForEndOfFrame();

		Assert.AreEqual(1, called);
	}

	[UnityTest]
	public IEnumerator ApplyInstructionsDelayedOneFrame() {
		var called = 0;
		var handle = new GameObject().AddComponent<InstructionsRunMB>();
		var source = new MockGetInstructions();

		handle.delayApply = true;

		IEnumerator<YieldInstruction> getInstructions(Func<bool> _) {
			++called;
			yield return new WaitForEndOfFrame();
			++called;
		}

		source.getInstructions = getInstructions;

		yield return new WaitForEndOfFrame();

		handle.Apply(source);

		Assert.AreEqual(0, called);

		yield return new WaitForEndOfFrame();

		Assert.AreEqual(1, called);

		yield return new WaitForEndOfFrame();

		Assert.AreEqual(2, called);
	}
}
