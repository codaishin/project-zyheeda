using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ReferenceSetterMBTests : TestCollection
{
	[Test]
	public void Set() {
		var obj = new GameObject("obj");
		var setterMB = obj.AddComponent<ReferenceSetterMB>();
		var referenceSO = ScriptableObject.CreateInstance<ReferenceSO>();

		setterMB.referenceSO = referenceSO;
		setterMB.SetReference();

		Assert.AreSame(obj, referenceSO.GameObject);
	}

	[Test]
	public void ErrorWhenGameObjectAlreadySet() {
		var obj = new GameObject("obj");
		var setterMB = obj.AddComponent<ReferenceSetterMB>();
		var referenceSO = ScriptableObject.CreateInstance<ReferenceSO>();

		setterMB.referenceSO = referenceSO;
		referenceSO.GameObject = new GameObject("other");

		Assert.Throws<ArgumentException>(() => setterMB.SetReference());
	}

	[Test]
	public void ErrorWhenGameObjectAlreadySetMessage() {
		var obj = new GameObject("obj");
		var setterMB = obj.AddComponent<ReferenceSetterMB>();
		var referenceSO = ScriptableObject.CreateInstance<ReferenceSO>();

		setterMB.referenceSO = referenceSO;
		referenceSO.GameObject = new GameObject("other");
		referenceSO.name = "my object";

		try {
			setterMB.SetReference();
		}
		catch (ArgumentException e) {
			Assert.AreEqual("\"my object\" already set to \"other\"", e.Message);
		}
	}

	[Test]
	public void NoErrorWhenAlreadySetToSelf() {
		var obj = new GameObject("obj");
		var setterMB = obj.AddComponent<ReferenceSetterMB>();
		var referenceSO = ScriptableObject.CreateInstance<ReferenceSO>();

		setterMB.referenceSO = referenceSO;
		referenceSO.GameObject = obj;

		Assert.DoesNotThrow(() => setterMB.SetReference());
	}

	[UnityTest]
	public IEnumerator SetOnStart() {
		var obj = new GameObject("obj");
		var setterMB = obj.AddComponent<ReferenceSetterMB>();
		var referenceSO = ScriptableObject.CreateInstance<ReferenceSO>();

		setterMB.referenceSO = referenceSO;

		yield return new WaitForEndOfFrame();

		Assert.AreSame(obj, referenceSO.GameObject);
	}

	[UnityTest]
	public IEnumerator UnsetOnDestroy() {
		var obj = new GameObject("obj");
		var setterMB = obj.AddComponent<ReferenceSetterMB>();
		var referenceSO = ScriptableObject.CreateInstance<ReferenceSO>();

		setterMB.referenceSO = referenceSO;

		yield return new WaitForEndOfFrame();

		UnityEngine.Object.Destroy(obj);

		yield return new WaitForEndOfFrame();

		Assert.Throws<NullReferenceException>(() => _ = referenceSO.GameObject);
	}
}
