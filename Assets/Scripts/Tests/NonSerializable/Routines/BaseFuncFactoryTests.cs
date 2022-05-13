using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace Routines
{
	public class BaseFuncFactoryTests : TestCollection
	{
		private static Func<Reference<IModifierFactory>, ModifierData> ToModifierData(
			ModifierHook hook
		) {
			return factory => new ModifierData { hook = hook, factory = factory, };
		}

		class MockFactory : BaseFuncFactory<Transform>
		{
			public Func<GameObject, Transform> concreteAgent = a => a.transform;
			public Func<Transform, SubRoutineFn[]> subRoutines = _ => new SubRoutineFn[0];
			public Action<RoutineData> extendData = _ => { };

			protected override SubRoutineFn[] SubRoutines(Transform agent) {
				return this.subRoutines(agent);
			}

			protected override Transform ConcreteAgent(GameObject agent) {
				return this.concreteAgent(agent);
			}

			protected
			override
			void ExtendData(RoutineData data) {
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
			var data = new List<RoutineData>();
			var agents = new List<GameObject>();
			var agent = new GameObject();
			var routineFactory = new MockFactory();
			var yieldsFn = (SubRoutineFn)(_ => new YieldInstruction[] {
				new WaitForEndOfFrame(),
				new WaitForEndOfFrame(),
				new WaitForEndOfFrame(),
			});
			var allHooks =
				ModifierHook.OnBegin |
				ModifierHook.OnUpdate |
				ModifierHook.OnEnd;
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
				.Select(BaseFuncFactoryTests.ToModifierData(ModifierHook.OnBegin))
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
				.Select(BaseFuncFactoryTests.ToModifierData(ModifierHook.OnUpdate))
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
				.Select(BaseFuncFactoryTests.ToModifierData(ModifierHook.OnEnd))
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

			IEnumerable<WaitForEndOfFrame> yieldsFn(RoutineData _) {
				for (; iterations < 9; ++iterations) {
					yield return new WaitForEndOfFrame();
				}
			}

			modifiers[0].getModifierFnFor = _ => d => () => ++called;

			routineFactory.modifiers = modifiers
				.Select(Reference<IModifierFactory>.Component)
				.Select(BaseFuncFactoryTests.ToModifierData(ModifierHook.OnEnd))
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
				(() => ++modifyCounters.begin, ModifierHook.OnBegin),
				(() => ++modifyCounters.update, ModifierHook.OnUpdate),
				(() => ++modifyCounters.end, ModifierHook.OnEnd)
			};

			SubRoutineFn GetSubRoutineFn() {
				IEnumerable<WaitForEndOfFrame> run(RoutineData _) {
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

		class MockmodifierFactoryDataA : RoutineData { }
		class MockmodifierFactoryDataB : RoutineData { }

		[Test]
		public void ExtendModifierFactoryData() {
			var data = null as RoutineData;
			var agent = new GameObject();
			var routineFactory = new MockFactory();
			var modifiers = new MockModifierFactory[] {
				new GameObject().AddComponent<MockModifierFactory>(),
			};

			modifiers[0].getModifierFnFor = _ => d => () => data = d;

			routineFactory.modifiers = modifiers
				.Select(Reference<IModifierFactory>.Component)
				.Select(BaseFuncFactoryTests.ToModifierData(ModifierHook.OnBegin))
				.ToArray();
			routineFactory.subRoutines = _ => new SubRoutineFn[] {
				_ => new YieldInstruction[0]
			};

			routineFactory.extendData = d => d.Extent<MockmodifierFactoryDataB>();

			var routineFn = routineFactory.GetRoutineFnFor(agent)!;

			foreach (var _ in routineFn().OrEmpty()) ;

			Assert.NotNull(data!.As<MockmodifierFactoryDataB>());
		}

		[Test]
		public void CallbackTimes() {
			var agent = new GameObject();
			var routineFactory = new MockFactory();
			var calledModifiers = (begin: 0, update: 0, end: 0);
			var modifiers = new (Action, ModifierHook)[] {
				(() => ++calledModifiers.begin, ModifierHook.OnBegin),
				(() => ++calledModifiers.update, ModifierHook.OnUpdate),
				(() => ++calledModifiers.end, ModifierHook.OnEnd),
			};
			var calledModifiersFn = 0;
			var calledYieldsFn = 0;

			SubRoutineFn yields = _ => {
				++calledYieldsFn;
				return new YieldInstruction[] {
					new WaitForEndOfFrame(),
					new WaitForEndOfFrame()
				};
			};

			ModifierData ToModifierData((Action, ModifierHook) pairs) {
				var (action, hook) = pairs;
				var modifier = new GameObject().AddComponent<MockModifierFactory>();
				modifier.getModifierFnFor = _ => {
					++calledModifiersFn;
					return _ => action;
				};
				return new ModifierData {
					hook = hook,
					factory = Reference<IModifierFactory>.Component(modifier),
				};
			}

			routineFactory.modifiers = modifiers
				.Select(ToModifierData)
				.ToArray();
			routineFactory.subRoutines = _ => new[] { yields, yields };

			var routineFn = routineFactory.GetRoutineFnFor(agent)!;

			Assert.AreEqual(
				(0, 3, (0, 0, 0)),
				(calledYieldsFn, calledModifiersFn, calledModifiers)
			);

			var routine = routineFn()!.GetEnumerator();

			routine.MoveNext(); // 1st set 1st yield: ++begin1

			Assert.AreEqual(
				(2, 3, (1, 0, 0)),
				(calledYieldsFn, calledModifiersFn, calledModifiers)
			);

			routine.MoveNext(); // 1st set 2nd yield: ++update1

			Assert.AreEqual(
				(2, 3, (1, 1, 0)),
				(calledYieldsFn, calledModifiersFn, calledModifiers)
			);

			routine.MoveNext(); // 2nd set 1st yield: ++update1 ++end1 ++begin2

			Assert.AreEqual(
				(2, 3, (2, 2, 1)),
				(calledYieldsFn, calledModifiersFn, calledModifiers)
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
