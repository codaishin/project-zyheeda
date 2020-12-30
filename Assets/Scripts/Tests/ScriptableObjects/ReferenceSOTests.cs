using NUnit.Framework;
using UnityEngine;

public class ReferenceSOTests : TestCollection
{
	[Test]
	public void Clear()
	{
		var reference = ScriptableObject.CreateInstance<ReferenceSO>();
		reference.GameObject = new GameObject("obj");
		reference.Clear();

		Assert.Null(reference.GameObject);
	}
}
