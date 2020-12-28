using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public abstract class TestCollection
{
	[TearDown]
	public void DestroyGameObjects()
	{
		foreach (GameObject obj in Object.FindObjectsOfType<GameObject>(true)) {
			Object.Destroy(obj);
		}
	}
}
