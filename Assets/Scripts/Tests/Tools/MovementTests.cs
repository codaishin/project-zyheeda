using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class MovementTests : TestCollection
{
	[Test]
	public void GetApproach()
	{
		var obj = new GameObject("obj");
		var moveTo = Movement.GetApproach<Vector3>(v => v, () => 1);

		moveTo(obj.transform, Vector3.right, 0f).MoveNext();

		Assert.AreEqual(Vector3.right, obj.transform.position);
	}

	[Test]
	public void GetApproachDeltaPerSecond()
	{
		var obj = new GameObject("obj");
		var moveTo = Movement.GetApproach<Vector3>(v => v, () => 1);

		moveTo(obj.transform, Vector3.right, 0.1f).MoveNext();

		Assert.AreEqual(Vector3.right * 0.1f, obj.transform.position);
	}

	[Test]
	public void GetApproachDeltaPerSecondFromOffset()
	{
		var obj = new GameObject("obj");
		var moveTo = Movement.GetApproach<Vector3>(v => v, () => 1);

		obj.transform.position = Vector3.left;

		moveTo(obj.transform, Vector3.right, 0.1f).MoveNext();

		Assert.AreEqual(Vector3.left + Vector3.right * 0.1f, obj.transform.position);
	}

	[Test]
	public void GetApproachTimeDelta()
	{
		var obj = new GameObject("obj");
		var moveTo = Movement.GetApproach<Vector3>(v => v, () => 0.3f);

		moveTo(obj.transform, Vector3.right, 1).MoveNext();

		Assert.AreEqual(Vector3.right * 0.3f, obj.transform.position);
	}

	[Test]
	public void GetApproachMultipleFrames()
	{
		var positions = new List<Vector3>();
		var obj = new GameObject("obj");
		var moveTo = Movement.GetApproach<Vector3>(v => v, () => 1);
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
	public void GetApproachTerminates()
	{
		var obj = new GameObject("obj");
		var moveTo = Movement.GetApproach<Vector3>(v => v, () => 1);
		var r = moveTo(obj.transform, Vector3.right, 0.51f);

		Assert.AreEqual((true, true, false), (r.MoveNext(), r.MoveNext(), r.MoveNext()));
	}

	[Test]
	public void GetApproachPostUpdateCalled()
	{
		var calledWith = new List<(Transform, Vector3)>();
		var obj = new GameObject("obj");
		var moveTo = Movement.GetApproach<Vector3>(v => v, () => 1, (tr, ta) => calledWith.Add((tr, ta)));
		var r = moveTo(obj.transform, Vector3.right, 0.2f);

		r.MoveNext();
		r.MoveNext();

		CollectionAssert.AreEqual(
			new (Transform, Vector3)[] { (obj.transform, Vector3.right), (obj.transform, Vector3.right) },
			calledWith
		);
	}

	[Test]
	public void GetApproachPostUpdateAfterPositionUpdate()
	{
		var called = default(Vector3);
		var obj = new GameObject("obj");
		var moveTo = Movement.GetApproach<Vector3>(v => v, () => 1, (tr, __) => called = tr.position);
		var r = moveTo(obj.transform, Vector3.right, 0.2f);

		r.MoveNext();

		Assert.AreEqual(new Vector3(0.2f, 0, 0), called);
	}
}
