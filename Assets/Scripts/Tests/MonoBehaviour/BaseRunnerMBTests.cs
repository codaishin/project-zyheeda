using System;
using NUnit.Framework;
using UnityEngine;

public class BaseSkillRunnerMBTests : TestCollection
{
	class MockBeginnable : IHasBegin
	{
		public Action begin;
		public void Begin() => this.begin();
	}

	class MockSkillRunnerMB : BaseRunnerMB<MockBeginnable> { }

	[Test]
	public void BeginSkill() {
		var called = false;
		var target = new MockBeginnable();
		var runner = new GameObject().AddComponent<MockSkillRunnerMB>();

		target.begin = () => called = true;
		runner.Value = target;
		runner.Begin();

		Assert.True(called);
	}
}
