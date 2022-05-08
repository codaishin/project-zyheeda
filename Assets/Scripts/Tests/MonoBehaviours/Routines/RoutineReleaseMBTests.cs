using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Routines;
using UnityEngine;
using UnityEngine.TestTools;

public class RoutineReleaseMBTests : TestCollection
{
	class MockFactoryMB : MonoBehaviour, IFactory
	{
		public IRoutine? GetRoutine() {
			throw new NotImplementedException();
		}
	}

	class MockRoutineRunMB : MonoBehaviour, IApplicable<IFactory>
	{
		public Action<IFactory> release = _ => { };
		public void Apply(IFactory value) {
			throw new NotImplementedException();
		}

		public void Release(IFactory value) => this.release(value);
	}

	[UnityTest]
	public IEnumerator ReleaseInstructions() {
		var releaser = new GameObject().AddComponent<RoutineReleaseMB>();
		var factories = new MockFactoryMB[] {
			new GameObject().AddComponent<MockFactoryMB>(),
			new GameObject().AddComponent<MockFactoryMB>(),
			new GameObject().AddComponent<MockFactoryMB>(),
		};
		var runner = new GameObject().AddComponent<MockRoutineRunMB>();
		var called = new List<IFactory>();

		releaser.runner = Reference<IApplicable<IFactory>>.Component(runner);
		releaser.routineFactories = factories
			.Select(Reference<IFactory>.Component)
			.ToArray();

		runner.release = instructions => called.Add(instructions);

		yield return new WaitForEndOfFrame();

		releaser.Apply();

		CollectionAssert.AreEqual(factories, called);
	}
}
