using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TransformMoveToMBTests : TestCollection
{
	[Test]
	public void MoveTo()
	{
		var moveTo = new GameObject("obj").AddComponent<TransformMoveToMB>();
		moveTo.MoveTo(new Vector3(10, 0, 0));

		Assert.AreEqual(new Vector3(10, 0, 0), moveTo.transform.position);
	}

	[Test]
	public void MoveToDeltaPerSecond()
	{
		var moveTo = new GameObject("obj").AddComponent<TransformMoveToMB>();
		moveTo.deltaPerSecond = 10;
		moveTo.MoveTo(new Vector3(10, 0, 0));

		Assert.AreEqual(
			new Vector3(1, 0, 0) * Time.deltaTime * 10,
			moveTo.transform.position
		);
	}

	[Test]
	public void MoveToFixed()
	{
		var moveTo = new GameObject("obj").AddComponent<TransformMoveToMB>();
		moveTo.MoveToFixed(new Vector3(10, 0, 0));

		Assert.AreEqual(new Vector3(10, 0, 0), moveTo.transform.position);
	}

	[Test]
	public void MoveToFixedDeltaPerSecond()
	{
		var moveTo = new GameObject("obj").AddComponent<TransformMoveToMB>();
		moveTo.deltaPerSecond = 10;
		moveTo.MoveToFixed(new Vector3(10, 0, 0));

		Assert.AreEqual(
			new Vector3(1, 0, 0) * Time.fixedDeltaTime * 10,
			moveTo.transform.position
		);
	}
}
