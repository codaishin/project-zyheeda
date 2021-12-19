using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class BaseApproachTests : TestCollection
{
	private class MockApproach : BaseApproach<Vector3>
	{
		public delegate void OnPositionUpdatedDunf(in Transform t, in Vector3 v);

		public float timeDelta = 1f;
		public OnPositionUpdatedDunf onPositionUpdated = (
			in Transform _,
			in Vector3 __
		) => { };

		public override Vector3 GetPosition(in Vector3 target) => target;
		public override float GetTimeDelta() => this.timeDelta;
		public override void OnPositionUpdated(
			in Transform current,
			in Vector3 target
		) => this.onPositionUpdated(current, target);
	}

	[Test]
	public void GetApproach() {
		var obj = new GameObject("obj");
		var approach = new MockApproach();

		approach.Apply(obj.transform, Vector3.right, 0f).MoveNext();

		Assert.AreEqual(Vector3.right, obj.transform.position);
	}


	[Test]
	public void GetApproachDeltaPerSecond() {
		var obj = new GameObject("obj");
		var approach = new MockApproach();

		approach.Apply(obj.transform, Vector3.right, 0.1f).MoveNext();

		Assert.AreEqual(Vector3.right * 0.1f, obj.transform.position);
	}

	[Test]
	public void GetApproachDeltaPerSecondFromOffset() {
		var obj = new GameObject("obj");
		var approach = new MockApproach();

		obj.transform.position = Vector3.left;

		approach.Apply(obj.transform, Vector3.right, 0.1f).MoveNext();

		Assert.AreEqual(Vector3.left + Vector3.right * 0.1f, obj.transform.position);
	}

	[Test]
	public void GetApproachTimeDelta() {
		var obj = new GameObject("obj");
		var approach = new MockApproach { timeDelta = 0.3f };

		approach.Apply(obj.transform, Vector3.right, 1).MoveNext();

		Assert.AreEqual(Vector3.right * 0.3f, obj.transform.position);
	}

	[Test]
	public void GetApproachMultipleFrames() {
		var positions = new List<Vector3>();
		var obj = new GameObject("obj");
		var approach = new MockApproach();
		var routine = approach.Apply(obj.transform, Vector3.right, 0.1f);

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
	public void GetApproachTerminates() {
		var obj = new GameObject("obj");
		var approach = new MockApproach();
		var r = approach.Apply(obj.transform, Vector3.right, 0.51f);

		Assert.AreEqual(
			(true, true, false),
			(r.MoveNext(), r.MoveNext(), r.MoveNext())
		);
	}

	[Test]
	public void GetApproachOnPositionUpdatedCalled() {
		var calledWith = new List<(Transform, Vector3)>();
		var obj = new GameObject("obj");
		var approach = new MockApproach();
		var r = approach.Apply(obj.transform, Vector3.right, 0.2f);

		approach.onPositionUpdated = (in Transform t, in Vector3 v) => calledWith.Add((t, v));

		r.MoveNext();
		r.MoveNext();

		CollectionAssert.AreEqual(
			new (Transform, Vector3)[] {
				(obj.transform, Vector3.right),
				(obj.transform, Vector3.right),
			},
			calledWith
		);
	}

	[Test]
	public void GetApproachOnPositionUpdatedAfterPositionUpdate() {
		var called = default(Vector3);
		var obj = new GameObject("obj");
		var approach = new MockApproach();
		var r = approach.Apply(obj.transform, Vector3.right, 0.2f);

		approach.onPositionUpdated = (in Transform t, in Vector3 _) => called = t.position;

		r.MoveNext();

		Assert.AreEqual(new Vector3(0.2f, 0, 0), called);
	}
}
