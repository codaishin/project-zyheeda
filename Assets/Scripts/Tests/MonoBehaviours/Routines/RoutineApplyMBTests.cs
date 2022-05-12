using System;
using System.Collections;
using NUnit.Framework;
using Routines;
using UnityEngine;
using UnityEngine.TestTools;

public class RoutineApplyMBTests : TestCollection
{
	class MockRoutineFactoryMB : MonoBehaviour, IFactory
	{
		public IRoutine? GetRoutine() {
			throw new NotImplementedException();
		}
	}

	class MockRunMB : MonoBehaviour, IApplicable<IFactory>
	{
		public Action<IFactory> apply = _ => { };

		public void Apply(IFactory value) =>
			this.apply(value);
	}

	[UnityTest]
	public IEnumerator Apply() {
		var runner = new GameObject().AddComponent<MockRunMB>();
		var factory = new GameObject().AddComponent<MockRoutineFactoryMB>();
		var apply = new GameObject().AddComponent<RoutineApplyMB>();
		var called = null as IFactory;

		apply.runner = Reference<IApplicable<IFactory>>.Component(runner);
		apply.routineFactory = Reference<IFactory>.Component(factory);

		runner.apply = param => called = param;

		yield return new WaitForEndOfFrame();

		apply.Apply();

		Assert.AreSame(factory, called);
	}
}
