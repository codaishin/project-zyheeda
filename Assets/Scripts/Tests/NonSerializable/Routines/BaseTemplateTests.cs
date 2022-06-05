using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace Routines
{
	public class BaseTemplateTests : TestCollection
	{
		private
		static
		Func<Reference<IPlugin>, PluginData> ToModifierData(
			PluginFlags hook
		) {
			return factory => new PluginData { flag = hook, plugin = factory, };
		}

		class MockTemplate : BaseTemplate<Transform>
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

		class MockPluginMB : MonoBehaviour, IPlugin
		{
			public Func<GameObject, PluginFn> getModifierFnFor = _ => _ => null;

			public PluginFn GetPluginFnFor(GameObject agent) {
				return this.getModifierFnFor(agent);
			}
		}

		[Test]
		public void GetConcreteAgent() {
			var called = null as Transform;
			var template = new MockTemplate();
			var agent = new GameObject();

			template.subRoutines = a => {
				called = a;
				return new SubRoutineFn[0];
			};

			_ = template.GetRoutineFnFor(agent);

			Assert.AreEqual(agent.transform, called);
		}

		[Test]
		public void RunRoutine() {
			var called = null as Transform;
			var template = new MockTemplate();
			var agent = new GameObject();

			IEnumerable<YieldInstruction> moveUp(Transform transform) {
				yield return new WaitForEndOfFrame();
				transform.position += Vector3.up;
				yield return new WaitForEndOfFrame();
				transform.position += Vector3.up;
				yield return new WaitForEndOfFrame();
				transform.position += Vector3.up;
			}

			template.subRoutines = a => new SubRoutineFn[] { _ => moveUp(a) };

			var routineFn = template.GetRoutineFnFor(agent)!;

			foreach (var _ in routineFn().OrEmpty()) ;

			Assert.AreEqual(Vector3.up * 3, agent.transform.position);
		}

		[Test]
		public void DataAndAgentToModifierFactory() {
			var data = new List<Data>();
			var agents = new List<GameObject>();
			var agent = new GameObject();
			var template = new MockTemplate();
			var yieldsFn = (SubRoutineFn)(_ => new YieldInstruction[] {
				new WaitForEndOfFrame(),
				new WaitForEndOfFrame(),
				new WaitForEndOfFrame(),
			});
			var allHooks =
				PluginFlags.OnBeginSubRoutine |
				PluginFlags.OnUpdateSubRoutine |
				PluginFlags.OnEndSubroutine;
			var plugins = new[] {
				new GameObject().AddComponent<MockPluginMB>(),
				new GameObject().AddComponent<MockPluginMB>(),
			};

			foreach (var modifier in plugins) {
				modifier.getModifierFnFor = a => {
					agents.Add(a);
					return d => () => data.Add(d);
				};
			}

			template.plugins = plugins
				.Select(Reference<IPlugin>.Component)
				.Select(BaseTemplateTests.ToModifierData(allHooks))
				.ToArray();
			template.subRoutines = _ => new SubRoutineFn[] { yieldsFn };

			var routineFn = template.GetRoutineFnFor(agent);

			foreach (var _ in routineFn().OrEmpty()) ;

			Assert.AreEqual(1, data.Distinct().Count());
			CollectionAssert.AreEqual(new[] { agent, agent }, agents);
		}

		[Test]
		public void OnBegin() {
			var called = 0;
			var agent = new GameObject();
			var template = new MockTemplate();
			var yieldsFn = (SubRoutineFn)(_ => new YieldInstruction[] {
				new WaitForEndOfFrame(),
				new WaitForEndOfFrame(),
				new WaitForEndOfFrame(),
			});
			var plugins = new MockPluginMB[] {
				new GameObject().AddComponent<MockPluginMB>(),
			};

			plugins[0].getModifierFnFor = _ => d => () => ++called;

			template.plugins = plugins
				.Select(Reference<IPlugin>.Component)
				.Select(BaseTemplateTests.ToModifierData(PluginFlags.OnBeginSubRoutine))
				.ToArray();
			template.subRoutines = _ => new SubRoutineFn[] { yieldsFn };

			var routineFn = template.GetRoutineFnFor(agent);
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
			var template = new MockTemplate();
			var yieldsFn = (SubRoutineFn)(_ => new YieldInstruction[] {
				new WaitForEndOfFrame(),
				new WaitForEndOfFrame(),
				new WaitForEndOfFrame(),
			});
			var plugins = new MockPluginMB[] {
				new GameObject().AddComponent<MockPluginMB>(),
			};

			plugins[0].getModifierFnFor = _ => d => () => ++called;

			template.plugins = plugins
				.Select(Reference<IPlugin>.Component)
				.Select(BaseTemplateTests.ToModifierData(PluginFlags.OnUpdateSubRoutine))
				.ToArray();
			template.subRoutines = _ => new SubRoutineFn[] { yieldsFn };

			var routineFn = template.GetRoutineFnFor(agent)!;

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
			var template = new MockTemplate();
			var yieldsFn = (SubRoutineFn)(_ => new YieldInstruction[] {
				new WaitForEndOfFrame(),
				new WaitForEndOfFrame(),
				new WaitForEndOfFrame(),
			});
			var plugins = new MockPluginMB[] {
				new GameObject().AddComponent<MockPluginMB>(),
			};

			plugins[0].getModifierFnFor = _ => d => () => ++called;

			template.plugins = plugins
				.Select(Reference<IPlugin>.Component)
				.Select(BaseTemplateTests.ToModifierData(PluginFlags.OnEndSubroutine))
				.ToArray();
			template.subRoutines = _ => new SubRoutineFn[] { yieldsFn };

			var routineFn = template.GetRoutineFnFor(agent)!;

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
			var template = new MockTemplate();
			var plugins = new MockPluginMB[] {
				new GameObject().AddComponent<MockPluginMB>(),
			};

			IEnumerable<WaitForEndOfFrame> yieldsFn(Data _) {
				for (; iterations < 9; ++iterations) {
					yield return new WaitForEndOfFrame();
				}
			}

			plugins[0].getModifierFnFor = _ => d => () => ++called;

			template.plugins = plugins
				.Select(Reference<IPlugin>.Component)
				.Select(BaseTemplateTests.ToModifierData(PluginFlags.OnEndSubroutine))
				.ToArray();
			template.subRoutines = _ => new SubRoutineFn[] { yieldsFn };

			var routineFn = template.GetRoutineFnFor(agent)!;
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
			var template = new MockTemplate();
			var plugins = new (Action, PluginFlags)[] {
				(() => ++modifyCounters.begin, PluginFlags.OnBeginSubRoutine),
				(() => ++modifyCounters.update, PluginFlags.OnUpdateSubRoutine),
				(() => ++modifyCounters.end, PluginFlags.OnEndSubroutine)
			};

			SubRoutineFn GetSubRoutineFn() {
				IEnumerable<WaitForEndOfFrame> run(Data _) {
					for (; iterations < 100; ++iterations) {
						yield return new WaitForEndOfFrame();
					}
				}
				return run;
			}

			PluginData ToModifierData((Action, PluginFlags) pairs) {
				var (action, hook) = pairs;
				var modifier = new GameObject().AddComponent<MockPluginMB>();
				modifier.getModifierFnFor = _ => _ => action;
				return new PluginData {
					flag = hook,
					plugin = Reference<IPlugin>.Component(modifier),
				};
			}

			template.plugins = plugins
				.Select(ToModifierData)
				.ToArray();
			template.subRoutines = _ => new SubRoutineFn[] {
				GetSubRoutineFn(),
				GetSubRoutineFn(),
				GetSubRoutineFn(),
			};

			var routineFn = template.GetRoutineFnFor(agent)!;
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
			var template = new MockTemplate();

			template.subRoutines = _ => new SubRoutineFn[0];

			var routineFn = template.GetRoutineFnFor(agent);

			Assert.Null(routineFn());
		}

		[Test]
		public void ReturnNullWhenAllYieldFnsReturnNull() {
			var agent = new GameObject();
			var template = new MockTemplate();

			template.subRoutines = _ => new SubRoutineFn[] { _ => null, _ => null };

			var routineFn = template.GetRoutineFnFor(agent);

			Assert.Null(routineFn());
		}

		class MockmodifierFactoryDataA : Data { }
		class MockModifierFactoryDataB : Data { }

		[Test]
		public void ExtendModifierFactoryData() {
			var data = null as Data;
			var agent = new GameObject();
			var template = new MockTemplate();
			var plugins = new MockPluginMB[] {
				new GameObject().AddComponent<MockPluginMB>(),
			};
			var onBeginSubroutine = PluginFlags.OnBeginSubRoutine;

			plugins[0].getModifierFnFor = _ => d => () => data = d;

			template.plugins = plugins
				.Select(Reference<IPlugin>.Component)
				.Select(BaseTemplateTests.ToModifierData(onBeginSubroutine))
				.ToArray();
			template.subRoutines = _ => new SubRoutineFn[] {
				_ => new YieldInstruction[0]
			};

			template.extendData = d => d.Extent<MockModifierFactoryDataB>();

			var routineFn = template.GetRoutineFnFor(agent)!;

			foreach (var _ in routineFn().OrEmpty()) ;

			Assert.NotNull(data!.As<MockModifierFactoryDataB>());
		}

		[Test]
		public void CallbackTimes() {
			var agent = new GameObject();
			var template = new MockTemplate();
			var callsModifiers = (
				begin: 0,
				beginSR: 0,
				updateSR: 0,
				endSR: 0,
				end: 0
			);
			var plugins = new (Action, PluginFlags)[] {
				(() => ++callsModifiers.begin, PluginFlags.OnBegin),
				(() => ++callsModifiers.beginSR, PluginFlags.OnBeginSubRoutine),
				(() => ++callsModifiers.updateSR, PluginFlags.OnUpdateSubRoutine),
				(() => ++callsModifiers.endSR, PluginFlags.OnEndSubroutine),
				(() => ++callsModifiers.end, PluginFlags.OnEnd),
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

			PluginData ToUnityGameObject((Action, PluginFlags) pairs) {
				var (action, hook) = pairs;
				var modifier = new GameObject().AddComponent<MockPluginMB>();
				modifier.getModifierFnFor = _ => {
					++callsModifiersFn;
					return _ => action;
				};
				return new PluginData {
					flag = hook,
					plugin = Reference<IPlugin>.Component(modifier),
				};
			}

			template.plugins = plugins
				.Select(ToUnityGameObject)
				.ToArray();
			template.subRoutines = _ => new[] { subRoutine, subRoutine };

			var routineFn = template.GetRoutineFnFor(agent)!;

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
			var template = new MockTemplate();

			IEnumerable<YieldInstruction> yields() {
				yield return new WaitForEndOfFrame();
				yield return new WaitForEndOfFrame();
				yield return new WaitForEndOfFrame();
			}

			template.subRoutines = a => new SubRoutineFn[] { _ => yields() };

			var routineFn = template.GetRoutineFnFor(new GameObject())!;
			var routine = routineFn()!;

			Assert.False(routine.NextSubRoutine());
		}

		[Test]
		public void NextSubRoutineTrue() {
			var called = null as Transform;
			var template = new MockTemplate();

			IEnumerable<YieldInstruction> yields() {
				yield return new WaitForEndOfFrame();
				yield return new WaitForEndOfFrame();
				yield return new WaitForEndOfFrame();
			}

			template.subRoutines = a => new SubRoutineFn[] {
				_ => yields(),
				_ => yields(),
			};

			var routineFn = template.GetRoutineFnFor(new GameObject())!;
			var routine = routineFn()!;

			Assert.True(routine.NextSubRoutine());
		}
	}
}
