using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Routines;
using UnityEngine;
using UnityEngine.TestTools;

public class RoutineDispatchMBTests : TestCollection
{
	class MockRoutine : IRoutine
	{
		public IEnumerator<YieldInstruction?> GetEnumerator() {
			throw new NotImplementedException();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			throw new NotImplementedException();
		}

		public bool NextSubRoutine() {
			throw new NotImplementedException();
		}
	}

	class MockTemplateSO : ScriptableObject, ITemplate
	{
		public Func<GameObject, Func<IRoutine?>> getInstructions = _ => () => null;

		public Func<IRoutine?> GetRoutineFnFor(GameObject agent) =>
			this.getInstructions(agent);
	}

	class MockRunMB : MonoBehaviour, IApplicable<(RoutineDispatchMB, Func<IRoutine?>)>
	{
		public Action<(RoutineDispatchMB, Func<IRoutine?>)> apply = _ => { };

		public void Apply((RoutineDispatchMB, Func<IRoutine?>) value) =>
			this.apply(value);
	}

	[UnityTest]
	public IEnumerator GetInstructionData() {
		var called = (null as RoutineDispatchMB, null as IRoutine);
		var template = ScriptableObject.CreateInstance<MockTemplateSO>();
		var agent = new GameObject();
		var routineDispatch = new GameObject().AddComponent<RoutineDispatchMB>();
		var runner = new GameObject().AddComponent<MockRunMB>();
		var routine = new MockRoutine();

		template.getInstructions = _ => () => routine;

		runner.apply = param => {
			var (key, getRoutine) = param;
			called = (key, getRoutine());
		};

		routineDispatch.agent = agent;
		routineDispatch.template = Reference<ITemplate>.ScriptableObject(template);
		routineDispatch.runner =
			Reference<IApplicable<(RoutineDispatchMB, Func<IRoutine?>)>>
				.Component(runner);

		yield return new WaitForEndOfFrame();

		routineDispatch.Apply();

		var (cMB, cRoutine) = called;
		Assert.AreSame(routine, cRoutine);
		Assert.AreSame(routineDispatch, cMB);
	}

	[UnityTest]
	public IEnumerator GetInstructionsNull() {
		var called = null as IRoutine;
		var template = ScriptableObject.CreateInstance<MockTemplateSO>();
		var agent = new GameObject();
		var routineDispatch = new GameObject().AddComponent<RoutineDispatchMB>();
		var runner = new GameObject().AddComponent<MockRunMB>();

		template.getInstructions = _ => () => null;

		runner.apply = param => {
			var (_, getRoutine) = param;
			called = getRoutine();
		};

		routineDispatch.agent = agent;
		routineDispatch.template = Reference<ITemplate>.ScriptableObject(template);
		routineDispatch.runner =
			Reference<IApplicable<(RoutineDispatchMB, Func<IRoutine?>)>>
				.Component(runner);

		yield return new WaitForEndOfFrame();

		routineDispatch.Apply();

		Assert.Null(called);
	}

	[UnityTest]
	public IEnumerator GetInstructionsParameters() {
		var calledAgent = null as GameObject;
		var calledRun = null as Func<bool>;
		var template = ScriptableObject.CreateInstance<MockTemplateSO>();
		var agent = new GameObject();
		var routineDispatch = new GameObject().AddComponent<RoutineDispatchMB>();
		var runner = new GameObject().AddComponent<MockRunMB>();

		template.getInstructions = agent => () => {
			calledAgent = agent;
			return new MockRoutine();
		};

		routineDispatch.agent = agent;
		routineDispatch.template = Reference<ITemplate>.ScriptableObject(template);
		routineDispatch.runner =
			Reference<IApplicable<(RoutineDispatchMB, Func<IRoutine?>)>>
				.Component(runner);

		runner.apply = param => {
			var (_, getRoutine) = param;
			_ = getRoutine();
		};

		yield return new WaitForEndOfFrame();

		routineDispatch.Apply();

		Assert.AreSame(agent, calledAgent);
	}
}
