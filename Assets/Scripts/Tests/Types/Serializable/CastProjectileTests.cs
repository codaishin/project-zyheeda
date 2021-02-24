using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class CastProjectileTests : TestCollection
{
  [Test]
	public void GetPosition()
	{
		var obj = new GameObject("obj");
		var appraoch = new CastProjectileApproach();
		obj.transform.position = Vector3.down;

		Assert.AreEqual(Vector3.down, appraoch.GetPosition(obj));
	}

	[UnityTest]
	public IEnumerator GetTimeDelta()
	{
		var obj = new GameObject("obj");
		var appraoch = new CastProjectileApproach();
		obj.transform.position = Vector3.down;

		yield return new WaitForFixedUpdate();

		Assert.AreEqual(Time.fixedDeltaTime, appraoch.GetTimeDelta());
	}

	[Test]
	public void PostUpdate()
	{
		var transform = new GameObject("transform").transform;
		var target = new GameObject("target");
		var appraoch = new CastProjectileApproach();

		target.transform.position = Vector3.up;

		appraoch.PostUpdate(transform, target);

		Tools.AssertEqual(
			Quaternion.LookRotation(Vector3.up).eulerAngles,
			transform.rotation.eulerAngles
		);
	}
}
