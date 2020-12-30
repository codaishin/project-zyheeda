using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ReferenceExtensionsTests : TestCollection
{
	[Test]
	public void Get()
	{
		var collider = new GameObject("obj").AddComponent<BoxCollider>();
		var reference = new Reference(collider.gameObject);
		var buffer = null as BoxCollider;

		Assert.AreSame(collider, reference.Get(ref buffer));
	}

	[Test]
	public void GetSetsBuffer()
	{
		var collider = new GameObject("obj").AddComponent<BoxCollider>();
		var reference = new Reference(collider.gameObject);
		var buffer = null as BoxCollider;

		reference.Get(ref buffer);

		Assert.AreSame(collider, buffer);
	}

	[Test]
	public void GetReturnsBufferWhenSet()
	{
		var collider = new GameObject("obj").AddComponent<BoxCollider>();
		var reference = new Reference(collider.gameObject);
		var buffer = new GameObject("buffer override").AddComponent<BoxCollider>();

		Assert.AreSame(buffer, reference.Get(ref buffer));
	}
}
