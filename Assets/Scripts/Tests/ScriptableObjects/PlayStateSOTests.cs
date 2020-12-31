using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayStateSOTests : TestCollection
{
	[Test]
	public void OnPlayStateChange()
	{
		var called = PlayState.None;
		var stateSO = ScriptableObject.CreateInstance<PlayStateSO>();

		stateSO.OnStateChange += v => called = v;
		stateSO.State = PlayState.Paused;

		Assert.AreEqual(PlayState.Paused, called);
	}

	[Test]
	public void EmptyOnStateChange()
	{
		var stateSO = ScriptableObject.CreateInstance<PlayStateSO>();

		Assert.DoesNotThrow(() => stateSO.State = PlayState.Paused);
	}

	[Test]
	public void StatePropertyReflectsState()
	{
		var stateSO = ScriptableObject.CreateInstance<PlayStateSO>();

		stateSO.State = PlayState.Paused;

		Assert.AreEqual(PlayState.Paused, stateSO.State);
	}
}
