using NUnit.Framework;
using UnityEngine;

public abstract class TestCollection
{
	[TearDown]
	public void DestroyGameObjects() {
		foreach (GameObject obj in Object.FindObjectsOfType<GameObject>(true)) {
			Object.Destroy(obj);
		}
	}
}
