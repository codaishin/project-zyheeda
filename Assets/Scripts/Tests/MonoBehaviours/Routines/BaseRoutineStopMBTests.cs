using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BaseRoutineStopMBTests : TestCollection
{
	class MockRoutineRunnerMB : MonoBehaviour, IStoppable<float>
	{
		public Action<float, int> stop = (_, __) => { };

		public void Stop(float value, int softStopAttempts) =>
			this.stop(value, softStopAttempts);
	}

	class MockRoutineStopMB : BaseRoutineStopMB<float> { }

	[UnityTest]
	public IEnumerator StopRoutine() {
		var keys = new[] { 1f, 2f, 3f };
		var stopController = new GameObject().AddComponent<MockRoutineStopMB>();
		var runner = new GameObject().AddComponent<MockRoutineRunnerMB>();
		var called = new List<(float, int)>();

		stopController.stopper = Reference<IStoppable<float>>.Component(runner);
		stopController.keys = new[] { 1f, 2f, 3f };

		runner.stop = (source, softStopAttempts) =>
			called.Add((source, softStopAttempts));

		yield return new WaitForEndOfFrame();

		stopController.Apply();

		CollectionAssert.AreEqual(new[] { (1f, 0), (2f, 0), (3f, 0) }, called);
	}

	[UnityTest]
	public IEnumerator StopRoutineWithSoftStopAttempts() {
		var keys = new[] { 1f, 2f, 3f };
		var stop = new GameObject().AddComponent<MockRoutineStopMB>();
		var runner = new GameObject().AddComponent<MockRoutineRunnerMB>();
		var called = new List<(float, int)>();

		stop.stopper = Reference<IStoppable<float>>.Component(runner);
		stop.keys = new[] { 1f, 2f, 3f };
		stop.softStopAttempts = 42;

		runner.stop = (source, softStopAttempts) =>
			called.Add((source, softStopAttempts));

		yield return new WaitForEndOfFrame();

		stop.Apply();

		CollectionAssert.AreEqual(new[] { (1f, 42), (2f, 42), (3f, 42) }, called);
	}
}
