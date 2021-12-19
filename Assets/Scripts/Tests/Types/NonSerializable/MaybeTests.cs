using NUnit.Framework;

public class MaybeTests
{
	[Test]
	public void NewSome() {
		var some = new Some<int>(42);

		Assert.AreEqual(42, some.Value);
	}

	[Test]
	public void SomeMethod() {
		var some = Maybe.Some("Hello");

		Assert.AreEqual("Hello", some.Value);
	}

	[Test]
	public void NoneMethod() {
		var none = Maybe.None<int>();

		Assert.True(none is None<int>);
	}

	[Test]
	public void MatchSome() {
		var value = 0f;
		var calledNone = false;
		Maybe<float> maybe = Maybe.Some(0.5f);

		maybe.Match(some: v => value = v, none: () => calledNone = true);

		Assert.AreEqual((0.5f, false), (value, calledNone));
	}

	[Test]
	public void MatchNone() {
		var value = 0f;
		var calledNone = false;
		Maybe<float> maybe = Maybe.None<float>();

		maybe.Match(some: v => value = v, none: () => calledNone = true);

		Assert.AreEqual((0f, true), (value, calledNone));
	}
}
