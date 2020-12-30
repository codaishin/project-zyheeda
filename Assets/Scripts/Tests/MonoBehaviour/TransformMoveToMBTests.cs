using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TransformMoveToMBTests : TestCollection
{
	[UnityTest]
	public IEnumerator MoveTo()
	{
		var moveTo = new GameObject("behaviour").AddComponent<TransformMoveToMB>();
		moveTo.agent = new GameObject("agent");

		yield return new WaitForEndOfFrame();

		moveTo.MoveTo(new Vector3(10, 0, 0));

		Assert.AreEqual(
			new Vector3(10, 0, 0),
			moveTo.agent.GameObject.transform.position
		);
	}

	[UnityTest]
	public IEnumerator MoveToDeltaPerSecond()
	{
		var moveTo = new GameObject("behaviour").AddComponent<TransformMoveToMB>();
		moveTo.agent = new GameObject("agent");
		moveTo.deltaPerSecond = 10;

		yield return new WaitForEndOfFrame();

		moveTo.MoveTo(new Vector3(10, 0, 0));

		Assert.AreEqual(
			new Vector3(1, 0, 0) * Time.deltaTime * 10,
			moveTo.agent.GameObject.transform.position
		);
	}

	[UnityTest]
	public IEnumerator MoveToFixed()
	{
		var moveTo = new GameObject("behaviour").AddComponent<TransformMoveToMB>();
		moveTo.agent = new GameObject("agent");

		yield return new WaitForEndOfFrame();

		moveTo.MoveToFixed(new Vector3(10, 0, 0));

		Assert.AreEqual(
			new Vector3(10, 0, 0),
			moveTo.agent.GameObject.transform.position
		);
	}

	[UnityTest]
	public IEnumerator MoveToFixedDeltaPerSecond()
	{
		var moveTo = new GameObject("behaviour").AddComponent<TransformMoveToMB>();
		moveTo.agent = new GameObject("agent");
		moveTo.deltaPerSecond = 10;

		yield return new WaitForEndOfFrame();

		moveTo.MoveToFixed(new Vector3(10, 0, 0));

		Assert.AreEqual(
			new Vector3(1, 0, 0) * Time.fixedDeltaTime * 10,
			moveTo.agent.GameObject.transform.position
		);
	}
}
