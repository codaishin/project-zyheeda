using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

public class BaseInstructionsSOTests : TestCollection
{
	class MockPluginSO : BaseInstructionsPluginSO
	{
		public Func<GameObject, PluginCallbacks> getCallbacks =
			_ => new PluginCallbacks();

		public Action<PluginData> extendPlugin = _ => { };

		public override PluginCallbacks GetCallbacks(GameObject agent) {
			return this.getCallbacks(agent);
		}

		public override void ExtendPluginData(PluginData data) {
			this.extendPlugin(data);
		}
	}

	class MockInstructionSO : BaseInstructionsSO<Transform>
	{
		public Func<GameObject, Transform> getConcreteAgent =
			agent => agent.transform;
		public Func<Transform, InstructionsPluginFunc> insructions =
			_ => _ => new YieldInstruction[0];

		protected override Transform GetConcreteAgent(GameObject agent) {
			return this.getConcreteAgent(agent);
		}

		protected override InstructionsPluginFunc Instructions(Transform agent) {
			return this.insructions(agent);
		}
	}

	[Test]
	public void GetConcreteAgent() {
		var called = null as Transform;
		var instructionsSO = ScriptableObject.CreateInstance<MockInstructionSO>();
		var agent = new GameObject();

		instructionsSO.insructions = (agent) => {
			called = agent;
			return _ => new YieldInstruction[0];
		};

		var _ = instructionsSO.GetInstructionsFor(agent);

		Assert.AreSame(agent.transform, called);
	}

	[Test]
	public void RunInstructions() {
		var called = null as Transform;
		var instructionsSO = ScriptableObject.CreateInstance<MockInstructionSO>();
		var agent = new GameObject();

		IEnumerable<YieldInstruction> moveUp(Transform transform) {
			yield return new WaitForEndOfFrame();
			transform.position += Vector3.up;
			yield return new WaitForEndOfFrame();
			transform.position += Vector3.up;
			yield return new WaitForEndOfFrame();
			transform.position += Vector3.up;
		}

		instructionsSO.insructions = agent => _ => moveUp(agent);

		var insructions = instructionsSO.GetInstructionsFor(agent);

		foreach (var _ in insructions()!) { }

		Assert.AreEqual(Vector3.up * 3, agent.transform.position);
	}

	public void GetCallbacks() {
		var called = new List<GameObject>();
		var agent = new GameObject();
		var instructionsSO = ScriptableObject.CreateInstance<MockInstructionSO>();

		PluginCallbacks getCallbacks(GameObject agent) {
			called!.Add(agent);
			return new PluginCallbacks();
		}

		var plugins = new MockPluginSO[] {
			ScriptableObject.CreateInstance<MockPluginSO>(),
			ScriptableObject.CreateInstance<MockPluginSO>(),
		};
		plugins.ForEach(plugin => plugin.getCallbacks = getCallbacks);
		instructionsSO.plugins = plugins;

		var insructions = instructionsSO.GetInstructionsFor(agent)!;

		Assert.AreEqual((agent, agent), (called[0], called[1]));
	}

	[Test]
	public void OnBeginBeforeFirstYield() {
		var called = null as PluginData;
		var agent = new GameObject();
		var instructionsSO = ScriptableObject.CreateInstance<MockInstructionSO>();

		PluginCallbacks getCallbacks(GameObject agent) {
			return new PluginCallbacks { onBegin = d => called = d };
		}

		var plugins = new MockPluginSO[] {
			ScriptableObject.CreateInstance<MockPluginSO>(),
			ScriptableObject.CreateInstance<MockPluginSO>(),
		};
		plugins.ForEach(pl => pl.getCallbacks = getCallbacks);
		instructionsSO.plugins = plugins;
		instructionsSO.insructions = _ => _ => new YieldInstruction[] {
			new WaitForEndOfFrame(),
			new WaitForEndOfFrame(),
			new WaitForEndOfFrame(),
		};

		var insructions = instructionsSO.GetInstructionsFor(agent);

		Assert.IsNull(called);

		foreach (var hold in insructions()!) break;

		Assert.NotNull(called);
	}

	[Test]
	public void PluginDataAllSame() {
		var data = new List<PluginData>();
		var agent = new GameObject();
		var instructionsSO = ScriptableObject.CreateInstance<MockInstructionSO>();
		var plugins = new MockPluginSO[] {
			ScriptableObject.CreateInstance<MockPluginSO>(),
			ScriptableObject.CreateInstance<MockPluginSO>(),
			ScriptableObject.CreateInstance<MockPluginSO>(),
		};
		plugins.ForEach(pl => pl.getCallbacks = _ => new PluginCallbacks {
			onBegin = d => data.Add(d),
			onBeforeYield = d => data.Add(d),
			onAfterYield = d => data.Add(d),
			onEnd = d => data.Add(d),
		});

		instructionsSO.insructions = _ => d => {
			data.Add(d);
			return new YieldInstruction[0];
		};
		instructionsSO.plugins = plugins;
		instructionsSO.insructions = _ => _ => new YieldInstruction[] {
			new WaitForEndOfFrame(),
			new WaitForEndOfFrame(),
			new WaitForEndOfFrame(),
		};

		var insructions = instructionsSO.GetInstructionsFor(agent);

		foreach (var _ in insructions()!) ;

		Assert.AreEqual(1, data.Distinct().Count());
	}

	[Test]
	public void OnAfterYield() {
		var called = 0;
		var agent = new GameObject();
		var instructionsSO = ScriptableObject.CreateInstance<MockInstructionSO>();

		PluginCallbacks getCallbacks(GameObject agent) => new PluginCallbacks {
			onAfterYield = _ => ++called
		};

		var plugins = new MockPluginSO[] {
			ScriptableObject.CreateInstance<MockPluginSO>(),
			ScriptableObject.CreateInstance<MockPluginSO>(),
		};
		plugins.ForEach(pl => pl.getCallbacks = getCallbacks);
		instructionsSO.plugins = plugins;
		instructionsSO.insructions = _ => _ => new YieldInstruction[] {
			new WaitForEndOfFrame(),
			new WaitForEndOfFrame(),
			new WaitForEndOfFrame(),
		};

		var insructions = instructionsSO.GetInstructionsFor(agent);

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
		var instructionsSO = ScriptableObject.CreateInstance<MockInstructionSO>();

		PluginCallbacks getCallbacks(GameObject agent) => new PluginCallbacks {
			onBeforeYield = _ => ++called
		};

		var plugins = new MockPluginSO[] {
			ScriptableObject.CreateInstance<MockPluginSO>(),
			ScriptableObject.CreateInstance<MockPluginSO>(),
		};
		plugins.ForEach(pl => pl.getCallbacks = getCallbacks);
		instructionsSO.plugins = plugins;
		instructionsSO.insructions = _ => _ => new YieldInstruction[] {
			new WaitForEndOfFrame(),
			new WaitForEndOfFrame(),
			new WaitForEndOfFrame(),
		};

		var insructions = instructionsSO.GetInstructionsFor(agent);

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
		var instructionsSO = ScriptableObject.CreateInstance<MockInstructionSO>();

		PluginCallbacks getCallbacks(GameObject agent) =>
			new PluginCallbacks { onEnd = _ => ++called };

		var plugins = new MockPluginSO[] {
			ScriptableObject.CreateInstance<MockPluginSO>(),
			ScriptableObject.CreateInstance<MockPluginSO>(),
		};
		plugins.ForEach(pl => pl.getCallbacks = getCallbacks);
		instructionsSO.plugins = plugins;
		instructionsSO.insructions = _ => _ => new YieldInstruction[] {
			new WaitForEndOfFrame(),
			new WaitForEndOfFrame(),
			new WaitForEndOfFrame(),
		};

		var insructions = instructionsSO.GetInstructionsFor(agent);

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
		var instructionsSO = ScriptableObject.CreateInstance<MockInstructionSO>();
		var plugin = ScriptableObject.CreateInstance<MockPluginSO>();

		IEnumerable<YieldInstruction> instructionFunc(PluginData _) {
			for (iterations = 0; iterations < 100; ++iterations) {
				yield return new WaitForEndOfFrame();
			}
		}

		plugin.getCallbacks = _ => new PluginCallbacks {
			onEnd = _ => ++called
		};
		instructionsSO.plugins = new MockPluginSO[] { plugin }; ;
		instructionsSO.insructions = _ => instructionFunc;

		var insructions = instructionsSO.GetInstructionsFor(agent, () => run);

		foreach (var _ in insructions()!) {
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
		var instructionsSO = ScriptableObject.CreateInstance<MockInstructionSO>();
		var plugin = ScriptableObject.CreateInstance<MockPluginSO>();

		IEnumerable<UnityEngine.YieldInstruction> instructionFunc(PluginData _) {
			for (iterations = 0; iterations < 100; ++iterations) {
				yield return new WaitForEndOfFrame();
			}
		}
		var pluginData = null as CorePluginData;
		plugin.getCallbacks = _ => new PluginCallbacks {
			onBegin = d => pluginData = d.As<CorePluginData>(),
			onEnd = _ => ++called
		};
		instructionsSO.plugins = new MockPluginSO[] { plugin }; ;
		instructionsSO.insructions = _ => instructionFunc;

		var insructions = instructionsSO.GetInstructionsFor(agent);

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
		var instructionsSO = ScriptableObject.CreateInstance<MockInstructionSO>();
		var run = true;
		var plugin = ScriptableObject.CreateInstance<MockPluginSO>();

		IEnumerable<UnityEngine.YieldInstruction> instructionFunc(PluginData _) {
			for (iterations = 0; iterations < 100; ++iterations) {
				yield return new WaitForEndOfFrame();
			}
		}
		var pluginData = null as CorePluginData;
		plugin.getCallbacks = _ => new PluginCallbacks {
			onBegin = d => pluginData = d.As<CorePluginData>(),
			onEnd = _ => ++called
		};
		instructionsSO.plugins = new MockPluginSO[] { plugin }; ;
		instructionsSO.insructions = _ => instructionFunc;

		var insructions = instructionsSO.GetInstructionsFor(agent, () => run);

		foreach (var _ in insructions()!) {
			if (iterations == 9) {
				pluginData!.run = false;
			};
		};

		Assert.AreEqual((9, 1), (iterations, called));
	}

	[Test]
	public void ReturnNullWhenInstructionsNull() {
		var agent = new GameObject();
		var instructionsSO = ScriptableObject.CreateInstance<MockInstructionSO>();

		instructionsSO.insructions = _ => _ => null;

		var insructions = instructionsSO.GetInstructionsFor(agent);

		Assert.Null(insructions());
	}

	class MockPluginDataA : PluginData { }
	class MockPluginDataB : PluginData { }

	[Test]
	public void DecoratePluginData() {
		var data = null as PluginData;
		var agent = new GameObject();
		var instructionsSO = ScriptableObject.CreateInstance<MockInstructionSO>();
		var plugins = new MockPluginSO[] {
			ScriptableObject.CreateInstance<MockPluginSO>(),
			ScriptableObject.CreateInstance<MockPluginSO>(),
			ScriptableObject.CreateInstance<MockPluginSO>(),
		};

		plugins[0].extendPlugin = d => d.Extent<MockPluginDataA>();
		plugins[1].extendPlugin = d => d.Extent<MockPluginDataB>();
		plugins[2].getCallbacks = _ => new PluginCallbacks {
			onBegin = d => data = d
		};
		instructionsSO.plugins = plugins;

		var insructions = instructionsSO.GetInstructionsFor(agent);

		foreach (var _ in insructions()!) ;

		Assert.NotNull(data!.As<MockPluginDataA>());
		Assert.NotNull(data!.As<MockPluginDataB>());
	}
}
