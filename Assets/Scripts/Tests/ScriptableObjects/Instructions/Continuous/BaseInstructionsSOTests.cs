using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

public class BaseInstructionsSOTests : TestCollection
{
	class MockPluginSO : BaseInstructionsPluginSO
	{
		public Func<GameObject, PluginData, Action> getOnBegin =
			(_, __) => () => { };
		public Func<GameObject, PluginData, Action> getOnUpdate =
			(_, __) => () => { };
		public Func<GameObject, PluginData, Action> getOnEnd =
			(_, __) => () => { };

		public override Action GetOnBegin(GameObject agent, PluginData data) {
			return this.getOnBegin(agent, data);
		}

		public override Action GetOnUpdate(GameObject agent, PluginData data) {
			return this.getOnUpdate(agent, data);
		}

		public override Action GetOnEnd(GameObject agent, PluginData data) {
			return this.getOnEnd(agent, data);
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

	[Test]
	public void GetOnBegin() {
		var called = new List<GameObject>();
		var agent = new GameObject();
		var instructionsSO = ScriptableObject.CreateInstance<MockInstructionSO>();

		Action getOnBegin(GameObject agent, PluginData data) {
			called!.Add(agent);
			return () => { };
		}

		var plugins = new MockPluginSO[] {
			ScriptableObject.CreateInstance<MockPluginSO>(),
			ScriptableObject.CreateInstance<MockPluginSO>(),
		};
		plugins.ForEach(plugin => plugin.getOnBegin = getOnBegin);
		instructionsSO.plugins = plugins;

		var insructions = instructionsSO.GetInstructionsFor(agent);

		CollectionAssert.AreEqual(new GameObject[] { agent, agent }, called);
	}

	[Test]
	public void OnBeginBeforeFirstYield() {
		var called = 0;
		var agent = new GameObject();
		var instructionsSO = ScriptableObject.CreateInstance<MockInstructionSO>();

		Action getOnBegin(GameObject agent, PluginData data) => () => ++called;

		var plugins = new MockPluginSO[] {
			ScriptableObject.CreateInstance<MockPluginSO>(),
			ScriptableObject.CreateInstance<MockPluginSO>(),
		};
		plugins.ForEach(pl => pl.getOnBegin = getOnBegin);
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
	public void GetOnEnd() {
		var called = new List<GameObject>();
		var agent = new GameObject();
		var instructionsSO = ScriptableObject.CreateInstance<MockInstructionSO>();

		Action getOnEnd(GameObject agent, PluginData data) {
			called!.Add(agent);
			return () => { };
		}

		var plugins = new MockPluginSO[] {
			ScriptableObject.CreateInstance<MockPluginSO>(),
			ScriptableObject.CreateInstance<MockPluginSO>(),
		};
		plugins.ForEach(plugin => plugin.getOnEnd = getOnEnd);
		instructionsSO.plugins = plugins;

		var insructions = instructionsSO.GetInstructionsFor(agent);

		CollectionAssert.AreEqual(new GameObject[] { agent, agent }, called);
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
		plugins.ForEach(pl => pl.getOnBegin = (_, d) => () => data.Add(d));
		plugins.ForEach(pl => pl.getOnUpdate = (_, d) => () => data.Add(d));
		plugins.ForEach(pl => pl.getOnEnd = (_, d) => () => data.Add(d));
		instructionsSO.insructions = (_, d) => () => {
			data.Add(d);
			return new YieldInstruction[] { new WaitForEndOfFrame() };
		};
		instructionsSO.plugins = plugins;

		var insructions = instructionsSO.GetInstructionsFor(agent);

		foreach (var _ in insructions()) ;

		Assert.AreEqual(1, data.Distinct().Count());
	}

	[Test]
	public void OnEndBeforeLastYield() {
		var called = 0;
		var agent = new GameObject();
		var instructionsSO = ScriptableObject.CreateInstance<MockInstructionSO>();

		Action getOnEnd(GameObject agent, PluginData data) => () => ++called;

		var plugins = new MockPluginSO[] {
			ScriptableObject.CreateInstance<MockPluginSO>(),
			ScriptableObject.CreateInstance<MockPluginSO>(),
		};
		plugins.ForEach(pl => pl.getOnEnd = getOnEnd);
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
	public void GetOnUpdate() {
		var called = new List<GameObject>();
		var agent = new GameObject();
		var instructionsSO = ScriptableObject.CreateInstance<MockInstructionSO>();
		var plugins = new MockPluginSO[] {
			ScriptableObject.CreateInstance<MockPluginSO>(),
			ScriptableObject.CreateInstance<MockPluginSO>(),
		};

		Action getOnUpdate(GameObject agent, PluginData data) {
			called!.Add(agent);
			return () => { };
		}

		plugins.ForEach(plugin => plugin.getOnUpdate = getOnUpdate);
		instructionsSO.plugins = plugins;

		var insructions = instructionsSO.GetInstructionsFor(agent);

		CollectionAssert.AreEqual(new GameObject[] { agent, agent }, called);
	}

	[Test]
	public void GracefullRelease() {
		var called = 0;
		var iterations = 0;
		var run = true;
		var agent = new GameObject();
		var instructionsSO = ScriptableObject.CreateInstance<MockInstructionSO>();
		var plugin = ScriptableObject.CreateInstance<MockPluginSO>();

		IEnumerable<UnityEngine.YieldInstruction> instructionFunc() {
			for (iterations = 0; iterations < 100; ++iterations) {
				yield return new WaitForEndOfFrame();
			}
		}

		plugin.getOnEnd = (_, __) => () => ++called;
		instructionsSO.plugins = new MockPluginSO[] { plugin }; ;
		instructionsSO.insructions = (_, __) => instructionFunc;

		var insructions = instructionsSO.GetInstructionsFor(agent, () => run);

		foreach (var _ in insructions()) {
			if (iterations == 9) {
				run = false;
			};
		};

		Assert.AreEqual((10, 1), (iterations, called));
	}
}
