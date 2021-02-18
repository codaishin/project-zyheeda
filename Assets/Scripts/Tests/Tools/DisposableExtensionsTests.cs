using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class DisposableExtensionsTests : TestCollection
{
	[Test]
	public void AsDisposableValue()
	{
		var value = "Hello";
		var disposable = value.AsDisposable(default);
		Assert.AreSame(value, disposable.Value);
	}

	[Test]
	public void AsDisposableDispose()
	{
		var called = string.Empty;
		var value = "Hello";
		var disposable = value.AsDisposable((in string v) => called = v);
		disposable.Dispose();
		Assert.AreSame(value, called);
	}

	[Test]
	public void UseValue()
	{
		var disposable = "Hello".AsDisposable(default);
		disposable.Use(out string value);

		Assert.AreSame(disposable.Value, value);
	}

	[Test]
	public void UseReturn()
	{
		var disposable = "Hello".AsDisposable(default);

		Assert.AreSame(disposable, disposable.Use(out string _));
	}
}
