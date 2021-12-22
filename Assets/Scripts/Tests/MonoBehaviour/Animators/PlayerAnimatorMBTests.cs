using System;
using NUnit.Framework;
using UnityEngine;

public class PlayerAnimatorMBTests : TestCollection
{
	private class MockAnimator : IAnimator
	{
		public Action<string, bool> setBool = (_, __) => { };
		public Action<string, float> setFloat = (_, __) => { };

		public void SetBool(string name, bool value) =>
			this.setBool(name, value);

		public void SetFloat(string name, float value) =>
			this.setFloat(name, value);
	}

	private class MockPlayerAnimatorMB : BasePlayerAnimatorMB<MockAnimator> { }

	[Test]
	public void StartMoveOne() {
		var move = ("", false);
		var blend = ("", -1f);
		var animator = new MockAnimator {
			setBool = (n, v) => move = (n, v),
			setFloat = (n, v) => blend = (n, v),
		};
		var player = new GameObject("player").AddComponent<MockPlayerAnimatorMB>();

		player.animator = animator;

		player.StartMove(1);

		Assert.AreEqual(("move", true), move);
		Assert.AreEqual(("blendWalkRun", 1f), blend);
	}

	[Test]
	public void StartMoveZero() {
		var move = ("", false);
		var blend = ("", -1f);
		var animator = new MockAnimator {
			setBool = (n, v) => move = (n, v),
			setFloat = (n, v) => blend = (n, v),
		};
		var player = new GameObject("player").AddComponent<MockPlayerAnimatorMB>();

		player.animator = animator;

		player.StartMove(0);

		Assert.AreEqual(("move", true), move);
		Assert.AreEqual(("blendWalkRun", 0f), blend);
	}

	[Test]
	public void WalkStop() {
		var move = ("", false);
		var blend = ("", -1f);
		var animator = new MockAnimator {
			setBool = (n, v) => move = (n, v),
			setFloat = (n, v) => blend = (n, v),
		};
		var player = new GameObject("player").AddComponent<MockPlayerAnimatorMB>();

		player.animator = animator;

		player.StopMove();

		Assert.AreEqual(("move", false), move);
		Assert.AreEqual(("blendWalkRun", 0f), blend);
	}
}
