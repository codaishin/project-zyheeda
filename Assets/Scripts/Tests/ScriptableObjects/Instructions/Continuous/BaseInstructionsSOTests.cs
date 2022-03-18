using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

public class BaseInstructionsSOTests : TestCollection
{
	class MockPluginSO : BaseInstructionsPluginSO
	{
		public Func<GameObject, PluginData, PluginCallbacks> getCallbacks =
			(_, __) => new PluginCallbacks();

		public override PluginCallbacks GetCallbacks(
			GameObject agent,
			PluginData data
		) {
			return this.getCallbacks(agent, data);
		}
	}

	class MockInstructionSO : BaseInstructionsSO<Transform>
	{
		public Func<GameObject, Transform> getConcreteAgent =
			agent => agent.transform;
		public Func<Transform, PluginData, CoroutineInstructions> insructions =
			(_, __) => () => new YieldInstruction[0];

		protected override Transform GetConcreteAgent(GameObject agent) {
			return this.getConcreteAgent(agent);
		}

		protected override CoroutineInstructions Instructions(
			Transform agent,
			PluginData data
		) {
			return this.insructions(agent, data);
		}
	}

	[Test]
	public void GetConcreteAgent() {
		var called = null as Transform;
		var instructionsSO = ScriptableObject.CreateInstance<MockInstructionSO>();
		var agent = new GameObject();

		instructionsSO.insructions = (agent, _) => {
			called = agent;
			return () => new YieldInstruction[0];
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

		instructionsSO.insructions = (agent, _) => () => moveUp(agent);

		var insructions = instructionsSO.GetInstructionsFor(agent);

		foreach (var _ in insructions()) { }

		Assert.AreEqual(Vector3.up * 3, agent.transform.position);
	}

	public void GetCallbacks() {
		var called = new List<GameObject>();
		var agent = new GameObject();
		var data = new PluginData();
		var instructionsSO = ScriptableObject.CreateInstance<MockInstructionSO>();

		PluginCallbacks getCallbacks(GameObject agent, PluginData _) {
			called.Add(agent);
			return new PluginCallbacks();
		}

		var plugins = new MockPluginSO[] {
			ScriptableObject.CreateInstance<MockPluginSO>(),
			ScriptableObject.CreateInstance<MockPluginSO>(),
		};
		plugins.ForEach(plugin => plugin.getCallbacks = getCallbacks);
		instructionsSO.plugins = plugins;

		var insructions = instructionsSO.GetInstructionsFor(agent, data);

		Assert.AreEqual((agent, agent), (called[0], called[1]));
	}

	[Test]
	public void GetCallbacksWithData() {
		var calledObj = new List<GameObject>();
		var calledData = new List<PluginData>();
		var agent = new GameObject();
		var data = new PluginData();
		var instructionsSO = ScriptableObject.CreateInstance<MockInstructionSO>();

		PluginCallbacks getCallbacks(GameObject agent, PluginData data) {
			calledObj.Add(agent);
			calledData.Add(data);
			return new PluginCallbacks();
		}

		var plugins = new MockPluginSO[] {
			ScriptableObject.CreateInstance<MockPluginSO>(),
			ScriptableObject.CreateInstance<MockPluginSO>(),
		};
		plugins.ForEach(plugin => plugin.getCallbacks = getCallbacks);
		instructionsSO.plugins = plugins;

		var insructions = instructionsSO.GetInstructionsFor(agent, data);

		Assert.AreEqual((agent, agent), (calledObj[0], calledObj[1]));
		Assert.AreEqual((data, data), (calledData[0], calledData[1]));
	}

	[Test]
	public void OnBeginBeforeFirstYield() {
		var called = 0;
		var agent = new GameObject();
		var instructionsSO = ScriptableObject.CreateInstance<MockInstructionSO>();

		PluginCallbacks getCallbacks(GameObject agent, PluginData data) {
			return new PluginCallbacks { onBegin = () => ++called };
		}

		var plugins = new MockPluginSO[] {
			ScriptableObject.CreateInstance<MockPluginSO>(),
			ScriptableObject.CreateInstance<MockPluginSO>(),
		};
		plugins.ForEach(pl => pl.getCallbacks = getCallbacks);
		instructionsSO.plugins = plugins;
		instructionsSO.insructions = (_, __) => () => new YieldInstruction[] {
			new WaitForEndOfFrame(),
			new WaitForEndOfFrame(),
			new WaitForEndOfFrame(),
		};

		var insructions = instructionsSO.GetInstructionsFor(agent);

		Assert.AreEqual(0, called);

		foreach (var hold in insructions()) break;

		Assert.AreEqual(2, called);
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
		plugins.ForEach(pl => pl.getCallbacks = (_, d) => {
			data.Add(d);
			return new PluginCallbacks();
		});
		instructionsSO.insructions = (_, d) => () => {
			data.Add(d);
			return new YieldInstruction[0];
		};
		instructionsSO.plugins = plugins;

		var insructions = instructionsSO.GetInstructionsFor(agent);

		Assert.AreEqual(1, data.Distinct().Count());
	}

	[Test]
	public void OnEndBeforeLastYield() {
		var called = 0;
		var agent = new GameObject();
		var instructionsSO = ScriptableObject.CreateInstance<MockInstructionSO>();

		PluginCallbacks getCallbacks(GameObject agent, PluginData data) =>
			new PluginCallbacks { onEnd = () => ++called };

		var plugins = new MockPluginSO[] {
			ScriptableObject.CreateInstance<MockPluginSO>(),
			ScriptableObject.CreateInstance<MockPluginSO>(),
		};
		plugins.ForEach(pl => pl.getCallbacks = getCallbacks);
		instructionsSO.plugins = plugins;
		instructionsSO.insructions = (_, __) => () => new YieldInstruction[] {
			new WaitForEndOfFrame(),
			new WaitForEndOfFrame(),
			new WaitForEndOfFrame(),
		};

		var insructions = instructionsSO.GetInstructionsFor(agent);

		foreach (var _ in insructions()) {
			Assert.AreEqual(0, called);
		};

		Assert.AreEqual(2, called);
	}

	[Test]
	public void GracefullRelease() {
		var called = 0;
		var iterations = 0;
		var agent = new GameObject();
		var instructionsSO = ScriptableObject.CreateInstance<MockInstructionSO>();
		var plugin = ScriptableObject.CreateInstance<MockPluginSO>();

		IEnumerable<UnityEngine.YieldInstruction> instructionFunc() {
			for (iterations = 0; iterations < 100; ++iterations) {
				yield return new WaitForEndOfFrame();
			}
		}

		plugin.getCallbacks =
			(_, __) => new PluginCallbacks { onEnd = () => ++called };
		instructionsSO.plugins = new MockPluginSO[] { plugin }; ;
		instructionsSO.insructions = (_, __) => instructionFunc;

		var pluginData = new PluginData { run = true };
		var insructions = instructionsSO.GetInstructionsFor(agent, pluginData);

		foreach (var _ in insructions()) {
			if (iterations == 9) {
				pluginData.run = false;
			};
		};

		Assert.AreEqual((9, 1), (iterations, called));
	}
}
