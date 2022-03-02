using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;


public abstract class TestCollection
{
	[UnityTearDown]
	public IEnumerator DestroyGameObjects() {
		foreach (GameObject obj in Object.FindObjectsOfType<GameObject>(true)) {
			Object.Destroy(obj);
		}
		yield return null;
	}
}
