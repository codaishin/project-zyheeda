using System;
using System.Collections;
using NUnit.Framework;
using Routines;
using UnityEngine;
using UnityEngine.TestTools;

public class BaseRoutineTemplateMBTests : TestCollection
{
	class MockFactory : ITemplate
	{
		public Func<GameObject, Func<IRoutine?>> getInstructionsFor =
			_ => () => null;

		public Func<IRoutine?> GetRoutineFnFor(GameObject agent) =>
			this.getInstructionsFor(agent);
	}

	class MockFactoryMB : BaseRoutineTemplateMB<MockFactory> { }

	[UnityTest]
	public IEnumerator GetInstructionsArguments() {
		var called = null as GameObject;
		var factoryMB = new GameObject().AddComponent<MockFactoryMB>();
		var agent = new GameObject();

		factoryMB.Template.getInstructionsFor = agent => {
			called = agent;
			return () => null;
		};

		yield return new WaitForEndOfFrame();

		_ = factoryMB.GetRoutineFnFor(agent);

		Assert.AreEqual(agent, called);
	}

	[UnityTest]
	public IEnumerator GetInstructionsFunc() {
		var factoryMB = new GameObject().AddComponent<MockFactoryMB>();
		Func<IRoutine?> func = () => null;

		factoryMB.Template.getInstructionsFor = _ => func;

		yield return new WaitForEndOfFrame();

		Assert.AreSame(func, factoryMB.GetRoutineFnFor(new GameObject()));
	}
}
