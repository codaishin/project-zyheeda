using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class MoveConstantSOTests : TestCollection
{
	class MockHitSO : BaseHitSO
	{
		public Func<Transform, Vector3?> getPoint = _ => null;

		public override Vector3? TryPoint(Transform source) {
			return this.getPoint(source);
		}

		public override T? Try<T>(T source) where T : class {
			throw new NotImplementedException();
		}
	}

	class MockMB : MonoBehaviour { }

	[UnityTest]
	public IEnumerator PassTransformToHitter() {
		Transform? transform = null;
		var moveSO = ScriptableObject.CreateInstance<MoveConstantSO>();
		var hitSO = ScriptableObject.CreateInstance<MockHitSO>();
		var agent = new GameObject();
		var runner = new GameObject().AddComponent<MockMB>();
		var getRoutine = moveSO.GetInstructionsFor(agent);
		moveSO.hitter = hitSO;
		hitSO.getPoint = t => { transform = t; return null; };

		yield return new WaitForEndOfFrame();

		runner.StartCoroutine(getRoutine().GetEnumerator());

		yield return new WaitForEndOfFrame();

		Assert.AreSame(agent.transform, transform);
	}

	[UnityTest]
	public IEnumerator MoveRight() {
		var moveSO = ScriptableObject.CreateInstance<MoveConstantSO>();
		var hitSO = ScriptableObject.CreateInstance<MockHitSO>();
		var agent = new GameObject();
		var runner = new GameObject().AddComponent<MockMB>();
		var getRoutine = moveSO.GetInstructionsFor(agent);
		moveSO.hitter = hitSO;
		hitSO.getPoint = _ => Vector3.right;

		yield return new WaitForEndOfFrame();

		runner.StartCoroutine(getRoutine().GetEnumerator());

		yield return new WaitForEndOfFrame();

		var delta = Time.deltaTime;

		Assert.AreEqual(Vector3.right * delta, agent.transform.position);
	}

	[UnityTest]
	public IEnumerator MoveFromOffCenterRight() {
		var moveSO = ScriptableObject.CreateInstance<MoveConstantSO>();
		var hitSO = ScriptableObject.CreateInstance<MockHitSO>();
		var agent = new GameObject();
		var runner = new GameObject().AddComponent<MockMB>();
		var getRoutine = moveSO.GetInstructionsFor(agent);
		moveSO.hitter = hitSO;
		hitSO.getPoint = _ => new Vector3(1, 1, 0);
		agent.transform.position = new Vector3(1, 0, 0);

		yield return new WaitForEndOfFrame();

		runner.StartCoroutine(getRoutine().GetEnumerator());

		yield return new WaitForEndOfFrame();

		var delta = Time.deltaTime;

		Assert.AreEqual(new Vector3(1, delta, 0), agent.transform.position);
	}

	[UnityTest]
	public IEnumerator MoveRightTwice() {
		var moveSO = ScriptableObject.CreateInstance<MoveConstantSO>();
		var hitSO = ScriptableObject.CreateInstance<MockHitSO>();
		var agent = new GameObject();
		var routineRunner = new GameObject().AddComponent<MockMB>();
		var getRoutine = moveSO.GetInstructionsFor(agent);
		moveSO.hitter = hitSO;
		hitSO.getPoint = _ => Vector3.right;

		yield return new WaitForEndOfFrame();

		routineRunner.StartCoroutine(getRoutine().GetEnumerator());

		yield return new WaitForEndOfFrame();

		var delta = Time.deltaTime;

		yield return new WaitForEndOfFrame();

		delta += Time.deltaTime;

		Assert.AreEqual(Vector3.right * delta, agent.transform.position);
	}

	[UnityTest]
	public IEnumerator MoveRightTwiceFaster() {
		var moveSO = ScriptableObject.CreateInstance<MoveConstantSO>();
		var hitSO = ScriptableObject.CreateInstance<MockHitSO>();
		var agent = new GameObject();
		var routineRunner = new GameObject().AddComponent<MockMB>();
		var getRoutine = moveSO.GetInstructionsFor(agent);
		moveSO.hitter = hitSO;
		moveSO.speed = 2f;
		hitSO.getPoint = _ => Vector3.right;

		yield return new WaitForEndOfFrame();

		routineRunner.StartCoroutine(getRoutine().GetEnumerator());

		yield return new WaitForEndOfFrame();

		var delta = Time.deltaTime;

		yield return new WaitForEndOfFrame();

		delta += Time.deltaTime;

		Assert.AreEqual(Vector3.right * delta * 2, agent.transform.position);
	}

	[UnityTest]
	public IEnumerator NoMoveWhenNoHit() {
		var moveSO = ScriptableObject.CreateInstance<MoveConstantSO>();
		var hitSO = ScriptableObject.CreateInstance<MockHitSO>();
		var agent = new GameObject();
		var routineRunner = new GameObject().AddComponent<MockMB>();
		var getRoutine = moveSO.GetInstructionsFor(agent);
		moveSO.hitter = hitSO;
		moveSO.speed = 2f;
		hitSO.getPoint = _ => null;
		agent.transform.position = Vector3.up;

		yield return new WaitForEndOfFrame();

		routineRunner.StartCoroutine(getRoutine().GetEnumerator());

		yield return new WaitForEndOfFrame();

		Assert.AreEqual(Vector3.up, agent.transform.position);
	}

	[UnityTest]
	public IEnumerator StopWhenOnTarget() {
		var moveSO = ScriptableObject.CreateInstance<MoveConstantSO>();
		var hitSO = ScriptableObject.CreateInstance<MockHitSO>();
		var agent = new GameObject();
		var routineRunner = new GameObject().AddComponent<MockMB>();
		var getRoutine = moveSO.GetInstructionsFor(agent);
		moveSO.hitter = hitSO;
		moveSO.speed = float.MaxValue;
		hitSO.getPoint = _ => Vector3.right;

		yield return new WaitForEndOfFrame();

		routineRunner.StartCoroutine(getRoutine().GetEnumerator());

		yield return new WaitForEndOfFrame();

		agent.transform.position = Vector3.zero;

		yield return new WaitForEndOfFrame();

		Assert.AreEqual(Vector3.zero, agent.transform.position);
	}

	class MockPluginSO : BaseInstructionsPluginSO
	{
		public Func<GameObject, PluginCallbacks> getCallbacks =
			_ => new PluginCallbacks();

		public override PluginCallbacks GetCallbacks(GameObject agent) {
			return this.getCallbacks(agent);
		}
	}

	[UnityTest]
	public IEnumerator WeightToPluginDataWeight() {
		var data = null as PluginData;
		var moveSO = ScriptableObject.CreateInstance<MoveConstantSO>();
		var hitSO = ScriptableObject.CreateInstance<MockHitSO>();
		var agent = new GameObject();
		var routineRunner = new GameObject().AddComponent<MockMB>();
		var pluginSO = ScriptableObject.CreateInstance<MockPluginSO>();
		moveSO.hitter = hitSO;
		moveSO.speed = 1;
		moveSO.weight = 0.0112f;
		moveSO.plugins = new MockPluginSO[] { pluginSO };

		hitSO.getPoint = _ => Vector3.right * 100;

		pluginSO.getCallbacks = _ => new PluginCallbacks {
			onBegin = d => data = d
		};

		yield return new WaitForEndOfFrame();

		var getRoutine = moveSO.GetInstructionsFor(agent);
		routineRunner.StartCoroutine(getRoutine().GetEnumerator());

		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		Assert.AreEqual(0.0112f, data!.weight);
	}
}