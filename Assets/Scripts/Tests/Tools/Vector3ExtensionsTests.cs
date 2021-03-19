using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class Vector3ExtensionsTests : TestCollection
{
	[Test]
	public void AsEnumerable()
	{
		CollectionAssert.AreEqual(
			new float[] { 3f, 4f, -1f },
			new Vector3(3f, 4f, -1f).AsEnumerable()
		);
	}
}
