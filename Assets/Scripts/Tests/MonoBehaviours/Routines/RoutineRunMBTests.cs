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
	class MockRoutine : IRoutine
	{
		public Func<bool> nextSubRoutine =
			() => false;
		public Func<IEnumerator<YieldInstruction?>> getEnumerator =
			() =>
				Enumerable
					.Empty<YieldInstruction>()
					.GetEnumerator();

		public IEnumerator<YieldInstruction?> GetEnumerator() =>
			this.getEnumerator();
		IEnumerator IEnumerable.GetEnumerator() =>
			this.getEnumerator();
		public bool NextSubRoutine() =>
			this.nextSubRoutine();
	}

	class MockGetRoutine : IFactory
	{
		public Func<IRoutine?> getRoutine = () => null;

		public IRoutine? GetRoutine() =>
			this.getRoutine();
	}

	[UnityTest]
	public IEnumerator RunRoutine() {
		var called = 0;
		var handle = new GameObject().AddComponent<RoutineRunMB>();

		IRoutine? getRoutine() {
			IEnumerator<YieldInstruction> getEnumerator() {
				++called;
				yield return new WaitForEndOfFrame();
				++called;
				yield return new WaitForEndOfFrame();
				++called;
				yield return new WaitForEndOfFrame();
			}
			return new MockRoutine { getEnumerator = getEnumerator };
		}

		var source = new MockGetRoutine { getRoutine = getRoutine };

		yield return new WaitForEndOfFrame();

		handle.Apply(source);

		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		Assert.AreEqual(3, called);
	}

	[UnityTest]
	public IEnumerator MoveThroughSubRoutines() {
		var calledSubRoutines = (first: 0, second: 0, third: 0);
		var handle = new GameObject().AddComponent<RoutineRunMB>();

		IRoutine? getRoutine() {
			var subRoutineIndex = 0;

			IEnumerator<YieldInstruction> getEnumerator() {
				while (subRoutineIndex == 0) {
					++calledSubRoutines.first;
					yield return new WaitForEndOfFrame();
				}
				while (subRoutineIndex == 1) {
					++calledSubRoutines.second;
					yield return new WaitForEndOfFrame();
				}
				while (subRoutineIndex == 2) {
					++calledSubRoutines.third;
					yield return new WaitForEndOfFrame();
				}
			}
			return new MockRoutine {
				getEnumerator = getEnumerator,
				nextSubRoutine = () => ++subRoutineIndex < 3,
			};
		}

		var source = new MockGetRoutine { getRoutine = getRoutine };

		yield return new WaitForEndOfFrame();

		handle.Apply(source);

		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		Assert.AreEqual((3, 0, 0), calledSubRoutines);
		handle.Apply(source);

		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		Assert.AreEqual((3, 3, 0), calledSubRoutines);
		handle.Apply(source);

		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		Assert.AreEqual((3, 3, 2), calledSubRoutines);
		handle.Apply(source);

		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		Assert.AreEqual((7, 3, 2), calledSubRoutines);
	}

	[UnityTest]
	public IEnumerator ApplyNextSubRoutine() {
		var called = new[] { 0, 0 };
		var handle = new GameObject().AddComponent<RoutineRunMB>();

		Func<int, Func<IRoutine?>> getRoutine = routineIndex => () => {
			var run = true;

			IEnumerator<YieldInstruction> getEnumerator() {
				while (run) {
					++called[routineIndex];
					yield return new WaitForEndOfFrame();
				}
			}
			return new MockRoutine {
				getEnumerator = getEnumerator,
				nextSubRoutine = () => run = false,
			};
		};

		var sourceA = new MockGetRoutine { getRoutine = getRoutine(0) };
		var sourceB = new MockGetRoutine { getRoutine = getRoutine(1) };

		yield return new WaitForEndOfFrame();

		handle.Apply(sourceA);

		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		handle.Apply(sourceB);

		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		CollectionAssert.AreEqual(new int[] { 3, 3 }, called);
	}

	[UnityTest]
	public IEnumerator IgnoreNullRoutine() {
		var called = 0;
		var handle = new GameObject().AddComponent<RoutineRunMB>();

		IRoutine? getRoutine() {
			IEnumerator<YieldInstruction> getEnumerator() {
				while (true) {
					++called;
					yield return new WaitForEndOfFrame();
				}
			}
			return new MockRoutine { getEnumerator = getEnumerator };
		}

		var sourceA = new MockGetRoutine { getRoutine = getRoutine };
		var sourceB = new MockGetRoutine { getRoutine = () => null };

		yield return new WaitForEndOfFrame();

		handle.Apply(sourceA);

		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		handle.Apply(sourceB);

		yield return new WaitForEndOfFrame();

		Assert.AreEqual(4, called);
	}

	[UnityTest]
	public IEnumerator NullRoutineDoesNotInterfereWithSubRoutineAdvancement() {
		var called = 0;
		var calledNext = 0;
		var handle = new GameObject().AddComponent<RoutineRunMB>();

		IRoutine? getRoutine() {
			var index = 0;

			IEnumerator<YieldInstruction> getEnumerator() {
				while (index == 0) {
					++called;
					yield return new WaitForEndOfFrame();
				}
				++calledNext;
				yield return new WaitForEndOfFrame();
			}
			return new MockRoutine {
				getEnumerator = getEnumerator,
				nextSubRoutine = () => index++ < 1,
			};
		}

		var sourceA = new MockGetRoutine { getRoutine = getRoutine };
		var sourceB = new MockGetRoutine { getRoutine = () => null };

		yield return new WaitForEndOfFrame();

		handle.Apply(sourceA);

		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		handle.Apply(sourceB);
		handle.Apply(sourceA);

		yield return new WaitForEndOfFrame();

		Assert.AreEqual((3, 1), (called, calledNext));
	}

	[UnityTest]
	public
	IEnumerator NullRoutineDoesNotInterfereWithSubRoutineAdvancementOfPrevious() {
		var called = 0;
		var calledNext = 0;
		var handle = new GameObject().AddComponent<RoutineRunMB>();
		var sourceA = new MockGetRoutine();
		var sourceB = new MockGetRoutine();

		IRoutine? getInstructions() {
			var index = 0;

			IEnumerator<YieldInstruction> getEnumerator() {
				while (index == 0) {
					++called;
					yield return new WaitForEndOfFrame();
				}
				++calledNext;
				yield return new WaitForEndOfFrame();
			}
			return new MockRoutine {
				getEnumerator = getEnumerator,
				nextSubRoutine = () => index++ < 1,
			};
		}

		sourceA.getRoutine = getInstructions;
		sourceB.getRoutine = () => null;

		yield return new WaitForEndOfFrame();

		handle.Apply(sourceA);

		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		handle.Apply(sourceA);
		handle.Apply(sourceB);

		yield return new WaitForEndOfFrame();

		Assert.AreEqual((3, 1), (called, calledNext));
	}

	[UnityTest]
	public IEnumerator NotInterferingWithOtherCoroutines() {
		var called = 0;
		var handle = new GameObject().AddComponent<RoutineRunMB>();

		IRoutine? getRoutine() {
			IEnumerator<YieldInstruction> getEnumerator() {
				yield break;
			}
			return new MockRoutine { getEnumerator = getEnumerator };
		}

		IEnumerator<YieldInstruction> otherCoroutine() {
			yield return new WaitForEndOfFrame();
			++called;
		}

		var source = new MockGetRoutine { getRoutine = getRoutine };

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

		run.delayApply = true;

		IRoutine? getRoutine() {
			IEnumerator<YieldInstruction> getEnumerator() {
				while (true) {
					++called;
					yield return new WaitForEndOfFrame();
				}
			}
			return new MockRoutine { getEnumerator = getEnumerator };
		}

		var source = new MockGetRoutine { getRoutine = getRoutine };

		yield return new WaitForEndOfFrame();

		run.Apply(source);

		Assert.AreEqual(0, called);

		yield return new WaitForEndOfFrame();

		Assert.AreEqual(1, called);

		yield return new WaitForEndOfFrame();

		Assert.AreEqual(2, called);
	}

	[UnityTest]
	public IEnumerator StopSoft() {
		var calls = (first: 0, second: 0, third: 0, after: 0);
		var handle = new GameObject().AddComponent<RoutineRunMB>();

		IRoutine? getRoutine() {
			var subRoutineIndex = 0;

			IEnumerator<YieldInstruction> getEnumerator() {
				while (subRoutineIndex == 0) {
					++calls.first;
					yield return new WaitForEndOfFrame();
				}
				while (subRoutineIndex == 1) {
					++calls.second;
					yield return new WaitForEndOfFrame();
				}
				while (subRoutineIndex == 2) {
					++calls.third;
					yield return new WaitForEndOfFrame();
				}
				++calls.after;
			}
			return new MockRoutine {
				getEnumerator = getEnumerator,
				nextSubRoutine = () => ++subRoutineIndex < 3,
			};
		}

		var source = new MockGetRoutine { getRoutine = getRoutine };

		yield return new WaitForEndOfFrame();

		handle.Apply(source);

		yield return new WaitForEndOfFrame();

		handle.Stop(source, 3);

		yield return new WaitForEndOfFrame();

		Assert.AreEqual((2, 0, 0, 1), calls);
	}

	[UnityTest]
	public IEnumerator StopSoftFailsThenCancelRoutine() {
		var calls = (first: 0, second: 0, third: 0, after: 0);
		var handle = new GameObject().AddComponent<RoutineRunMB>();

		IRoutine? getRoutine() {
			var subRoutineIndex = 0;

			IEnumerator<YieldInstruction> getEnumerator() {
				while (subRoutineIndex == 0) {
					++calls.first;
					yield return new WaitForEndOfFrame();
				}
				while (subRoutineIndex == 1) {
					++calls.second;
					yield return new WaitForEndOfFrame();
				}
				while (subRoutineIndex == 2) {
					++calls.third;
					yield return new WaitForEndOfFrame();
				}
				++calls.after;
			}
			return new MockRoutine {
				getEnumerator = getEnumerator,
				nextSubRoutine = () => ++subRoutineIndex < 3,
			};
		}

		var source = new MockGetRoutine { getRoutine = getRoutine };

		yield return new WaitForEndOfFrame();

		handle.Apply(source);

		yield return new WaitForEndOfFrame();

		handle.Stop(source, 1);

		yield return new WaitForEndOfFrame();

		Assert.AreEqual((2, 0, 0, 0), calls);
	}

	[UnityTest]
	public IEnumerator StopSoftAfterHardStopAllowNewApply() {
		var calls = (first: 0, second: 0, third: 0, after: 0);
		var handle = new GameObject().AddComponent<RoutineRunMB>();

		IRoutine? getRoutine() {
			var subRoutineIndex = 0;

			IEnumerator<YieldInstruction> getEnumerator() {
				while (subRoutineIndex == 0) {
					++calls.first;
					yield return new WaitForEndOfFrame();
				}
				while (subRoutineIndex == 1) {
					++calls.second;
					yield return new WaitForEndOfFrame();
				}
				while (subRoutineIndex == 2) {
					++calls.third;
					yield return new WaitForEndOfFrame();
				}
				++calls.after;
			}
			return new MockRoutine {
				getEnumerator = getEnumerator,
				nextSubRoutine = () => ++subRoutineIndex < 3,
			};
		}

		var source = new MockGetRoutine { getRoutine = getRoutine };

		yield return new WaitForEndOfFrame();

		handle.Apply(source);

		yield return new WaitForEndOfFrame();

		handle.Stop(source, 1);

		yield return new WaitForEndOfFrame();

		handle.Apply(source);

		Assert.AreEqual((3, 0, 0, 0), calls);
	}

	[UnityTest]
	public IEnumerator DoNotStopDifferentSource() {
		var calls = (first: 0, second: 0, third: 0, after: 0);
		var handle = new GameObject().AddComponent<RoutineRunMB>();

		IRoutine? getRoutine() {
			var subRoutineIndex = 0;

			IEnumerator<YieldInstruction> getEnumerator() {
				while (subRoutineIndex == 0) {
					++calls.first;
					yield return new WaitForEndOfFrame();
				}
				while (subRoutineIndex == 1) {
					++calls.second;
					yield return new WaitForEndOfFrame();
				}
				while (subRoutineIndex == 2) {
					++calls.third;
					yield return new WaitForEndOfFrame();
				}
				++calls.after;
			}
			return new MockRoutine {
				getEnumerator = getEnumerator,
				nextSubRoutine = () => ++subRoutineIndex < 3,
			};
		}

		var sourceA = new MockGetRoutine { getRoutine = getRoutine };
		var sourceB = new MockGetRoutine { getRoutine = () => null };

		yield return new WaitForEndOfFrame();

		handle.Apply(sourceA);

		yield return new WaitForEndOfFrame();

		handle.Stop(sourceB, 3);

		yield return new WaitForEndOfFrame();

		Assert.AreEqual((3, 0, 0, 0), calls);
	}
}
