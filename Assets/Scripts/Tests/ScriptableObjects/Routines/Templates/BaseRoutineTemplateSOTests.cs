using System;
using System.Collections;
using NUnit.Framework;
using Routines;
using UnityEngine;
using UnityEngine.TestTools;

public class BaseRoutineTemplateSOTests : TestCollection
{
	class MockTemplate : ITemplate
	{
		public Func<GameObject, Func<IRoutine?>> getRoutineFn = _ => () => null;

		public Func<IRoutine?> GetRoutineFnFor(GameObject agent) =>
			this.getRoutineFn(agent);
	}

	class MockTemplateSO : BaseRoutineTemplateSO<MockTemplate> { }

	[UnityTest]
	public IEnumerator GetInstructionsArguments() {
		var called = null as GameObject;
		var templateSO = ScriptableObject.CreateInstance<MockTemplateSO>();
		var agent = new GameObject();

		templateSO.Template.getRoutineFn = agent => {
			called = agent;
			return () => null;
		};

		yield return new WaitForEndOfFrame();

		_ = templateSO.GetRoutineFnFor(agent);

		Assert.AreSame(agent, called);
	}

	[UnityTest]
	public IEnumerator GetInstructionsFunc() {
		var templateSO = ScriptableObject.CreateInstance<MockTemplateSO>();
		Func<IRoutine?> func = () => null;

		templateSO.Template.getRoutineFn = _ => func;

		yield return new WaitForEndOfFrame();

		Assert.AreSame(func, templateSO.GetRoutineFnFor(new GameObject()));
	}
}
