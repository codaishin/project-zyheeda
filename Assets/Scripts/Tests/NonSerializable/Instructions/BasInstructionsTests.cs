using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

public class BaseInstructionsTests : TestCollection
{
	class MockPluginSO : ScriptableObject, IPlugin
	{
		public Func<GameObject, PartialPluginCallbacks> getCallbacks =
			_ => _ => new PluginCallbacks();

		public PartialPluginCallbacks GetCallbacks(
			GameObject agent
		) => this.getCallbacks(agent);
	}

	class MockInstructions : BaseInstructions<Transform>
	{
		public Func<GameObject, Transform> getConcreteAgent =
			agent => agent.transform;
		public Func<Transform, PartialInstructionFunc> partialInstructions =
			_ => _ => new YieldInstruction[0];
		public Action<PluginData> extendPluginData =
			_ => { };

		protected override Transform GetConcreteAgent(GameObject agent) {
			return this.getConcreteAgent(agent);
		}

		protected override PartialInstructionFunc PartialInstructions(Transform agent) {
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

		var _ = instructions.GetInstructionsFor(agent);

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

		var insructions = instructions.GetInstructionsFor(agent);

		foreach (var _ in insructions()!) { }

		Assert.AreEqual(Vector3.up * 3, agent.transform.position);
	}

	public void GetCallbacks() {
		var called = new List<GameObject>();
		var agent = new GameObject();
		var instructions = new MockInstructions();

		PartialPluginCallbacks getCallbacks(GameObject agent) {
			called!.Add(agent);
			return _ => new PluginCallbacks();
		}

		var plugins = new MockPluginSO[] {
			ScriptableObject.CreateInstance<MockPluginSO>(),
			ScriptableObject.CreateInstance<MockPluginSO>(),
		};
		plugins.ForEach(plugin => plugin.getCallbacks = getCallbacks);
		instructions.plugins = plugins
			.Select(Reference<IPlugin>.ScriptableObject)
			.ToArray();

		var insructions = instructions.GetInstructionsFor(agent)!;

		Assert.AreEqual((agent, agent), (called[0], called[1]));
	}

	[Test]
	public void OnBeginBeforeFirstYield() {
		var called = null as PluginData;
		var agent = new GameObject();
		var instructions = new MockInstructions();

		PartialPluginCallbacks getCallbacks(GameObject agent) {
			return d => new PluginCallbacks { onBegin = () => called = d };
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

		var insructions = instructions.GetInstructionsFor(agent);

		Assert.IsNull(called);

		foreach (var hold in insructions()!) break;

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
		plugins.ForEach(pl => pl.getCallbacks = _ => d => new PluginCallbacks {
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

		var insructions = instructions.GetInstructionsFor(agent);

		foreach (var _ in insructions()!) ;

		Assert.AreEqual(1, data.Distinct().Count());
	}

	[Test]
	public void OnAfterYield() {
		var called = 0;
		var agent = new GameObject();
		var instructions = new MockInstructions();

		PartialPluginCallbacks getCallbacks(GameObject agent) {
			return _ => new PluginCallbacks { onAfterYield = () => ++called };
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

		var insructions = instructions.GetInstructionsFor(agent);

		var i = 0;
		foreach (var _ in insructions()!) {
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

		PartialPluginCallbacks getCallbacks(GameObject agent) {
			return _ => new PluginCallbacks { onBeforeYield = () => ++called };
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

		var insructions = instructions.GetInstructionsFor(agent);

		var i = 0;
		foreach (var _ in insructions()!) {
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

		PartialPluginCallbacks getCallbacks(GameObject agent) {
			return _ => new PluginCallbacks { onEnd = () => ++called };
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

		var insructions = instructions.GetInstructionsFor(agent);

		foreach (var _ in insructions()!) {
			Assert.AreEqual(0, called);
		};

		Assert.AreEqual(2, called);
	}

	[Test]
	public void StopWhenProvidedPredicateFalse() {
		var called = 0;
		var run = true;
		var iterations = 0;
		var agent = new GameObject();
		var instructions = new MockInstructions();
		var plugin = ScriptableObject.CreateInstance<MockPluginSO>();

		IEnumerable<YieldInstruction> instructionFunc(PluginData _) {
			for (iterations = 0; iterations < 100; ++iterations) {
				yield return new WaitForEndOfFrame();
			}
		}

		plugin.getCallbacks = _ => _ => new PluginCallbacks {
			onEnd = () => ++called
		};
		instructions.plugins = new Reference<IPlugin>[] {
			Reference<IPlugin>.ScriptableObject(plugin),
		};
		instructions.partialInstructions = _ => instructionFunc;

		var insructions = instructions.GetInstructionsFor(agent);

		foreach (var _ in insructions(() => run)!) {
			if (iterations == 9) {
				run = false;
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
		plugin.getCallbacks = _ => d => new PluginCallbacks {
			onBegin = () => pluginData = d.As<CorePluginData>(),
			onEnd = () => ++called
		};
		instructions.plugins = new Reference<IPlugin>[] {
			Reference<IPlugin>.ScriptableObject(plugin),
		};
		instructions.partialInstructions = _ => instructionFunc;

		var insructions = instructions.GetInstructionsFor(agent);

		foreach (var _ in insructions()!) {
			if (iterations == 9) {
				pluginData!.run = false;
			};
		};

		Assert.AreEqual((9, 1), (iterations, called));
	}

	[Test]
	public void StopWhenPluginDataRunFalseWhenPredicateProvided() {
		var called = 0;
		var iterations = 0;
		var agent = new GameObject();
		var instructions = new MockInstructions();
		var run = true;
		var plugin = ScriptableObject.CreateInstance<MockPluginSO>();

		IEnumerable<UnityEngine.YieldInstruction> instructionFunc(PluginData _) {
			for (iterations = 0; iterations < 100; ++iterations) {
				yield return new WaitForEndOfFrame();
			}
		}
		var pluginData = null as CorePluginData;
		plugin.getCallbacks = _ => d => new PluginCallbacks {
			onBegin = () => pluginData = d.As<CorePluginData>(),
			onEnd = () => ++called
		};
		instructions.plugins = new Reference<IPlugin>[] {
			Reference<IPlugin>.ScriptableObject(plugin),
		};
		instructions.partialInstructions = _ => instructionFunc;

		var insructions = instructions.GetInstructionsFor(agent);

		foreach (var _ in insructions(() => run)!) {
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

		plugin.getCallbacks = _ => d => new PluginCallbacks {
			onBegin = () => data = d
		};
		instructions.plugins = new Reference<IPlugin>[] {
			Reference<IPlugin>.ScriptableObject(plugin),
		};
		instructions.extendPluginData = d => d.Extent<MockPluginDataB>();

		var insructions = instructions.GetInstructionsFor(agent);

		foreach (var _ in insructions()!) ;

		Assert.NotNull(data!.As<MockPluginDataB>());
	}
}
