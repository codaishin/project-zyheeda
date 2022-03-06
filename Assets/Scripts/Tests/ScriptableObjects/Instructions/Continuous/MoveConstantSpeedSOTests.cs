using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class MoveConstantSpeedSOTests : TestCollection
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
		var moveSO = ScriptableObject.CreateInstance<MoveConstantSpeedSO>();
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
		var moveSO = ScriptableObject.CreateInstance<MoveConstantSpeedSO>();
		var hitSO = ScriptableObject.CreateInstance<MockHitSO>();
		var agent = new GameObject();
		var runner = new GameObject().AddComponent<MockMB>();
		var getRoutine = moveSO.GetInstructionsFor(agent);
		moveSO.hitter = hitSO;
		hitSO.getPoint = _ => Vector3.right;

		yield return new WaitForEndOfFrame();

		runner.StartCoroutine(getRoutine().GetEnumerator());

		var delta = Time.fixedDeltaTime;

		yield return new WaitForFixedUpdate();

		Assert.AreEqual(Vector3.right * delta, agent.transform.position);
	}

	[UnityTest]
	public IEnumerator MoveFromOffCenterRight() {
		var moveSO = ScriptableObject.CreateInstance<MoveConstantSpeedSO>();
		var hitSO = ScriptableObject.CreateInstance<MockHitSO>();
		var agent = new GameObject();
		var runner = new GameObject().AddComponent<MockMB>();
		var getRoutine = moveSO.GetInstructionsFor(agent);
		moveSO.hitter = hitSO;
		hitSO.getPoint = _ => new Vector3(1, 1, 0);
		agent.transform.position = new Vector3(1, 0, 0);

		yield return new WaitForEndOfFrame();

		runner.StartCoroutine(getRoutine().GetEnumerator());

		var delta = Time.fixedDeltaTime;

		yield return new WaitForFixedUpdate();

		Assert.AreEqual(new Vector3(1, delta, 0), agent.transform.position);
	}

	[UnityTest]
	public IEnumerator MoveRightTwice() {
		var moveSO = ScriptableObject.CreateInstance<MoveConstantSpeedSO>();
		var hitSO = ScriptableObject.CreateInstance<MockHitSO>();
		var agent = new GameObject();
		var routineRunner = new GameObject().AddComponent<MockMB>();
		var getRoutine = moveSO.GetInstructionsFor(agent);
		moveSO.hitter = hitSO;
		hitSO.getPoint = _ => Vector3.right;

		yield return new WaitForEndOfFrame();

		routineRunner.StartCoroutine(getRoutine().GetEnumerator());

		var delta = Time.fixedDeltaTime;

		yield return new WaitForFixedUpdate();

		delta += Time.fixedDeltaTime;

		yield return new WaitForFixedUpdate();

		Assert.AreEqual(Vector3.right * delta, agent.transform.position);
	}

	[UnityTest]
	public IEnumerator MoveRightTwiceFaster() {
		var moveSO = ScriptableObject.CreateInstance<MoveConstantSpeedSO>();
		var hitSO = ScriptableObject.CreateInstance<MockHitSO>();
		var agent = new GameObject();
		var routineRunner = new GameObject().AddComponent<MockMB>();
		var getRoutine = moveSO.GetInstructionsFor(agent);
		moveSO.hitter = hitSO;
		moveSO.speed = 2f;
		hitSO.getPoint = _ => Vector3.right;

		yield return new WaitForEndOfFrame();

		routineRunner.StartCoroutine(getRoutine().GetEnumerator());

		var delta = Time.fixedDeltaTime;

		yield return new WaitForFixedUpdate();

		delta += Time.fixedDeltaTime;

		yield return new WaitForFixedUpdate();

		Assert.AreEqual(Vector3.right * delta * 2, agent.transform.position);
	}

	[UnityTest]
	public IEnumerator NoMoveWhenNoHit() {
		var moveSO = ScriptableObject.CreateInstance<MoveConstantSpeedSO>();
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

		yield return new WaitForFixedUpdate();

		Assert.AreEqual(Vector3.up, agent.transform.position);
	}

	[UnityTest]
	public IEnumerator StopWhenOnTarget() {
		var moveSO = ScriptableObject.CreateInstance<MoveConstantSpeedSO>();
		var hitSO = ScriptableObject.CreateInstance<MockHitSO>();
		var agent = new GameObject();
		var routineRunner = new GameObject().AddComponent<MockMB>();
		var getRoutine = moveSO.GetInstructionsFor(agent);
		moveSO.hitter = hitSO;
		moveSO.speed = float.MaxValue;
		hitSO.getPoint = _ => Vector3.right;

		yield return new WaitForEndOfFrame();

		routineRunner.StartCoroutine(getRoutine().GetEnumerator());

		yield return new WaitForFixedUpdate();

		agent.transform.position = Vector3.zero;

		yield return new WaitForFixedUpdate();

		Assert.AreEqual(Vector3.zero, agent.transform.position);
	}

	[UnityTest]
	public IEnumerator FaceTarget() {
		var moveSO = ScriptableObject.CreateInstance<MoveConstantSpeedSO>();
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

		Assert.True(
			Vector3.right == agent.transform.forward,
			$"{Vector3.right} vs {agent.transform.forward}"
		);
	}
}
