using System;
using System.Collections;
using NUnit.Framework;
using Routines;
using UnityEngine;
using UnityEngine.TestTools;

public class BaseRoutineFuncFactorySOTests : TestCollection
{
	class MockFuncFactory : IFuncFactory
	{
		public Func<GameObject, Func<IRoutine?>> getRoutineFn = _ => () => null;

		public Func<IRoutine?> GetRoutineFnFor(GameObject agent) =>
			this.getRoutineFn(agent);
	}

	class MockFactorySO : BaseRoutineFuncFactorySO<MockFuncFactory> { }

	[UnityTest]
	public IEnumerator GetInstructionsArguments() {
		var called = null as GameObject;
		var factorySO = ScriptableObject.CreateInstance<MockFactorySO>();
		var agent = new GameObject();

		factorySO.Factory.getRoutineFn = agent => {
			called = agent;
			return () => null;
		};

		yield return new WaitForEndOfFrame();

		_ = factorySO.GetRoutineFnFor(agent);

		Assert.AreSame(agent, called);
	}

	[UnityTest]
	public IEnumerator GetInstructionsFunc() {
		var factorySO = ScriptableObject.CreateInstance<MockFactorySO>();
		Func<IRoutine?> func = () => null;

		factorySO.Factory.getRoutineFn = _ => func;

		yield return new WaitForEndOfFrame();

		Assert.AreSame(func, factorySO.GetRoutineFnFor(new GameObject()));
	}
}
