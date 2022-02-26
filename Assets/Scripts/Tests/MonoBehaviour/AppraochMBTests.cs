using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ApproachMBTests : TestCollection
{
	[Test]
	public void GetPosition() {
		var approach = new Approach();

		Assert.AreEqual(Vector3.down, approach.GetPosition(Vector3.down));
	}

	[UnityTest]
	public IEnumerator GetTimeDelta() {
		var approach = new Approach();

		yield return new WaitForFixedUpdate();

		Assert.AreEqual(Time.fixedDeltaTime, approach.GetTimeDelta());
	}

	[Test]
	public void OnPositionUpdated() {
		var approach = new Approach();
		var transformA = new GameObject().transform;
		var target = Vector3.up;

		transformA.position = Vector3.right;

		approach.OnPositionUpdated(transformA, target);

		Assert.AreEqual(
			Quaternion.LookRotation(Vector3.left + Vector3.up).eulerAngles,
			transformA.rotation.eulerAngles
		);
	}
}
