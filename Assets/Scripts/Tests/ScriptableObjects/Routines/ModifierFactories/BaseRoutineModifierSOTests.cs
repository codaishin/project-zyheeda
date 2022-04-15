using System;
using System.Collections;
using NUnit.Framework;
using Routines;
using UnityEngine;
using UnityEngine.TestTools;

public class BaseRoutineModifierSOTests : TestCollection
{
	class MockModifier : IModifierFactory
	{
		public Func<GameObject, ModifierFn> getModifierFn =
			_ => _ => (null as Action, null as Action, null as Action);

		public ModifierFn GetModifierFnFor(GameObject agent) =>
			this.getModifierFn(agent);
	}

	class MockModifierSO : BaseRoutineModifierSO<MockModifier>
	{
		public MockModifier Modifier => this.modifier;
	}

	[UnityTest]
	public IEnumerator UseWrappedCallback() {
		var modifierSO = ScriptableObject.CreateInstance<MockModifierSO>();
		var agent = new GameObject();
		var calledAgent = null as GameObject;

		ModifierFn modifierFn = _ => (
			null as Action,
			null as Action,
			null as Action
		);
		ModifierFn getModifierFn(GameObject agent) {
			calledAgent = agent;
			return modifierFn;
		};

		modifierSO.Modifier.getModifierFn = getModifierFn;

		yield return new WaitForEndOfFrame();

		var calledModifierFn = modifierSO.GetModifierFnFor(agent);

		Assert.AreSame(modifierFn, calledModifierFn);
		Assert.AreSame(agent, calledAgent);
	}
}
