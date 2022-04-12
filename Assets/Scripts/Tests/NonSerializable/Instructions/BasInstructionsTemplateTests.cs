using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

public class BaseInstructionsTemplateTests : TestCollection
{
	class MockPluginSO : ScriptableObject, IPlugin
	{
		public Func<GameObject, PluginHooksFn> getCallbacks =
			_ => _ => new PluginHooks();

		public PluginHooksFn PluginHooksFor(
			GameObject agent
		) => this.getCallbacks(agent);
	}

	class MockInstructions : BaseInstructionsTemplate<Transform>
	{
		public Func<GameObject, Transform> getConcreteAgent =
			agent => agent.transform;
		public Func<Transform, InternalInstructionFn> partialInstructions =
			_ => _ => new YieldInstruction[0];
		public Action<PluginData> extendPluginData =
			_ => { };

		protected override Transform ConcreteAgent(GameObject agent) {
			return this.getConcreteAgent(agent);
		}

		protected override InternalInstructionFn InternalInstructionsFn(Transform agent) {
			return this.partialInstructions(agent);
		}

		protected override void ExtendPluginData(PluginData pluginData) {
			this.extendPluginData(pluginData);
		}
	}

	[Test]
	public void GetConcreteAgent() {
		var called = null as Transform;
		var instructions = new MockInstructions();
		var agent = new GameObject();

		instructions.partialInstructions = (agent) => {
			called = agent;
			return _ => new YieldInstruction[0];
		};

		_ = instructions.GetInstructionsFor(agent);

		Assert.AreSame(agent.transform, called);
	}

	[Test]
	public void RunInstructions() {
		var called = null as Transform;
		var instructions = new MockInstructions();
		var agent = new GameObject();

		IEnumerable<YieldInstruction> moveUp(Transform transform) {
			yield return new WaitForEndOfFrame();
			transform.position += Vector3.up;
			yield return new WaitForEndOfFrame();
			transform.position += Vector3.up;
			yield return new WaitForEndOfFrame();
			transform.position += Vector3.up;
		}

		instructions.partialInstructions = agent => _ => moveUp(agent);

		var (run, _) = instructions.GetInstructionsFor(agent)()!.Value;

		foreach (var _ in run) { }

		Assert.AreEqual(Vector3.up * 3, agent.transform.position);
	}

	public void GetCallbacks() {
		var called = new List<GameObject>();
		var agent = new GameObject();
		var instructions = new MockInstructions();

		PluginHooksFn getCallbacks(GameObject agent) {
			called!.Add(agent);
			return _ => new PluginHooks();
		}

		var plugins = new MockPluginSO[] {
			ScriptableObject.CreateInstance<MockPluginSO>(),
			ScriptableObject.CreateInstance<MockPluginSO>(),
		};
		plugins.ForEach(plugin => plugin.getCallbacks = getCallbacks);
		instructions.plugins = plugins
			.Select(Reference<IPlugin>.ScriptableObject)
			.ToArray();

		_ = instructions.GetInstructionsFor(agent);

		Assert.AreEqual((agent, agent), (called[0], called[1]));
	}

	[Test]
	public void OnBeginBeforeFirstYield() {
		var called = null as PluginData;
		var agent = new GameObject();
		var instructions = new MockInstructions();

		PluginHooksFn getCallbacks(GameObject agent) {
			return d => new PluginHooks { onBegin = () => called = d };
		}

		var plugins = new MockPluginSO[] {
			ScriptableObject.CreateInstance<MockPluginSO>(),
			ScriptableObject.CreateInstance<MockPluginSO>(),
		};
		plugins.ForEach(pl => pl.getCallbacks = getCallbacks);
		instructions.plugins = plugins
			.Select(Reference<IPlugin>.ScriptableObject)
			.ToArray(); ;
		instructions.partialInstructions = _ => _ => new YieldInstruction[] {
			new WaitForEndOfFrame(),
			new WaitForEndOfFrame(),
			new WaitForEndOfFrame(),
		};

		var (run, _) = instructions.GetInstructionsFor(agent)()!.Value;

		Assert.IsNull(called);

		foreach (var hold in run) break;

		Assert.NotNull(called);
	}

	[Test]
	public void PluginDataAllSame() {
		var data = new List<PluginData>();
		var agent = new GameObject();
		var instructions = new MockInstructions();
		var plugins = new MockPluginSO[] {
			ScriptableObject.CreateInstance<MockPluginSO>(),
			ScriptableObject.CreateInstance<MockPluginSO>(),
			ScriptableObject.CreateInstance<MockPluginSO>(),
		};
		plugins.ForEach(pl => pl.getCallbacks = _ => d => new PluginHooks {
			onBegin = () => data.Add(d),
			onBeforeYield = () => data.Add(d),
			onAfterYield = () => data.Add(d),
			onEnd = () => data.Add(d),
		});

		instructions.partialInstructions = _ => d => {
			data.Add(d);
			return new YieldInstruction[0];
		};
		instructions.plugins = plugins
			.Select(Reference<IPlugin>.ScriptableObject)
			.ToArray(); ;
		instructions.partialInstructions = _ => _ => new YieldInstruction[] {
			new WaitForEndOfFrame(),
			new WaitForEndOfFrame(),
			new WaitForEndOfFrame(),
		};

		var (run, _) = instructions.GetInstructionsFor(agent)()!.Value;

		foreach (var _ in run) ;

		Assert.AreEqual(1, data.Distinct().Count());
	}

	[Test]
	public void OnAfterYield() {
		var called = 0;
		var agent = new GameObject();
		var instructions = new MockInstructions();

		PluginHooksFn getCallbacks(GameObject agent) {
			return _ => new PluginHooks { onAfterYield = () => ++called };
		};

		var plugins = new MockPluginSO[] {
			ScriptableObject.CreateInstance<MockPluginSO>(),
			ScriptableObject.CreateInstance<MockPluginSO>(),
		};
		plugins.ForEach(pl => pl.getCallbacks = getCallbacks);
		instructions.plugins = plugins
			.Select(Reference<IPlugin>.ScriptableObject)
			.ToArray(); ;
		instructions.partialInstructions = _ => _ => new YieldInstruction[] {
			new WaitForEndOfFrame(),
			new WaitForEndOfFrame(),
			new WaitForEndOfFrame(),
		};

		var (run, _) = instructions.GetInstructionsFor(agent)()!.Value;

		var i = 0;
		foreach (var _ in run) {
			Assert.AreEqual(i, called);
			i += 2;
		};

		Assert.AreEqual(6, called);
	}

	[Test]
	public void OnBeforeYield() {
		var called = 0;
		var agent = new GameObject();
		var instructions = new MockInstructions();

		PluginHooksFn getCallbacks(GameObject agent) {
			return _ => new PluginHooks { onBeforeYield = () => ++called };
		};

		var plugins = new MockPluginSO[] {
			ScriptableObject.CreateInstance<MockPluginSO>(),
			ScriptableObject.CreateInstance<MockPluginSO>(),
		};
		plugins.ForEach(pl => pl.getCallbacks = getCallbacks);
		instructions.plugins = plugins
			.Select(Reference<IPlugin>.ScriptableObject)
			.ToArray(); ;
		instructions.partialInstructions = _ => _ => new YieldInstruction[] {
			new WaitForEndOfFrame(),
			new WaitForEndOfFrame(),
			new WaitForEndOfFrame(),
		};

		var (run, _) = instructions.GetInstructionsFor(agent)()!.Value;

		var i = 0;
		foreach (var _ in run) {
			i += 2;
			Assert.AreEqual(i, called);
		};

		Assert.AreEqual(6, called);
	}

	[Test]
	public void OnEndBeforeLastYield() {
		var called = 0;
		var agent = new GameObject();
		var instructions = new MockInstructions();

		PluginHooksFn getCallbacks(GameObject agent) {
			return _ => new PluginHooks { onEnd = () => ++called };
		};

		var plugins = new MockPluginSO[] {
			ScriptableObject.CreateInstance<MockPluginSO>(),
			ScriptableObject.CreateInstance<MockPluginSO>(),
		};
		plugins.ForEach(pl => pl.getCallbacks = getCallbacks);
		instructions.plugins = plugins
			.Select(Reference<IPlugin>.ScriptableObject)
			.ToArray(); ;
		instructions.partialInstructions = _ => _ => new YieldInstruction[] {
			new WaitForEndOfFrame(),
			new WaitForEndOfFrame(),
			new WaitForEndOfFrame(),
		};

		var (run, _) = instructions.GetInstructionsFor(agent)()!.Value;

		foreach (var _ in run) {
			Assert.AreEqual(0, called);
		};

		Assert.AreEqual(2, called);
	}

	[Test]
	public void StopWhenReleaseCalled() {
		var called = 0;
		var iterations = 0;
		var agent = new GameObject();
		var instructions = new MockInstructions();
		var plugin = ScriptableObject.CreateInstance<MockPluginSO>();

		IEnumerable<YieldInstruction> instructionFunc(PluginData _) {
			for (iterations = 0; iterations < 100; ++iterations) {
				yield return new WaitForEndOfFrame();
			}
		}

		plugin.getCallbacks = _ => _ => new PluginHooks {
			onEnd = () => ++called
		};
		instructions.plugins = new Reference<IPlugin>[] {
			Reference<IPlugin>.ScriptableObject(plugin),
		};
		instructions.partialInstructions = _ => instructionFunc;

		var (run, release) = instructions.GetInstructionsFor(agent)()!.Value;

		foreach (var _ in run) {
			if (iterations == 9) {
				release();
			};
		};

		Assert.AreEqual((9, 1), (iterations, called));
	}

	[Test]
	public void StopWhenPluginDataRunFalse() {
		var called = 0;
		var iterations = 0;
		var agent = new GameObject();
		var instructions = new MockInstructions();
		var plugin = ScriptableObject.CreateInstance<MockPluginSO>();

		IEnumerable<UnityEngine.YieldInstruction> instructionFunc(PluginData _) {
			for (iterations = 0; iterations < 100; ++iterations) {
				yield return new WaitForEndOfFrame();
			}
		}
		var pluginData = null as CorePluginData;
		plugin.getCallbacks = _ => d => new PluginHooks {
			onBegin = () => pluginData = d.As<CorePluginData>(),
			onEnd = () => ++called
		};
		instructions.plugins = new Reference<IPlugin>[] {
			Reference<IPlugin>.ScriptableObject(plugin),
		};
		instructions.partialInstructions = _ => instructionFunc;

		var (run, _) = instructions.GetInstructionsFor(agent)()!.Value;

		foreach (var _ in run) {
			if (iterations == 9) {
				pluginData!.run = false;
			};
		};

		Assert.AreEqual((9, 1), (iterations, called));
	}

	[Test]
	public void ReturnNullWhenInstructionsNull() {
		var agent = new GameObject();
		var instructions = new MockInstructions();

		instructions.partialInstructions = _ => _ => null;

		var insructions = instructions.GetInstructionsFor(agent);

		Assert.Null(insructions());
	}

	class MockPluginDataA : PluginData { }
	class MockPluginDataB : PluginData { }

	[Test]
	public void ExtendPluginData() {
		var data = null as PluginData;
		var agent = new GameObject();
		var instructions = new MockInstructions();
		var plugin = ScriptableObject.CreateInstance<MockPluginSO>();

		plugin.getCallbacks = _ => d => new PluginHooks {
			onBegin = () => data = d
		};
		instructions.plugins = new Reference<IPlugin>[] {
			Reference<IPlugin>.ScriptableObject(plugin),
		};
		instructions.extendPluginData = d => d.Extent<MockPluginDataB>();

		var (run, _) = instructions.GetInstructionsFor(agent)()!.Value;

		foreach (var _ in run) ;

		Assert.NotNull(data!.As<MockPluginDataB>());
	}
}
