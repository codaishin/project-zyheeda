using System;
using NUnit.Framework;
using UnityEngine;

public class ReferenceTests : TestCollection
{
	[Test]
	public void ExposeGameObject() {
		var obj = new GameObject("obj");
		Reference reference = obj;

		Assert.AreSame(obj, reference.GameObject);
	}

	[Test]
	public void ExposeReferenceGameObject() {
		var obj = new GameObject("obj");
		var referenceSO = ScriptableObject.CreateInstance<ReferenceSO>();
		Reference reference = referenceSO;

		referenceSO.GameObject = obj;
		Assert.AreSame(obj, reference.GameObject);
	}

	[Test]
	public void ThrowWhenNotSet() {
		var reference = new Reference();
		Assert.Throws<NullReferenceException>(() => _ = reference.GameObject);
	}
}
