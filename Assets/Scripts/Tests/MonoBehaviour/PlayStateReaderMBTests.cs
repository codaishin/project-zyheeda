using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayStateReaderMBTests : TestCollection
{
	[UnityTest]
	public IEnumerator OnStateEnterNotNullAfterStart()
	{
		var reader = new GameObject("reader").AddComponent<PlayStateReaderMB>();
		var stateSwitch = ScriptableObject.CreateInstance<PlayStateSwitchSO>();
		reader.stateSwitch = stateSwitch;

		yield return new WaitForEndOfFrame();

		Assert.NotNull(reader.onStateEnter);
	}

	[UnityTest]
	public IEnumerator OnStateEnterDefaultNull()
	{
		var reader = new GameObject("reader").AddComponent<PlayStateReaderMB>();
		var stateSwitch = ScriptableObject.CreateInstance<PlayStateSwitchSO>();
		reader.stateSwitch = stateSwitch;

		Assert.Null(reader.onStateEnter);

		yield break;
	}

	[UnityTest]
	public IEnumerator OnStateEnter()
	{
		var called = 0;
		var reader = new GameObject("reader").AddComponent<PlayStateReaderMB>();
		var stateSwitch = ScriptableObject.CreateInstance<PlayStateSwitchSO>();

		reader.stateSwitch = stateSwitch;
		reader.state = PlayState.Paused;

		yield return new WaitForEndOfFrame();

		reader.onStateEnter.AddListener(() => ++called);
		stateSwitch.State = PlayState.Paused;

		Assert.AreEqual(1, called);
	}

	[UnityTest]
	public IEnumerator OnStateEnterUnsubscribeWhenDestroyed()
	{
		var called = 0;
		var reader = new GameObject("reader").AddComponent<PlayStateReaderMB>();
		var stateSwitch = ScriptableObject.CreateInstance<PlayStateSwitchSO>();

		reader.stateSwitch = stateSwitch;
		reader.state = PlayState.Paused;

		yield return new WaitForEndOfFrame();

		reader.onStateEnter.AddListener(() => ++called);
		Object.Destroy(reader);

		yield return new WaitForEndOfFrame();

		stateSwitch.State = PlayState.Paused;

		Assert.AreEqual(0, called);
	}
}
