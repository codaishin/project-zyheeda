using System;
using NUnit.Framework;
using UnityEngine;

public class ReferenceSOTests : TestCollection
{
	[Test]
	public void Clear() {
		var reference = ScriptableObject.CreateInstance<ReferenceSO>();
		reference.GameObject = new GameObject("obj");
		reference.Clear();

		Assert.Throws<NullReferenceException>(() => _ = reference.GameObject);
	}

	[Test]
	public void IsSetInitialFalse() {
		var reference = ScriptableObject.CreateInstance<ReferenceSO>();

		Assert.False(reference.IsSet);
	}

	[Test]
	public void IsSetTrue() {
		var reference = ScriptableObject.CreateInstance<ReferenceSO>();
		reference.GameObject = new GameObject("obj");

		Assert.True(reference.IsSet);
	}

	[Test]
	public void IsSetFalseAfterClear() {
		var reference = ScriptableObject.CreateInstance<ReferenceSO>();
		reference.GameObject = new GameObject("obj");
		reference.Clear();

		Assert.False(reference.IsSet);
	}
}
