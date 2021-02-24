using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class AppraochMBTests : TestCollection
{
	[Test]
	public void GetPosition()
	{
		var appraoch = new Approach();

		Assert.AreEqual(Vector3.down, appraoch.GetPosition(Vector3.down));
	}

	[UnityTest]
	public IEnumerator GetTimeDelta()
	{
		var appraoch = new Approach();

		yield return new WaitForFixedUpdate();

		Assert.AreEqual(Time.fixedDeltaTime, appraoch.GetTimeDelta());
	}

	[Test]
	public void PostUpdate()
	{
		var appraoch = new Approach();

		Assert.DoesNotThrow(() => appraoch.PostUpdate(default, default));
	}
}
