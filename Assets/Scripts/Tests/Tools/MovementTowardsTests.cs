using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class MovementTowardsTests : TestCollection
{
	[Test]
	public void MovementTowards()
	{
		var obj = new GameObject("obj");
		var moveTo = Movement.Towards<Vector3>(v => v, () => 1);

		moveTo(obj.transform, Vector3.right, 0f).MoveNext();

		Assert.AreEqual(Vector3.right, obj.transform.position);
	}

	[Test]
	public void MovementTowardsDeltaPerSecond()
	{
		var obj = new GameObject("obj");
		var moveTo = Movement.Towards<Vector3>(v => v, () => 1);

		moveTo(obj.transform, Vector3.right, 0.1f).MoveNext();

		Assert.AreEqual(Vector3.right * 0.1f, obj.transform.position);
	}

	[Test]
	public void MovementTowardsDeltaPerSecondFromOffset()
	{
		var obj = new GameObject("obj");
		var moveTo = Movement.Towards<Vector3>(v => v, () => 1);

		obj.transform.position = Vector3.left;

		moveTo(obj.transform, Vector3.right, 0.1f).MoveNext();

		Assert.AreEqual(Vector3.left + Vector3.right * 0.1f, obj.transform.position);
	}

	[Test]
	public void MovementTowardsTimeDelta()
	{
		var obj = new GameObject("obj");
		var moveTo = Movement.Towards<Vector3>(v => v, () => 0.3f);

		moveTo(obj.transform, Vector3.right, 1).MoveNext();

		Assert.AreEqual(Vector3.right * 0.3f, obj.transform.position);
	}

	[Test]
	public void MovementTowardsMultipleFrames()
	{
		var positions = new List<Vector3>();
		var obj = new GameObject("obj");
		var moveTo = Movement.Towards<Vector3>(v => v, () => 1);
		var routine = moveTo(obj.transform, Vector3.right, 0.1f);

		routine.MoveNext();

		positions.Add(obj.transform.position);

		routine.MoveNext();

		positions.Add(obj.transform.position);

		CollectionAssert.AreEqual(
			new Vector3[] { new Vector3(0.1f, 0, 0), new Vector3(0.2f, 0, 0) },
			positions
		);
	}

	[Test]
	public void MovementTowardsTerminates()
	{
		var obj = new GameObject("obj");
		var moveTo = Movement.Towards<Vector3>(v => v, () => 1);
		var r = moveTo(obj.transform, Vector3.right, 0.51f);

		Assert.AreEqual((true, true, false), (r.MoveNext(), r.MoveNext(), r.MoveNext()));
	}

	[Test]
	public void MovementTowardsPostUpdateCalled()
	{
		var calledWith = new List<(Transform, Vector3)>();
		var obj = new GameObject("obj");
		var moveTo = Movement.Towards<Vector3>(v => v, () => 1, (tr, ta) => calledWith.Add((tr, ta)));
		var r = moveTo(obj.transform, Vector3.right, 0.2f);

		r.MoveNext();
		r.MoveNext();

		CollectionAssert.AreEqual(
			new (Transform, Vector3)[] { (obj.transform, Vector3.right), (obj.transform, Vector3.right) },
			calledWith
		);
	}

	[Test]
	public void MovementTowardsPostUpdateAfterPositionUpdate()
	{
		var called = default(Vector3);
		var obj = new GameObject("obj");
		var moveTo = Movement.Towards<Vector3>(v => v, () => 1, (tr, __) => called = tr.position);
		var r = moveTo(obj.transform, Vector3.right, 0.2f);

		r.MoveNext();

		Assert.AreEqual(new Vector3(0.2f, 0, 0), called);
	}
}
