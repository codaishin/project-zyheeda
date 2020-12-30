using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ReferenceTests : TestCollection
{
	[Test]
	public void ExposeGameObject()
	{
		var obj = new GameObject("obj");
		var reference = new Reference(obj);

		Assert.AreSame(obj, reference.GameObject);
	}

	[Test]
	public void ExposeReferenceGameObject()
	{
		var obj = new GameObject("obj");
		var referenceSO = ScriptableObject.CreateInstance<ReferenceSO>();
		var reference = new Reference(referenceSO);

		referenceSO.GameObject = obj;
		Assert.AreSame(obj, reference.GameObject);
	}

	[Test]
	public void ExceptionWhenGameObjectAndReferenceSOSet()
	{
		var obj = new GameObject("obj");
		var referenceSO = ScriptableObject.CreateInstance<ReferenceSO>();
		var reference = new Reference(obj, referenceSO);

		Assert.Throws<System.ArgumentException>(() => _ = reference.GameObject);
	}

	[Test]
	public void ExceptionWhenGameObjectAndReferenceSOSetMessage()
	{
		var obj = new GameObject("obj");
		var referenceSO = ScriptableObject.CreateInstance<ReferenceSO>();
		var reference = new Reference(obj, referenceSO);

		try {
			_ = reference.GameObject;
		} catch (System.ArgumentException e) {
			Assert.AreEqual("\"gameObject\" and \"referenceSO\" are both set", e.Message);
		}
	}
}
