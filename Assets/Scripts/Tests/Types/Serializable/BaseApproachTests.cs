using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class BaseApproachTests
{
	private class MockApproach : BaseApproach<Vector3>
	{
		public float timeDelta = 1f;

		public override Vector3 GetPosition(in Vector3 target) => target;
		public override float GetTimeDelta() => this.timeDelta;
	}

	[Test]
	public void GetApproach()
	{
		var obj = new GameObject("obj");
		var approach = new MockApproach();

		approach.Approach(obj.transform, Vector3.right, 0f).MoveNext();

		Assert.AreEqual(Vector3.right, obj.transform.position);
	}


	[Test]
	public void GetApproachDeltaPerSecond()
	{
		var obj = new GameObject("obj");
		var approach = new MockApproach();

		approach.Approach(obj.transform, Vector3.right, 0.1f).MoveNext();

		Assert.AreEqual(Vector3.right * 0.1f, obj.transform.position);
	}

	[Test]
	public void GetApproachDeltaPerSecondFromOffset()
	{
		var obj = new GameObject("obj");
		var approach = new MockApproach();

		obj.transform.position = Vector3.left;

		approach.Approach(obj.transform, Vector3.right, 0.1f).MoveNext();

		Assert.AreEqual(Vector3.left + Vector3.right * 0.1f, obj.transform.position);
	}

	[Test]
	public void GetApproachTimeDelta()
	{
		var obj = new GameObject("obj");
		var approach = new MockApproach{ timeDelta = 0.3f };

		approach.Approach(obj.transform, Vector3.right, 1).MoveNext();

		Assert.AreEqual(Vector3.right * 0.3f, obj.transform.position);
	}

	[Test]
	public void GetApproachMultipleFrames()
	{
		var positions = new List<Vector3>();
		var obj = new GameObject("obj");
		var approach = new MockApproach();
		var routine = approach.Approach(obj.transform, Vector3.right, 0.1f);

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
		var approach = new MockApproach();
		var r = approach.Approach(obj.transform, Vector3.right, 0.51f);

		Assert.AreEqual((true, true, false), (r.MoveNext(), r.MoveNext(), r.MoveNext()));
	}

	[Test]
	public void GetApproachPostUpdateCalled()
	{
		var calledWith = new List<(Transform, Vector3)>();
		var obj = new GameObject("obj");
		var approach = new MockApproach();
		var r = approach.Approach(obj.transform, Vector3.right, 0.2f);

		approach.postUpdate = new Action<Transform, Vector3> [] {
			(tr, ta) => calledWith.Add((tr, ta)),
		};

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
		var approach = new MockApproach();
		var r = approach.Approach(obj.transform, Vector3.right, 0.2f);

		approach.postUpdate = new Action<Transform, Vector3>[] {
			(tr, __) => called = tr.position,
		};

		r.MoveNext();

		Assert.AreEqual(new Vector3(0.2f, 0, 0), called);
	}
}
