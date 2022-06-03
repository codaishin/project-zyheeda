using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace Routines
{
	public class BaseFuncFactoryTests : TestCollection
	{
		private
		static
		Func<Reference<IModifierFactory>, ModifierData> ToModifierData(
			ModifierHook hook
		) {
			return factory => new ModifierData { hook = hook, factory = factory, };
		}

		class MockFactory : BaseFuncFactory<Transform>
		{
			public Func<GameObject, Transform> concreteAgent = a => a.transform;
			public Func<Transform, SubRoutineFn[]> subRoutines = _ => new SubRoutineFn[0];
			public Action<Data> extendData = _ => { };

			protected override SubRoutineFn[] SubRoutines(Transform agent) {
				return this.subRoutines(agent);
			}

			protected override Transform ConcreteAgent(GameObject agent) {
				return this.concreteAgent(agent);
			}

			protected
			override
			void ExtendData(Data data) {
				this.extendData(data);
			}
		}

		class MockModifierFactory : MonoBehaviour, IModifierFactory
		{
			public Func<GameObject, ModifierFn> getModifierFnFor = _ => _ => null;

			public ModifierFn GetModifierFnFor(GameObject agent) {
				return this.getModifierFnFor(agent);
			}
		}

		[Test]
		public void GetConcreteAgent() {
			var called = null as Transform;
			var routineFactory = new MockFactory();
			var agent = new GameObject();

			routineFactory.subRoutines = a => {
				called = a;
				return new SubRoutineFn[0];
			};

			_ = routineFactory.GetRoutineFnFor(agent);

			Assert.AreEqual(agent.transform, called);
		}

		[Test]
		public void RunRoutine() {
			var called = null as Transform;
			var routineFactory = new MockFactory();
			var agent = new GameObject();

			IEnumerable<YieldInstruction> moveUp(Transform transform) {
				yield return new WaitForEndOfFrame();
				transform.position += Vector3.up;
				yield return new WaitForEndOfFrame();
				transform.position += Vector3.up;
				yield return new WaitForEndOfFrame();
				transform.position += Vector3.up;
			}

			routineFactory.subRoutines = a => new SubRoutineFn[] { _ => moveUp(a) };

			var routineFn = routineFactory.GetRoutineFnFor(agent)!;

			foreach (var _ in routineFn().OrEmpty()) ;

			Assert.AreEqual(Vector3.up * 3, agent.transform.position);
		}

		[Test]
		public void DataAndAgentToModifierFactory() {
			var data = new List<Data>();
			var agents = new List<GameObject>();
			var agent = new GameObject();
			var routineFactory = new MockFactory();
			var yieldsFn = (SubRoutineFn)(_ => new YieldInstruction[] {
				new WaitForEndOfFrame(),
				new WaitForEndOfFrame(),
				new WaitForEndOfFrame(),
			});
			var allHooks =
				ModifierHook.OnBeginSubRoutine |
				ModifierHook.OnUpdateSubRoutine |
				ModifierHook.OnEndSubroutine;
			var modifiers = new[] {
				new GameObject().AddComponent<MockModifierFactory>(),
				new GameObject().AddComponent<MockModifierFactory>(),
			};

			foreach (var modifier in modifiers) {
				modifier.getModifierFnFor = a => {
					agents.Add(a);
					return d => () => data.Add(d);
				};
			}

			routineFactory.modifiers = modifiers
				.Select(Reference<IModifierFactory>.Component)
				.Select(BaseFuncFactoryTests.ToModifierData(allHooks))
				.ToArray();
			routineFactory.subRoutines = _ => new SubRoutineFn[] { yieldsFn };

			var routineFn = routineFactory.GetRoutineFnFor(agent);

			foreach (var _ in routineFn().OrEmpty()) ;

			Assert.AreEqual(1, data.Distinct().Count());
			CollectionAssert.AreEqual(new[] { agent, agent }, agents);
		}

		[Test]
		public void OnBegin() {
			var called = 0;
			var agent = new GameObject();
			var routineFactory = new MockFactory();
			var yieldsFn = (SubRoutineFn)(_ => new YieldInstruction[] {
				new WaitForEndOfFrame(),
				new WaitForEndOfFrame(),
				new WaitForEndOfFrame(),
			});
			var modifiers = new MockModifierFactory[] {
				new GameObject().AddComponent<MockModifierFactory>(),
			};

			modifiers[0].getModifierFnFor = _ => d => () => ++called;

			routineFactory.modifiers = modifiers
				.Select(Reference<IModifierFactory>.Component)
				.Select(BaseFuncFactoryTests.ToModifierData(ModifierHook.OnBeginSubRoutine))
				.ToArray();
			routineFactory.subRoutines = _ => new SubRoutineFn[] { yieldsFn };

			var routineFn = routineFactory.GetRoutineFnFor(agent);
			var routine = routineFn()!.GetEnumerator();

			Assert.AreEqual(0, called);

			routine.MoveNext();

			Assert.AreEqual(1, called);

			while (routine.MoveNext()) ;

			Assert.AreEqual(1, called);
		}

		[Test]
		public void OnUpdate() {
			var called = 0;
			var agent = new GameObject();
			var routineFactory = new MockFactory();
			var yieldsFn = (SubRoutineFn)(_ => new YieldInstruction[] {
				new WaitForEndOfFrame(),
				new WaitForEndOfFrame(),
				new WaitForEndOfFrame(),
			});
			var modifiers = new MockModifierFactory[] {
				new GameObject().AddComponent<MockModifierFactory>(),
			};

			modifiers[0].getModifierFnFor = _ => d => () => ++called;

			routineFactory.modifiers = modifiers
				.Select(Reference<IModifierFactory>.Component)
				.Select(BaseFuncFactoryTests.ToModifierData(ModifierHook.OnUpdateSubRoutine))
				.ToArray();
			routineFactory.subRoutines = _ => new SubRoutineFn[] { yieldsFn };

			var routineFn = routineFactory.GetRoutineFnFor(agent)!;

			var i = 0;
			foreach (var _ in routineFn().OrEmpty()) {
				Assert.AreEqual(i, called);
				++i;
			};

			Assert.AreEqual(3, called);
		}

		[Test]
		public void OnEnd() {
			var called = 0;
			var agent = new GameObject();
			var routineFactory = new MockFactory();
			var yieldsFn = (SubRoutineFn)(_ => new YieldInstruction[] {
				new WaitForEndOfFrame(),
				new WaitForEndOfFrame(),
				new WaitForEndOfFrame(),
			});
			var modifiers = new MockModifierFactory[] {
				new GameObject().AddComponent<MockModifierFactory>(),
			};

			modifiers[0].getModifierFnFor = _ => d => () => ++called;

			routineFactory.modifiers = modifiers
				.Select(Reference<IModifierFactory>.Component)
				.Select(BaseFuncFactoryTests.ToModifierData(ModifierHook.OnEndSubroutine))
				.ToArray();
			routineFactory.subRoutines = _ => new SubRoutineFn[] { yieldsFn };

			var routineFn = routineFactory.GetRoutineFnFor(agent)!;

			foreach (var _ in routineFn().OrEmpty()) {
				Assert.AreEqual(0, called);
			};

			Assert.AreEqual(1, called);
		}

		[Test]
		public void StopWhenSwitchCalled() {
			var called = 0;
			var iterations = 0;
			var agent = new GameObject();
			var routineFactory = new MockFactory();
			var modifiers = new MockModifierFactory[] {
				new GameObject().AddComponent<MockModifierFactory>(),
			};

			IEnumerable<WaitForEndOfFrame> yieldsFn(Data _) {
				for (; iterations < 9; ++iterations) {
					yield return new WaitForEndOfFrame();
				}
			}

			modifiers[0].getModifierFnFor = _ => d => () => ++called;

			routineFactory.modifiers = modifiers
				.Select(Reference<IModifierFactory>.Component)
				.Select(BaseFuncFactoryTests.ToModifierData(ModifierHook.OnEndSubroutine))
				.ToArray();
			routineFactory.subRoutines = _ => new SubRoutineFn[] { yieldsFn };

			var routineFn = routineFactory.GetRoutineFnFor(agent)!;
			var routine = routineFn()!;

			foreach (var _ in routine) {
				if (iterations == 9) {
					routine.NextSubRoutine();
				};
			};

			Assert.AreEqual((9, 1), (iterations, called));
		}

		[Test]
		public void SwitchThroughroutines() {
			var iterations = 1;
			var modifyCounters = (begin: 0, update: 0, end: 0);
			var agent = new GameObject();
			var routineFactory = new MockFactory();
			var modifiers = new (Action, ModifierHook)[] {
				(() => ++modifyCounters.begin, ModifierHook.OnBeginSubRoutine),
				(() => ++modifyCounters.update, ModifierHook.OnUpdateSubRoutine),
				(() => ++modifyCounters.end, ModifierHook.OnEndSubroutine)
			};

			SubRoutineFn GetSubRoutineFn() {
				IEnumerable<WaitForEndOfFrame> run(Data _) {
					for (; iterations < 100; ++iterations) {
						yield return new WaitForEndOfFrame();
					}
				}
				return run;
			}

			ModifierData ToModifierData((Action, ModifierHook) pairs) {
				var (action, hook) = pairs;
				var modifier = new GameObject().AddComponent<MockModifierFactory>();
				modifier.getModifierFnFor = _ => _ => action;
				return new ModifierData {
					hook = hook,
					factory = Reference<IModifierFactory>.Component(modifier),
				};
			}

			routineFactory.modifiers = modifiers
				.Select(ToModifierData)
				.ToArray();
			routineFactory.subRoutines = _ => new SubRoutineFn[] {
				GetSubRoutineFn(),
				GetSubRoutineFn(),
				GetSubRoutineFn(),
			};

			var routineFn = routineFactory.GetRoutineFnFor(agent)!;
			var routine = routineFn()!;

			var frames = 0;
			bool PassedFourFrames() => ++frames % 4 == 0;
			foreach (var _ in routine) {
				if (PassedFourFrames()) {
					routine.NextSubRoutine();
				};
			};

			Assert.AreEqual((10, (3, 12, 3)), (iterations, modifyCounters));
		}

		[Test]
		public void ReturnNullWhenNoYieldFns() {
			var agent = new GameObject();
			var routineFactory = new MockFactory();

			routineFactory.subRoutines = _ => new SubRoutineFn[0];

			var routineFn = routineFactory.GetRoutineFnFor(agent);

			Assert.Null(routineFn());
		}

		[Test]
		public void ReturnNullWhenAllYieldFnsReturnNull() {
			var agent = new GameObject();
			var routineFactory = new MockFactory();

			routineFactory.subRoutines = _ => new SubRoutineFn[] { _ => null, _ => null };

			var routineFn = routineFactory.GetRoutineFnFor(agent);

			Assert.Null(routineFn());
		}

		class MockmodifierFactoryDataA : Data { }
		class MockModifierFactoryDataB : Data { }

		[Test]
		public void ExtendModifierFactoryData() {
			var data = null as Data;
			var agent = new GameObject();
			var routineFactory = new MockFactory();
			var modifiers = new MockModifierFactory[] {
				new GameObject().AddComponent<MockModifierFactory>(),
			};
			var onBeginSubroutine = ModifierHook.OnBeginSubRoutine;

			modifiers[0].getModifierFnFor = _ => d => () => data = d;

			routineFactory.modifiers = modifiers
				.Select(Reference<IModifierFactory>.Component)
				.Select(BaseFuncFactoryTests.ToModifierData(onBeginSubroutine))
				.ToArray();
			routineFactory.subRoutines = _ => new SubRoutineFn[] {
				_ => new YieldInstruction[0]
			};

			routineFactory.extendData = d => d.Extent<MockModifierFactoryDataB>();

			var routineFn = routineFactory.GetRoutineFnFor(agent)!;

			foreach (var _ in routineFn().OrEmpty()) ;

			Assert.NotNull(data!.As<MockModifierFactoryDataB>());
		}

		[Test]
		public void CallbackTimes() {
			var agent = new GameObject();
			var routineFactory = new MockFactory();
			var callsModifiers = (
				begin: 0,
				beginSR: 0,
				updateSR: 0,
				endSR: 0,
				end: 0
			);
			var modifiers = new (Action, ModifierHook)[] {
				(() => ++callsModifiers.begin, ModifierHook.OnBegin),
				(() => ++callsModifiers.beginSR, ModifierHook.OnBeginSubRoutine),
				(() => ++callsModifiers.updateSR, ModifierHook.OnUpdateSubRoutine),
				(() => ++callsModifiers.endSR, ModifierHook.OnEndSubroutine),
				(() => ++callsModifiers.end, ModifierHook.OnEnd),
			};
			var callsModifiersFn = 0;
			var callsSubRoutineFn = 0;

			SubRoutineFn subRoutine = _ => {
				++callsSubRoutineFn;
				return new[] {
					new WaitForEndOfFrame(),
					new WaitForEndOfFrame(),
				};
			};

			ModifierData ToUnityGameObject((Action, ModifierHook) pairs) {
				var (action, hook) = pairs;
				var modifier = new GameObject().AddComponent<MockModifierFactory>();
				modifier.getModifierFnFor = _ => {
					++callsModifiersFn;
					return _ => action;
				};
				return new ModifierData {
					hook = hook,
					factory = Reference<IModifierFactory>.Component(modifier),
				};
			}

			routineFactory.modifiers = modifiers
				.Select(ToUnityGameObject)
				.ToArray();
			routineFactory.subRoutines = _ => new[] { subRoutine, subRoutine };

			var routineFn = routineFactory.GetRoutineFnFor(agent)!;

			Assert.AreEqual(
				(0, 5, (0, 0, 0, 0, 0)),
				(callsSubRoutineFn, callsModifiersFn, callsModifiers)
			);

			var routine = routineFn()!.GetEnumerator();

			routine.MoveNext(); // 1st set 1st yield: ++begin, ++beginSR1

			Assert.AreEqual(
				(2, 5, (1, 1, 0, 0, 0)),
				(callsSubRoutineFn, callsModifiersFn, callsModifiers)
			);

			routine.MoveNext(); // 1st set 2nd yield: ++updateSR1

			Assert.AreEqual(
				(2, 5, (1, 1, 1, 0, 0)),
				(callsSubRoutineFn, callsModifiersFn, callsModifiers)
			);

			routine.MoveNext(); // 2nd set 1st yield: ++updateSR1 ++endSR1 ++beginSR2

			Assert.AreEqual(
				(2, 5, (1, 2, 2, 1, 0)),
				(callsSubRoutineFn, callsModifiersFn, callsModifiers)
			);

			routine.MoveNext(); // 2nd set 2nd yield: ++updateSR2

			Assert.AreEqual(
				(2, 5, (1, 2, 3, 1, 0)),
				(callsSubRoutineFn, callsModifiersFn, callsModifiers)
			);

			routine.MoveNext(); // done: ++updateSR2 ++endSR2, ++end

			Assert.AreEqual(
				(2, 5, (1, 2, 4, 2, 1)),
				(callsSubRoutineFn, callsModifiersFn, callsModifiers)
			);
		}

		[Test]
		public void NextSubRoutineFalse() {
			var called = null as Transform;
			var routineFactory = new MockFactory();

			IEnumerable<YieldInstruction> yields() {
				yield return new WaitForEndOfFrame();
				yield return new WaitForEndOfFrame();
				yield return new WaitForEndOfFrame();
			}

			routineFactory.subRoutines = a => new SubRoutineFn[] { _ => yields() };

			var routineFn = routineFactory.GetRoutineFnFor(new GameObject())!;
			var routine = routineFn()!;

			Assert.False(routine.NextSubRoutine());
		}

		[Test]
		public void NextSubRoutineTrue() {
			var called = null as Transform;
			var routineFactory = new MockFactory();

			IEnumerable<YieldInstruction> yields() {
				yield return new WaitForEndOfFrame();
				yield return new WaitForEndOfFrame();
				yield return new WaitForEndOfFrame();
			}

			routineFactory.subRoutines = a => new SubRoutineFn[] {
				_ => yields(),
				_ => yields(),
			};

			var routineFn = routineFactory.GetRoutineFnFor(new GameObject())!;
			var routine = routineFn()!;

			Assert.True(routine.NextSubRoutine());
		}
	}
}
