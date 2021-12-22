using System;
using NUnit.Framework;
using UnityEngine;

public class PlayerAnimatorMBTests : TestCollection
{
	private class MockAnimator : IAnimator
	{
		public Action<string, bool> setBool = (_, __) => { };
		public void SetBool(string name, bool value) => this.setBool(name, value);
	}

	private class MockPlayerAnimatorMB : BasePlayerAnimatorMB<MockAnimator> { }

	[Test]
	public void WalkTrue() {
		var called = ("", false);
		var player = new GameObject("player").AddComponent<MockPlayerAnimatorMB>();
		var animator = new MockAnimator { setBool = (n, v) => called = (n, v) };

		player.animator = animator;

		player.Move(true);

		Assert.AreEqual(("move", true), called);
	}

	[Test]
	public void WalkFalse() {
		var called = ("", true);
		var player = new GameObject("player").AddComponent<MockPlayerAnimatorMB>();
		var animator = new MockAnimator { setBool = (n, v) => called = (n, v) };

		player.animator = animator;

		player.Move(false);

		Assert.AreEqual(("move", false), called);
	}
}
