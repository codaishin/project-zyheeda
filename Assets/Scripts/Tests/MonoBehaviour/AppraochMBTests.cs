using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class AppraochMBTests : TestCollection
{
	[Test]
	public void GetPosition() {
		var appraoch = new Approach();

		Assert.AreEqual(Vector3.down, appraoch.GetPosition(Vector3.down));
	}

	[UnityTest]
	public IEnumerator GetTimeDelta() {
		var appraoch = new Approach();

		yield return new WaitForFixedUpdate();

		Assert.AreEqual(Time.fixedDeltaTime, appraoch.GetTimeDelta());
	}

	[Test]
	public void OnPositionUpdated() {
		var appraoch = new Approach();
		var transformA = new GameObject().transform;
		var target = Vector3.up;

		transformA.position = Vector3.right;

		appraoch.OnPositionUpdated(transformA, target);

		Assert.AreEqual(
			Quaternion.LookRotation(Vector3.left + Vector3.up).eulerAngles,
			transformA.rotation.eulerAngles
		);
	}
}
