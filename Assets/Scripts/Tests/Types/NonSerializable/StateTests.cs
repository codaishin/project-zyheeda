using NUnit.Framework;

public class StateTests : TestCollection
{
	[Test]
	public void TransitionFail()
	{
		var a = new State<string>();

		Assert.False(a.TransitionTo("b", out _));
	}

	[Test]
	public void Transition()
	{
		var a = new State<string>();
		var b = new State<string>();

		a.Transitions["b"] = b;

		Assert.AreEqual((true, b), (a.TransitionTo("b", out var t), t));
	}

	[Test]
	public void OnExitState()
	{
		var called = false;
		var a = new State<string>();
		var b = new State<string>();

		a.Transitions["b"] = b;
		a.onExit += () => called = true;
		a.TransitionTo("b", out _);

		Assert.True(called);
	}

	[Test]
	public void OnEnterState()
	{
		var called = false;
		var a = new State<string>();
		var b = new State<string>();

		a.Transitions["b"] = b;
		b.onEnter += () => called = true;
		a.TransitionTo("b", out _);

		Assert.True(called);
	}
}
