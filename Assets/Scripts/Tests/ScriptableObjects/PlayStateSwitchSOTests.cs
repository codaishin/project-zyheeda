using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayStateSwitchSOTests : TestCollection
{
	[Test]
	public void OnPlayStateChange()
	{
		var called = PlayState.None;
		var stateSO = ScriptableObject.CreateInstance<PlayStateSwitchSO>();

		stateSO.OnStateChange += v => called = v;
		stateSO.State = PlayState.Paused;

		Assert.AreEqual(PlayState.Paused, called);
	}

	[Test]
	public void EmptyOnStateChange()
	{
		var stateSO = ScriptableObject.CreateInstance<PlayStateSwitchSO>();

		Assert.DoesNotThrow(() => stateSO.State = PlayState.Paused);
	}

	[Test]
	public void StatePropertyReflectsState()
	{
		var stateSO = ScriptableObject.CreateInstance<PlayStateSwitchSO>();

		stateSO.State = PlayState.Paused;

		Assert.AreEqual(PlayState.Paused, stateSO.State);
	}

	[Test]
	public void StateStateViaStateObject()
	{
		var stateSO = ScriptableObject.CreateInstance<PlayStateSwitchSO>();
		var stateValue = ScriptableObject.CreateInstance<PlayStateValueSO>();

		stateValue.state = PlayState.Play;
		stateSO.SetState(stateValue);

		Assert.AreEqual(PlayState.Play, stateSO.State);
	}

	[Test]
	public void InvokeOnStateChangeViaStateObject()
	{
		var called = PlayState.None;
		var stateSO = ScriptableObject.CreateInstance<PlayStateSwitchSO>();
		var stateValue = ScriptableObject.CreateInstance<PlayStateValueSO>();

		stateValue.state = PlayState.Play;
		stateSO.OnStateChange += v => called = v;
		stateSO.SetState(stateValue);

		Assert.AreEqual(PlayState.Play, called);
	}

	[Test]
	public void OnPlayStateChangeOnlyCalledOnActualChange()
	{
		var called = 0;
		var stateSO = ScriptableObject.CreateInstance<PlayStateSwitchSO>();

		stateSO.State = PlayState.Paused;
		stateSO.OnStateChange += _ => ++called;
		stateSO.State = PlayState.Paused;

		Assert.AreEqual(0, called);
	}
}
