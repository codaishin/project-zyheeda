using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Routines;
using UnityEngine;
using UnityEngine.TestTools;

public class BaseRoutineRunMBTests : TestCollection
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

	class IntKeyRoutineRunMB : BaseRoutineRunMB<int> { }


	[UnityTest]
	public IEnumerator RunRoutine() {
		var called = 0;
		var run = new GameObject().AddComponent<IntKeyRoutineRunMB>();

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


		yield return new WaitForEndOfFrame();

		run.Apply((42, getRoutine));

		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		Assert.AreEqual(3, called);
	}

	[UnityTest]
	public IEnumerator MoveThroughSubRoutines() {
		var calledSubRoutines = (first: 0, second: 0, third: 0);
		var handle = new GameObject().AddComponent<IntKeyRoutineRunMB>();

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

		yield return new WaitForEndOfFrame();

		handle.Apply((11, getRoutine));

		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		Assert.AreEqual((3, 0, 0), calledSubRoutines);
		handle.Apply((11, getRoutine));

		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		Assert.AreEqual((3, 3, 0), calledSubRoutines);
		handle.Apply((11, getRoutine));

		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		Assert.AreEqual((3, 3, 2), calledSubRoutines);
		handle.Apply((11, getRoutine));

		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		Assert.AreEqual((7, 3, 2), calledSubRoutines);
	}

	[UnityTest]
	public IEnumerator ApplyNextSubRoutine() {
		var called = new[] { 0, 0 };
		var run = new GameObject().AddComponent<IntKeyRoutineRunMB>();

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

		yield return new WaitForEndOfFrame();

		run.Apply((0, getRoutine(0)));

		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		run.Apply((1, getRoutine(1)));

		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		CollectionAssert.AreEqual(new int[] { 3, 3 }, called);
	}

	[UnityTest]
	public IEnumerator IgnoreNullRoutine() {
		var called = 0;
		var run = new GameObject().AddComponent<IntKeyRoutineRunMB>();

		IRoutine? getRoutine() {
			IEnumerator<YieldInstruction> getEnumerator() {
				while (true) {
					++called;
					yield return new WaitForEndOfFrame();
				}
			}
			return new MockRoutine { getEnumerator = getEnumerator };
		}

		yield return new WaitForEndOfFrame();

		run.Apply((-3, getRoutine));

		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		run.Apply((45, () => null));

		yield return new WaitForEndOfFrame();

		Assert.AreEqual(4, called);
	}

	[UnityTest]
	public IEnumerator NullRoutineDoesNotInterfereWithSubRoutineAdvancement() {
		var called = 0;
		var calledNext = 0;
		var run = new GameObject().AddComponent<IntKeyRoutineRunMB>();

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

		yield return new WaitForEndOfFrame();

		run.Apply((33, getRoutine));

		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		run.Apply((-11, () => null));
		run.Apply((33, getRoutine));

		yield return new WaitForEndOfFrame();

		Assert.AreEqual((3, 1), (called, calledNext));
	}

	[UnityTest]
	public
	IEnumerator NullRoutineDoesNotInterfereWithSubRoutineAdvancementOfPrevious() {
		var called = 0;
		var calledNext = 0;
		var run = new GameObject().AddComponent<IntKeyRoutineRunMB>();

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

		yield return new WaitForEndOfFrame();

		run.Apply((22, getRoutine));

		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		run.Apply((22, getRoutine));
		run.Apply((11, () => null));

		yield return new WaitForEndOfFrame();

		Assert.AreEqual((3, 1), (called, calledNext));
	}

	[UnityTest]
	public IEnumerator NotInterferingWithOtherCoroutines() {
		var called = 0;
		var run = new GameObject().AddComponent<IntKeyRoutineRunMB>();

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

		yield return new WaitForEndOfFrame();

		run.StartCoroutine(otherCoroutine());
		run.Apply((55, getRoutine));

		yield return new WaitForEndOfFrame();

		Assert.AreEqual(1, called);
	}

	[UnityTest]
	public IEnumerator StopSoft() {
		var calls = (first: 0, second: 0, third: 0, after: 0);
		var run = new GameObject().AddComponent<IntKeyRoutineRunMB>();

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

		yield return new WaitForEndOfFrame();

		run.Apply((166345, getRoutine));

		yield return new WaitForEndOfFrame();

		run.Stop(166345, 3);

		yield return new WaitForEndOfFrame();

		Assert.AreEqual((2, 0, 0, 1), calls);
	}

	[UnityTest]
	public IEnumerator StopSoftFailsThenCancelRoutine() {
		var calls = (first: 0, second: 0, third: 0, after: 0);
		var run = new GameObject().AddComponent<IntKeyRoutineRunMB>();

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

		yield return new WaitForEndOfFrame();

		run.Apply((33, getRoutine));

		yield return new WaitForEndOfFrame();

		run.Stop(33, 1);

		yield return new WaitForEndOfFrame();

		Assert.AreEqual((2, 0, 0, 0), calls);
	}

	[UnityTest]
	public IEnumerator StopSoftAfterHardStopAllowNewApply() {
		var calls = (first: 0, second: 0, third: 0, after: 0);
		var handle = new GameObject().AddComponent<IntKeyRoutineRunMB>();

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

		yield return new WaitForEndOfFrame();

		handle.Apply((77, getRoutine));

		yield return new WaitForEndOfFrame();

		handle.Stop(77, 1);

		yield return new WaitForEndOfFrame();

		handle.Apply((77, getRoutine));

		Assert.AreEqual((3, 0, 0, 0), calls);
	}

	[UnityTest]
	public IEnumerator DoNotStopDifferentSource() {
		var calls = (first: 0, second: 0, third: 0, after: 0);
		var handle = new GameObject().AddComponent<IntKeyRoutineRunMB>();

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

		yield return new WaitForEndOfFrame();

		handle.Apply((33, getRoutine));

		yield return new WaitForEndOfFrame();

		handle.Stop(55, 3);

		yield return new WaitForEndOfFrame();

		Assert.AreEqual((3, 0, 0, 0), calls);
	}
}
