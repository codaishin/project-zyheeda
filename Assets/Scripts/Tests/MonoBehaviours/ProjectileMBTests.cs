using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;


public class ProjectileMBTests : TestCollection
{
	class MockApplicableMB : MonoBehaviour, IApplicable
	{
		public Action apply = () => { };

		public void Apply() => this.apply();
		public void Release() => throw new NotImplementedException();
	}

	[UnityTest]
	public IEnumerator ApplyTarget() {
		var called = 0;
		var applicable = new GameObject().AddComponent<MockApplicableMB>();
		var projectile = new GameObject().AddComponent<ProjectileMB>();
		var target = new GameObject().transform;

		applicable.apply = () => ++called;
		projectile.apply = Reference<IApplicable>.Component(applicable);

		yield return new WaitForEndOfFrame();

		projectile.Apply(target);

		yield return new WaitForEndOfFrame();

		Assert.AreSame(target, projectile.target);
		Assert.AreEqual(1, called);
	}

	[UnityTest]
	public IEnumerator SetTargetBeforeApplyCall() {
		var called = null as Transform;
		var applicable = new GameObject().AddComponent<MockApplicableMB>();
		var projectile = new GameObject().AddComponent<ProjectileMB>();
		var target = new GameObject().transform;

		applicable.apply = () => called = projectile.target;
		projectile.apply = Reference<IApplicable>.Component(applicable);

		yield return new WaitForEndOfFrame();

		projectile.Apply(target);

		Assert.AreSame(target, called);
	}
}
