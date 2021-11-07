using NUnit.Framework;

public class HitSourceTests : TestCollection
{
	private class MockClass { }

	[Test]
	public void TryHitHitSource() {
		var hitter = new HitSource();
		var source = new MockClass();

		hitter.Try(source).Match(
			some: target => Assert.AreSame(source, target),
			none: () => Assert.Fail("No hit")
		);
	}
}
