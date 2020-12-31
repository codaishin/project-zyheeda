using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class MoveTowardsOngoingMBTests : TestCollection
{
	[UnityTest]
	public IEnumerator MoveOneFrame()
	{
		var position = new Vector3(1, 0, 0);
		var agent = new GameObject("agent");
		var mover = new GameObject("mover").AddComponent<MoveTowardsOngoingMB>();
		mover.agent = agent;

		yield return new WaitForEndOfFrame();

		mover.BeginMoveTo(position);

		yield return new WaitForFixedUpdate();

		Tools.AssertEqual(position, agent.transform.position);
	}
}
