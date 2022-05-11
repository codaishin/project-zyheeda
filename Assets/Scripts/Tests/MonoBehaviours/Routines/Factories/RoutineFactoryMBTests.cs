using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Routines;
using UnityEngine;
using UnityEngine.TestTools;

public class RoutineFactoryMBTests : TestCollection
{
	class MockRoutine : IRoutine
	{
		public IEnumerator<YieldInstruction?> GetEnumerator() {
			throw new NotImplementedException();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			throw new NotImplementedException();
		}

		public void NextSubRoutine() {
			throw new NotImplementedException();
		}
	}

	class MockFactorySO : ScriptableObject, IFuncFactory
	{
		public Func<GameObject, Func<IRoutine?>> getInstructions = _ => () => null;

		public Func<IRoutine?> GetRoutineFnFor(GameObject agent) =>
			this.getInstructions(agent);
	}

	[UnityTest]
	public IEnumerator GetInstructionData() {
		var agent = new GameObject();
		var instructions = new GameObject().AddComponent<RoutineFactoryMB>();
		var factory = ScriptableObject.CreateInstance<MockFactorySO>();
		var instructionData = new MockRoutine();

		instructions.routineFuncFactory =
			Reference<IFuncFactory>.ScriptableObject(factory);
		instructions.agent =
			agent;
		factory.getInstructions =
			_ => () => instructionData;

		yield return new WaitForEndOfFrame();

		Assert.AreSame(instructionData, instructions.GetRoutine());
	}

	[UnityTest]
	public IEnumerator GetInstructionsNull() {
		var agent = new GameObject();
		var instructions = new GameObject().AddComponent<RoutineFactoryMB>();
		var factory = ScriptableObject.CreateInstance<MockFactorySO>();

		instructions.routineFuncFactory =
			Reference<IFuncFactory>.ScriptableObject(factory);
		instructions.agent =
			agent;

		factory.getInstructions = _ => () => null;

		yield return new WaitForEndOfFrame();

		Assert.Null(instructions.GetRoutine());
	}

	[UnityTest]
	public IEnumerator GetInstructionsParameters() {
		var calledAgent = null as GameObject;
		var calledRun = null as Func<bool>;
		var agent = new GameObject();
		var instructions = new GameObject().AddComponent<RoutineFactoryMB>();
		var factory = ScriptableObject.CreateInstance<MockFactorySO>();

		instructions.routineFuncFactory =
			Reference<IFuncFactory>.ScriptableObject(factory);
		instructions.agent =
			agent;

		factory.getInstructions = agent => () => {
			calledAgent = agent;
			return new MockRoutine();
		};

		yield return new WaitForEndOfFrame();

		_ = instructions.GetRoutine();

		Assert.AreSame(agent, calledAgent);
	}
}
