using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Reflection;
using System;

public class PlayerAnimatorMBTests
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

		player.Walk(true);

		Assert.AreEqual(("walk", true), called);
	}

	[Test]
	public void WalkFalse() {
		var called = ("", true);
		var player = new GameObject("player").AddComponent<MockPlayerAnimatorMB>();
		var animator = new MockAnimator { setBool = (n, v) => called = (n, v) };

		player.animator = animator;

		player.Walk(false);

		Assert.AreEqual(("walk", false), called);
	}
}
