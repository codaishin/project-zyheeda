using NUnit.Framework;

public class DisposableTests : TestCollection
{
	[Test]
	public void OnDispose()
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
