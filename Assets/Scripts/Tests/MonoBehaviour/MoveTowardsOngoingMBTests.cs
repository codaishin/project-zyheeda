using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class MoveTowardsOngoingMBTests : TestCollection
{
	[UnityTest]
	public IEnumerator MoveImmediate()
	{
		var position = new Vector3(1, 0, 0);
		var agent = new GameObject("agent");
		var mover = new GameObject("mover").AddComponent<MoveTowardsOngoingMB>();
		mover.agent = agent;

		yield return new WaitForEndOfFrame();

		mover.BeginMoveTo(position);

		Tools.AssertEqual(position, agent.transform.position);
	}

	[UnityTest]
	public IEnumerator MoveImmediateSpeed()
	{
		var position = new Vector3(10, 0, 0);
		var agent = new GameObject("agent");
		var mover = new GameObject("mover").AddComponent<MoveTowardsOngoingMB>();
		mover.agent = agent;
		mover.deltaPerSecond = 10;

		yield return new WaitForEndOfFrame();

		mover.BeginMoveTo(position);

		Tools.AssertEqual(
			Vector3.right * Time.fixedDeltaTime * 10,
			agent.transform.position
		);
	}

	[UnityTest]
	public IEnumerator MoveOneFramesSpeed()
	{
		var position = new Vector3(10, 0, 0);
		var agent = new GameObject("agent");
		var mover = new GameObject("mover").AddComponent<MoveTowardsOngoingMB>();
		mover.agent = agent;
		mover.deltaPerSecond = 10;

		yield return new WaitForEndOfFrame();

		mover.BeginMoveTo(position);

		yield return new WaitForFixedUpdate();

		Tools.AssertEqual(
			Vector3.right * Time.fixedDeltaTime * 10 * 2,
			agent.transform.position
		);
	}

	[UnityTest]
	public IEnumerator StopMoving()
	{
		var position = new Vector3(10, 0, 0);
		var agent = new GameObject("agent");
		var mover = new GameObject("mover").AddComponent<MoveTowardsOngoingMB>();
		mover.agent = agent;
		mover.deltaPerSecond = 10;

		yield return new WaitForEndOfFrame();

		mover.BeginMoveTo(position);
		mover.StopMoving();

		yield return new WaitForFixedUpdate();

		Tools.AssertEqual(
			Vector3.right * Time.fixedDeltaTime * 10,
			agent.transform.position
		);
	}

	[UnityTest]
	public IEnumerator MoveOverrideOldMove()
	{
		var right = new Vector3(10, 0, 0);
		var left = new Vector3(-10, 0, 0);
		var agent = new GameObject("agent");
		var mover = new GameObject("mover").AddComponent<MoveTowardsOngoingMB>();
		mover.agent = agent;
		mover.deltaPerSecond = 10;

		yield return new WaitForEndOfFrame();

		mover.BeginMoveTo(right);
		mover.BeginMoveTo(left);

		Tools.AssertEqual(Vector3.zero, agent.transform.position);
	}
}
