using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class BaseInstructionsSOTests : TestCollection
{
	class MockPluginSO : BaseInstructionsPluginSO
	{
		public Func<GameObject, Action> getOnBegin = _ => () => { };
		public Func<GameObject, Action> getOnEnd = _ => () => { };

		public override Action GetOnBegin(GameObject agent) =>
			this.getOnBegin(agent);
		public override Action? GetOnEnd(GameObject agent) =>
			this.getOnEnd(agent);
	}

	class MockInstructionSO : BaseInstructionsSO<Transform>
	{
		public Func<GameObject, Transform> getConcreteAgent =
			agent => agent.transform;
		public Func<Transform, CoroutineInstructions> insructions =
			_ => () => new YieldInstruction[0];

		protected override Transform GetConcreteAgent(GameObject agent) =>
			this.getConcreteAgent(agent);
		protected override CoroutineInstructions Instructions(Transform agent) =>
			this.insructions(agent);
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

		Action getOnBegin(GameObject agent) {
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
	public void OnBegin() {
		var called = 0;
		var agent = new GameObject();
		var instructionsSO = ScriptableObject.CreateInstance<MockInstructionSO>();

		Action getOnBegin(GameObject agent) => () => ++called;

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

		Action getOnBegin(GameObject agent) => () => ++called;

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

		Action getOnEnd(GameObject agent) {
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
	public void OnEnd() {
		var called = 0;
		var agent = new GameObject();
		var instructionsSO = ScriptableObject.CreateInstance<MockInstructionSO>();

		Action getOnEnd(GameObject agent) => () => ++called;

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

		Action getOnEnd(GameObject agent) => () => ++called;

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
}
