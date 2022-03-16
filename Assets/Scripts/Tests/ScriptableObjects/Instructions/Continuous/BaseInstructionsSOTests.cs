using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class BaseInstructionsSOTests : TestCollection
{
	struct MockData { }

	class MockPluginSO : BaseInstructionsPluginSO<MockData>
	{
		public Func<GameObject, Action<MockData>> getOnBegin = _ => _ => { };
		public Func<GameObject, Action<MockData>> getOnEnd = _ => _ => { };

		public override Action<MockData> GetOnBegin(GameObject agent) {
			return this.getOnBegin(agent);
		}

		public override Action<MockData> GetOnEnd(GameObject agent) {
			return this.getOnEnd(agent);
		}
	}

	class MockInstructionSO : BaseInstructionsSO<Transform, MockData>
	{
		public Func<GameObject, Transform> getConcreteAgent =
			agent => agent.transform;
		public Func<Transform, CoroutineInstructions> insructions =
			_ => () => new YieldInstruction[0];

		protected override Transform GetConcreteAgent(GameObject agent) {
			return this.getConcreteAgent(agent);
		}

		protected override CoroutineInstructions Instructions(Transform agent) {
			return this.insructions(agent);
		}

		protected override MockData GetPluginData(GameObject agent) {
			return default;
		}
	}

	[Test]
	public void GetConcreteAgent() {
		var called = null as Transform;
		var instructionsSO = ScriptableObject.CreateInstance<MockInstructionSO>();
		var agent = new GameObject();

		instructionsSO.insructions = agent => {
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

		instructionsSO.insructions = agent => () => moveUp(agent);

		var insructions = instructionsSO.GetInstructionsFor(agent);

		foreach (var _ in insructions()) { }

		Assert.AreEqual(Vector3.up * 3, agent.transform.position);
	}

	[Test]
	public void GetOnBegin() {
		var called = new List<GameObject>();
		var agent = new GameObject();
		var instructionsSO = ScriptableObject.CreateInstance<MockInstructionSO>();

		Action<MockData> getOnBegin(GameObject agent) {
			called!.Add(agent);
			return _ => { };
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
	public void OnBegin() {
		var called = 0;
		var agent = new GameObject();
		var instructionsSO = ScriptableObject.CreateInstance<MockInstructionSO>();

		Action<MockData> getOnBegin(GameObject agent) => _ => ++called;

		var plugins = new MockPluginSO[] {
			ScriptableObject.CreateInstance<MockPluginSO>(),
			ScriptableObject.CreateInstance<MockPluginSO>(),
		};
		plugins.ForEach(pl => pl.getOnBegin = getOnBegin);
		instructionsSO.plugins = plugins;

		var insructions = instructionsSO.GetInstructionsFor(agent);

		foreach (var _ in insructions()) ;

		Assert.AreEqual(2, called);
	}

	[Test]
	public void OnBeginBeforeFirstYield() {
		var called = 0;
		var agent = new GameObject();
		var instructionsSO = ScriptableObject.CreateInstance<MockInstructionSO>();

		Action<MockData> getOnBegin(GameObject agent) => _ => ++called;

		var plugins = new MockPluginSO[] {
			ScriptableObject.CreateInstance<MockPluginSO>(),
			ScriptableObject.CreateInstance<MockPluginSO>(),
		};
		plugins.ForEach(pl => pl.getOnBegin = getOnBegin);
		instructionsSO.plugins = plugins;
		instructionsSO.insructions = _ => () => new YieldInstruction[] {
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

		Action<MockData> getOnEnd(GameObject agent) {
			called!.Add(agent);
			return _ => { };
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
	public void OnEnd() {
		var called = 0;
		var agent = new GameObject();
		var instructionsSO = ScriptableObject.CreateInstance<MockInstructionSO>();

		Action<MockData> getOnEnd(GameObject agent) => _ => ++called;

		var plugins = new MockPluginSO[] {
			ScriptableObject.CreateInstance<MockPluginSO>(),
			ScriptableObject.CreateInstance<MockPluginSO>(),
		};
		plugins.ForEach(pl => pl.getOnEnd = getOnEnd);
		instructionsSO.plugins = plugins;

		var insructions = instructionsSO.GetInstructionsFor(agent);

		foreach (var _ in insructions()) ;

		Assert.AreEqual(2, called);
	}

	[Test]
	public void OnEndBeforeLastYield() {
		var called = 0;
		var agent = new GameObject();
		var instructionsSO = ScriptableObject.CreateInstance<MockInstructionSO>();

		Action<MockData> getOnEnd(GameObject agent) => _ => ++called;

		var plugins = new MockPluginSO[] {
			ScriptableObject.CreateInstance<MockPluginSO>(),
			ScriptableObject.CreateInstance<MockPluginSO>(),
		};
		plugins.ForEach(pl => pl.getOnEnd = getOnEnd);
		instructionsSO.plugins = plugins;
		instructionsSO.insructions = _ => () => new YieldInstruction[] {
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
		var run = true;
		var agent = new GameObject();
		var instructionsSO = ScriptableObject.CreateInstance<MockInstructionSO>();
		var plugin = ScriptableObject.CreateInstance<MockPluginSO>();

		IEnumerable<UnityEngine.YieldInstruction> instructionFunc() {
			for (iterations = 0; iterations < 100; ++iterations) {
				yield return new WaitForEndOfFrame();
			}
		}

		plugin.getOnEnd = _ => _ => ++called;
		instructionsSO.plugins = new MockPluginSO[] { plugin }; ;
		instructionsSO.insructions = _ => instructionFunc;

		var insructions = instructionsSO.GetInstructionsFor(agent, () => run);

		foreach (var _ in insructions()) {
			if (iterations == 9) {
				run = false;
			};
		};

		Assert.AreEqual((10, 1), (iterations, called));
	}
}
