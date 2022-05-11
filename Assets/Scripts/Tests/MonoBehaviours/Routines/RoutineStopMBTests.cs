using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Routines;
using UnityEngine;
using UnityEngine.TestTools;

public class RoutineStopMBTests : TestCollection
{
	class MockFactoryMB : MonoBehaviour, IFactory
	{
		public IRoutine? GetRoutine() {
			throw new NotImplementedException();
		}
	}

	class MockRoutineRunMB : MonoBehaviour, IStoppable<IFactory>
	{
		public Action<IFactory, int> stop = (_, __) => { };

		public void Stop(IFactory value, int softStopAttempts) =>
			this.stop(value, softStopAttempts);
	}

	[UnityTest]
	public IEnumerator StopRoutine() {
		var stopController = new GameObject().AddComponent<RoutineStopMB>();
		var factories = new MockFactoryMB[] {
			new GameObject("first").AddComponent<MockFactoryMB>(),
			new GameObject("second").AddComponent<MockFactoryMB>(),
			new GameObject("third").AddComponent<MockFactoryMB>(),
		};
		var stopper = new GameObject().AddComponent<MockRoutineRunMB>();
		var called = new List<(IFactory, int)>();

		stopController.stopper = Reference<IStoppable<IFactory>>.Component(stopper);
		stopController.routineFactories = factories
			.Select(Reference<IFactory>.Component)
			.ToArray();

		stopper.stop = (source, softStopAttempts) =>
			called.Add((source, softStopAttempts));

		yield return new WaitForEndOfFrame();

		stopController.Apply();

		CollectionAssert.AreEqual(
			new[] {
				(factories[0] as IFactory, 0),
				(factories[1] as IFactory, 0),
				(factories[2] as IFactory, 0)
			},
			called
		);
	}

	[UnityTest]
	public IEnumerator StopRoutineWithSoftStopAttempts() {
		var stopController = new GameObject().AddComponent<RoutineStopMB>();
		var factories = new MockFactoryMB[] {
			new GameObject("first").AddComponent<MockFactoryMB>(),
			new GameObject("second").AddComponent<MockFactoryMB>(),
			new GameObject("third").AddComponent<MockFactoryMB>(),
		};
		var stopper = new GameObject().AddComponent<MockRoutineRunMB>();
		var called = new List<(IFactory, int)>();

		stopController.stopper = Reference<IStoppable<IFactory>>.Component(stopper);
		stopController.softStopAttempts = 42;
		stopController.routineFactories = factories
			.Select(Reference<IFactory>.Component)
			.ToArray();

		stopper.stop = (source, softStopAttempts) =>
			called.Add((source, softStopAttempts));

		yield return new WaitForEndOfFrame();

		stopController.Apply();

		CollectionAssert.AreEqual(
			new[] {
				(factories[0] as IFactory, 42),
				(factories[1] as IFactory, 42),
				(factories[2] as IFactory, 42)
			},
			called
		);
	}
}
