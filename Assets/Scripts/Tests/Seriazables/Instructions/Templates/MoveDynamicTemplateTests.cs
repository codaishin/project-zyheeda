using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class MoveDynamicTemplateTests : TestCollection
{
	class MockHitSO : ScriptableObject, IHit
	{
		public Func<GameObject, Vector3?> getPoint = _ => null;

		public Func<T?> Try<T>(GameObject source) where T : Component {
			throw new NotImplementedException();
		}

		public Func<Vector3?> TryPoint(GameObject source) {
			return () => this.getPoint(source);
		}
	}

	class MockMB : MonoBehaviour { }

	class MockPluginSO : ScriptableObject, IPlugin
	{
		public Func<GameObject, PartialPluginCallbacks> getCallbacks =
			_ => _ => new PluginCallbacks();

		public PartialPluginCallbacks GetCallbacks(
			GameObject agent
		) => this.getCallbacks(agent);
	}

	[UnityTest]
	public IEnumerator PassAgentToHitter() {
		var calledAgent = null as GameObject;
		var move = new MoveDynamicTemplate();
		var hitSO = ScriptableObject.CreateInstance<MockHitSO>();
		var agent = new GameObject();
		var runner = new GameObject().AddComponent<MockMB>();
		move.hitter = Reference<IHit>.ScriptableObject(hitSO);
		hitSO.getPoint = a => { calledAgent = a; return null; };

		yield return new WaitForEndOfFrame();

		var getRoutine = move.GetInstructionsFor(agent)!;
		runner.StartCoroutine(getRoutine()!.GetEnumerator());

		yield return new WaitForEndOfFrame();

		Assert.AreSame(agent, calledAgent);
	}

	[UnityTest]
	public IEnumerator MoveRight() {
		var move = new MoveDynamicTemplate();
		var hitSO = ScriptableObject.CreateInstance<MockHitSO>();
		var agent = new GameObject();
		var runner = new GameObject().AddComponent<MockMB>();
		move.hitter = Reference<IHit>.ScriptableObject(hitSO);
		move.min = new MoveDynamicTemplate.ValueSet {
			speed = 1,
			distance = 1,
		};
		move.max = new MoveDynamicTemplate.ValueSet {
			speed = 1,
			distance = 1,
		};
		hitSO.getPoint = _ => Vector3.right;

		yield return new WaitForEndOfFrame();

		var getRoutine = move.GetInstructionsFor(agent)!;
		runner.StartCoroutine(getRoutine()!.GetEnumerator());

		yield return new WaitForEndOfFrame();

		Assert.AreEqual(Vector3.right * Time.deltaTime, agent.transform.position);
	}

	[UnityTest]
	public IEnumerator MoveUpMaxSpeed() {
		var move = new MoveDynamicTemplate();
		var hitSO = ScriptableObject.CreateInstance<MockHitSO>();
		var agent = new GameObject();
		var runner = new GameObject().AddComponent<MockMB>();
		move.hitter = Reference<IHit>.ScriptableObject(hitSO);
		move.min = new MoveDynamicTemplate.ValueSet {
			speed = 1,
			distance = 1,
		};
		move.max = new MoveDynamicTemplate.ValueSet {
			speed = 5,
			distance = 10,
		};
		hitSO.getPoint = _ => Vector3.up * 100;

		yield return new WaitForEndOfFrame();

		var getRoutine = move.GetInstructionsFor(agent)!;
		runner.StartCoroutine(getRoutine()!.GetEnumerator());

		yield return new WaitForEndOfFrame();

		Assert.AreEqual(Vector3.up * Time.deltaTime * 5, agent.transform.position);
	}

	[UnityTest]
	public IEnumerator MoveLeftMaxSpeedContinuous() {
		var move = new MoveDynamicTemplate();
		var hitSO = ScriptableObject.CreateInstance<MockHitSO>();
		var agent = new GameObject();
		var runner = new GameObject().AddComponent<MockMB>();
		move.hitter = Reference<IHit>.ScriptableObject(hitSO);
		move.min = new MoveDynamicTemplate.ValueSet {
			speed = 1,
			distance = 1,
		};
		move.max = new MoveDynamicTemplate.ValueSet {
			speed = 5,
			distance = 10,
		};
		hitSO.getPoint = _ => Vector3.left * 100;

		yield return new WaitForEndOfFrame();

		var getRoutine = move.GetInstructionsFor(agent)!;
		runner.StartCoroutine(getRoutine()!.GetEnumerator());

		yield return new WaitForEndOfFrame();

		var expected = Vector3.left * Time.deltaTime * 5;

		yield return new WaitForEndOfFrame();

		expected += Vector3.left * Time.deltaTime * 5;

		yield return new WaitForEndOfFrame();

		expected += Vector3.left * Time.deltaTime * 5;

		Assert.AreEqual(expected, agent.transform.position);
	}

	[UnityTest]
	public IEnumerator MoveChanginPositionMaxSpeedContinuous() {
		var move = new MoveDynamicTemplate();
		var hitSO = ScriptableObject.CreateInstance<MockHitSO>();
		var agent = new GameObject();
		var runner = new GameObject().AddComponent<MockMB>();
		move.hitter = Reference<IHit>.ScriptableObject(hitSO);
		move.min = new MoveDynamicTemplate.ValueSet {
			speed = 1,
			distance = 1,
		};
		move.max = new MoveDynamicTemplate.ValueSet {
			speed = 10,
			distance = 10,
		};

		yield return new WaitForEndOfFrame();

		var getRoutine = move.GetInstructionsFor(agent)!;
		runner.StartCoroutine(getRoutine()!.GetEnumerator());

		hitSO.getPoint = _ => Vector3.up * 100;

		yield return new WaitForEndOfFrame();

		var expected = Vector3.up * Time.deltaTime * 10;
		Assert.AreEqual(expected, agent.transform.position);
		hitSO.getPoint = _ => agent.transform.position + Vector3.right * 100;

		yield return new WaitForEndOfFrame();

		expected += Vector3.right * Time.deltaTime * 10;
		Assert.AreEqual(expected, agent.transform.position);
		hitSO.getPoint = _ => agent.transform.position + Vector3.down * 100;

		yield return new WaitForEndOfFrame();

		expected += Vector3.down * Time.deltaTime * 10;
		Assert.AreEqual(expected, agent.transform.position);
	}

	[UnityTest]
	public IEnumerator MoveChanginPositionUseLastIfCurrentlyNoHit() {
		var move = new MoveDynamicTemplate();
		var hitSO = ScriptableObject.CreateInstance<MockHitSO>();
		var agent = new GameObject();
		var runner = new GameObject().AddComponent<MockMB>();
		move.hitter = Reference<IHit>.ScriptableObject(hitSO);
		move.min = new MoveDynamicTemplate.ValueSet {
			speed = 1,
			distance = 1,
		};
		move.max = new MoveDynamicTemplate.ValueSet {
			speed = 5,
			distance = 10,
		};

		yield return new WaitForEndOfFrame();

		var getRoutine = move.GetInstructionsFor(agent)!;
		runner.StartCoroutine(getRoutine()!.GetEnumerator());

		hitSO.getPoint = _ => Vector3.up * 100;

		yield return new WaitForEndOfFrame();

		var expected = Vector3.up * Time.deltaTime * 5;

		hitSO.getPoint = _ => null;

		yield return new WaitForEndOfFrame();

		expected += Vector3.up * Time.deltaTime * 5;

		yield return new WaitForEndOfFrame();

		expected += Vector3.up * Time.deltaTime * 5;

		Assert.AreEqual(expected, agent.transform.position);
	}

	[UnityTest]
	public IEnumerator MoveLeftMinSpeed() {
		var move = new MoveDynamicTemplate();
		var hitSO = ScriptableObject.CreateInstance<MockHitSO>();
		var agent = new GameObject();
		var runner = new GameObject().AddComponent<MockMB>();
		move.hitter = Reference<IHit>.ScriptableObject(hitSO);
		move.min = new MoveDynamicTemplate.ValueSet {
			speed = 1,
			distance = 500,
		};
		move.max = new MoveDynamicTemplate.ValueSet {
			speed = 5,
			distance = 1000,
		};

		yield return new WaitForEndOfFrame();

		var getRoutine = move.GetInstructionsFor(agent)!;
		runner.StartCoroutine(getRoutine()!.GetEnumerator());


		hitSO.getPoint = _ => Vector3.left * 100;

		yield return new WaitForEndOfFrame();

		var expected = Vector3.left * Time.deltaTime;

		yield return new WaitForEndOfFrame();

		expected += Vector3.left * Time.deltaTime;

		yield return new WaitForEndOfFrame();

		expected += Vector3.left * Time.deltaTime;

		Assert.AreEqual(expected, agent.transform.position);
	}

	[UnityTest]
	public IEnumerator MoveDownMediumSpeed() {
		var move = new MoveDynamicTemplate();
		var hitSO = ScriptableObject.CreateInstance<MockHitSO>();
		var agent = new GameObject();
		var runner = new GameObject().AddComponent<MockMB>();
		move.hitter = Reference<IHit>.ScriptableObject(hitSO);
		move.min = new MoveDynamicTemplate.ValueSet {
			speed = 10,
			distance = 100,
		};
		move.max = new MoveDynamicTemplate.ValueSet {
			speed = 20,
			distance = 1100,
		};

		yield return new WaitForEndOfFrame();

		var getRoutine = move.GetInstructionsFor(agent)!;
		runner.StartCoroutine(getRoutine()!.GetEnumerator());

		hitSO.getPoint = _ => agent.transform.position + Vector3.left * 600;

		yield return new WaitForEndOfFrame();

		hitSO.getPoint = _ => agent.transform.position + Vector3.left * 600;
		var expected = Vector3.left * Time.deltaTime * 15f;

		yield return new WaitForEndOfFrame();

		hitSO.getPoint = _ => agent.transform.position + Vector3.left * 600;
		expected += Vector3.left * Time.deltaTime * 15f;

		yield return new WaitForEndOfFrame();

		expected += Vector3.left * Time.deltaTime * 15f;

		Assert.AreEqual(expected, agent.transform.position);
	}

	[UnityTest]
	public IEnumerator UpdateWeight() {
		var weights = new List<float>();
		var move = new MoveDynamicTemplate();
		var hitSO = ScriptableObject.CreateInstance<MockHitSO>();
		var plugin = ScriptableObject.CreateInstance<MockPluginSO>();
		var agent = new GameObject();
		var runner = new GameObject().AddComponent<MockMB>();
		move.hitter = Reference<IHit>.ScriptableObject(hitSO);
		move.min = new MoveDynamicTemplate.ValueSet {
			speed = 10,
			distance = 100,
			weight = 42,
		};
		move.max = new MoveDynamicTemplate.ValueSet {
			speed = 20,
			distance = 1100,
			weight = 200,
		};

		move.plugins = new Reference<IPlugin>[] {
			Reference<IPlugin>.ScriptableObject(plugin),
		};

		plugin.getCallbacks = _ => d => new PluginCallbacks {
			onAfterYield = () => weights.Add(d.As<CorePluginData>()!.weight)
		};

		yield return new WaitForEndOfFrame();

		var getRoutine = move.GetInstructionsFor(agent)!;
		runner.StartCoroutine(getRoutine()!.GetEnumerator());

		hitSO.getPoint = _ => agent.transform.position + Vector3.left * 5000;

		yield return new WaitForEndOfFrame();

		hitSO.getPoint = _ => agent.transform.position + Vector3.left * 10;

		yield return new WaitForEndOfFrame();

		hitSO.getPoint = _ => agent.transform.position + Vector3.left * 600;

		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		Assert.AreEqual((200f, 42f, 121f), (weights[1], weights[2], weights[3]));
	}
}
