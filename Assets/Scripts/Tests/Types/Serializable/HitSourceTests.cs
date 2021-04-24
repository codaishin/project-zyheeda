using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class HitSourceTests : TestCollection
{
	private class MockClass {}

	[Test]
	public void TryHitHitSource()
	{
		var hitter = new HitSource();
		var source = new MockClass();
		hitter.Try(source, out var target);
		Assert.AreSame(source, target);
	}

	[Test]
	public void TryHitReturnsTrue()
	{
		var hitter = new HitSource();
		Assert.True(hitter.Try(default(MockClass), out _));
	}
}
