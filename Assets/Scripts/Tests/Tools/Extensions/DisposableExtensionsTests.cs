using NUnit.Framework;

public class DisposableExtensionsTests : TestCollection
{
	[Test]
	public void AsDisposableValue() {
		var value = "Hello";
		var disposable = value.AsDisposable(_ => { });
		Assert.AreSame(value, disposable.Value);
	}

	[Test]
	public void AsDisposableDispose() {
		var called = string.Empty;
		var value = "Hello";
		var disposable = value.AsDisposable(v => called = v);
		disposable.Dispose();
		Assert.AreSame(value, called);
	}
}
