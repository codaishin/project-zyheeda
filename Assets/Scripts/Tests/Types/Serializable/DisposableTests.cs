using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class DisposableTests : TestCollection
{
	[Test]
	public void Dispose()
	{
		var called = 0;
		var disposable = new Disposable<int>(42, (in int v) => called = v);
		disposable.Dispose();

		Assert.AreEqual(42, called);
	}

	[Test]
	public void Value()
	{
		var called = 0;
		var disposable = new Disposable<int>(42, (in int v) => called = v);

		Assert.AreEqual(42, disposable.Value);
	}
}
