using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Routines;
using UnityEngine;
using UnityEngine.TestTools;

public class RoutineRunMBTests : TestCollection
{
	class MockSubRoutine : IRoutine
	{
		public Action release =
			() => { };
		public Func<IEnumerator<YieldInstruction?>> getEnumerator =
			() =>
				Enumerable
					.Empty<YieldInstruction>()
					.GetEnumerator();

		public IEnumerator<YieldInstruction?> GetEnumerator() =>
			this.getEnumerator();
		IEnumerator IEnumerable.GetEnumerator() =>
			this.getEnumerator();
		public void Switch() =>
			this.release();
	}

	class MockGetInstructions : IFactory
	{
		public Func<IRoutine?> getInstructions = () => null;

		public IRoutine? GetRoutine() =>
			this.getInstructions();
	}

	[UnityTest]
	public IEnumerator ApplyInstructions() {
		var called = 0;
		var handle = new GameObject().AddComponent<RoutineRunMB>();
		var source = new MockGetInstructions();

		IRoutine? getInstructions() {
			IEnumerator<YieldInstruction> getEnumerator() {
				yield return new WaitForEndOfFrame();
				++called;
				yield return new WaitForEndOfFrame();
				++called;
				yield return new WaitForEndOfFrame();
				++called;
			}
			return new MockSubRoutine { getEnumerator = getEnumerator };
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
	public IEnumerator ReleaseInstructions() {
		var called = 0;
		var handle = new GameObject().AddComponent<RoutineRunMB>();
		var source = new MockGetInstructions();

		IRoutine? getInstructions() {
			var run = true;

			IEnumerator<YieldInstruction> getEnumerator() {
				while (run) {
					yield return new WaitForEndOfFrame();
					++called;
				}
			}
			return new MockSubRoutine {
				getEnumerator = getEnumerator,
				release = () => run = false,
			};
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
		var handle = new GameObject().AddComponent<RoutineRunMB>();
		var sourceA = new MockGetInstructions();
		var sourceB = new MockGetInstructions();

		IRoutine? getInstructions() {
			var run = true;

			IEnumerator<YieldInstruction> getEnumerator() {
				while (run) {
					yield return new WaitForEndOfFrame();
					++called;
				}
			}
			return new MockSubRoutine {
				getEnumerator = getEnumerator,
				release = () => run = false,
			};
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
		var handle = new GameObject().AddComponent<RoutineRunMB>();
		var source = new MockGetInstructions();

		IRoutine? getInstructions() {
			var run = true;
			called = 0;

			IEnumerator<YieldInstruction> getEnumerator() {
				while (run) {
					yield return new WaitForEndOfFrame();
					++called;
				}
			}
			return new MockSubRoutine {
				getEnumerator = getEnumerator,
				release = () => run = false,
			};
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
		var handle = new GameObject().AddComponent<RoutineRunMB>();
		var sourceA = new MockGetInstructions();
		var sourceB = new MockGetInstructions();

		IRoutine? getInstructions() {
			var run = true;

			IEnumerator<YieldInstruction> getEnumerator() {
				while (run) {
					yield return new WaitForEndOfFrame();
					++called;
				}
			}
			return new MockSubRoutine {
				getEnumerator = getEnumerator,
				release = () => run = false,
			};
		}

		sourceA.getInstructions = getInstructions;
		sourceB.getInstructions = () => null;

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
		var handle = new GameObject().AddComponent<RoutineRunMB>();
		var sourceA = new MockGetInstructions();
		var sourceB = new MockGetInstructions();

		IRoutine? getInstructions() {
			var run = true;

			IEnumerator<YieldInstruction> getEnumerator() {
				while (run) {
					yield return new WaitForEndOfFrame();
					++called;
				}
			}
			return new MockSubRoutine {
				getEnumerator = getEnumerator,
				release = () => run = false,
			};
		}

		sourceA.getInstructions = getInstructions;
		sourceB.getInstructions = () => null;

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
		var handle = new GameObject().AddComponent<RoutineRunMB>();
		var sourceA = new MockGetInstructions();
		var sourceB = new MockGetInstructions();

		IRoutine? getInstructions() {
			var run = true;

			IEnumerator<YieldInstruction> getEnumerator() {
				while (run) {
					yield return new WaitForEndOfFrame();
					++called;
				}
			}
			return new MockSubRoutine {
				getEnumerator = getEnumerator,
				release = () => run = false,
			};
		}

		sourceA.getInstructions = getInstructions;
		sourceB.getInstructions = () => null;

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
		var handle = new GameObject().AddComponent<RoutineRunMB>();
		var source = new MockGetInstructions();

		IRoutine? getInstructions() {
			IEnumerator<YieldInstruction> getEnumerator() {
				yield break;
			}
			return new MockSubRoutine {
				getEnumerator = getEnumerator,
				release = () => { },
			};
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
		var run = new GameObject().AddComponent<RoutineRunMB>();
		var source = new MockGetInstructions();

		run.delayApply = true;

		IRoutine? getInstructions() {
			var run = true;

			IEnumerator<YieldInstruction> getEnumerator() {
				while (run) {
					++called;
					yield return new WaitForEndOfFrame();
				}
			}
			return new MockSubRoutine {
				getEnumerator = getEnumerator,
				release = () => run = false,
			};
		}

		source.getInstructions = getInstructions;

		yield return new WaitForEndOfFrame();

		run.Apply(source);

		Assert.AreEqual(0, called);

		yield return new WaitForEndOfFrame();

		Assert.AreEqual(1, called);

		yield return new WaitForEndOfFrame();

		Assert.AreEqual(2, called);
	}

	[UnityTest]
	public IEnumerator ReleaseInstructionsDelayedOneFrame() {
		var called = 0;
		var handle = new GameObject().AddComponent<RoutineRunMB>();
		var source = new MockGetInstructions();

		handle.delayApply = true;

		IRoutine? getInstructions() {
			var run = true;

			IEnumerator<YieldInstruction> getEnumerator() {
				while (run) {
					yield return new WaitForEndOfFrame();
					++called;
				}
			}
			return new MockSubRoutine {
				getEnumerator = getEnumerator,
				release = () => run = false,
			};
		}

		source.getInstructions = getInstructions;

		yield return new WaitForEndOfFrame();

		handle.Apply(source);
		handle.Release(source);

		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		Assert.AreEqual(1, called);
	}
}
