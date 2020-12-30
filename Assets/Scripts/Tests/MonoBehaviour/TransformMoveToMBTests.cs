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
}
