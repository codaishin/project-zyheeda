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
	public IEnumerator OnStateEnterOnStart()
	{
		var called = 0;
		var reader = new GameObject("reader").AddComponent<PlayStateReaderMB>();
		var stateSwitch = ScriptableObject.CreateInstance<PlayStateSwitchSO>();

		stateSwitch.State = PlayState.Paused;
		reader.stateSwitch = stateSwitch;
		reader.state = PlayState.Paused;
		reader.onStateEnter = new UnityEngine.Events.UnityEvent();
		reader.onStateEnter.AddListener(() => ++called);

		yield return new WaitForEndOfFrame();

		Assert.AreEqual(1, called);
	}

	[UnityTest]
	public IEnumerator OnStateEnterOnlyOnRealEnter()
	{
		var called = 0;
		var reader = new GameObject("reader").AddComponent<PlayStateReaderMB>();
		var stateSwitch = ScriptableObject.CreateInstance<PlayStateSwitchSO>();

		reader.stateSwitch = stateSwitch;
		reader.state = PlayState.Paused;

		yield return new WaitForEndOfFrame();

		reader.onStateEnter.AddListener(() => ++called);
		stateSwitch.State = PlayState.Paused;
		stateSwitch.State = PlayState.Menu;

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

	[UnityTest]
	public IEnumerator OnStateExitNotNullAfterStart()
	{
		var reader = new GameObject("reader").AddComponent<PlayStateReaderMB>();
		var stateSwitch = ScriptableObject.CreateInstance<PlayStateSwitchSO>();
		reader.stateSwitch = stateSwitch;

		yield return new WaitForEndOfFrame();

		Assert.NotNull(reader.onStateExit);
	}

	[UnityTest]
	public IEnumerator OnStateExitDefaultNull()
	{
		var reader = new GameObject("reader").AddComponent<PlayStateReaderMB>();
		var stateSwitch = ScriptableObject.CreateInstance<PlayStateSwitchSO>();
		reader.stateSwitch = stateSwitch;

		Assert.Null(reader.onStateExit);

		yield break;
	}

	[UnityTest]
	public IEnumerator OnStateExit()
	{
		var called = 0;
		var reader = new GameObject("reader").AddComponent<PlayStateReaderMB>();
		var stateSwitch = ScriptableObject.CreateInstance<PlayStateSwitchSO>();

		stateSwitch.State = PlayState.Paused;
		reader.stateSwitch = stateSwitch;
		reader.state = PlayState.Paused;

		yield return new WaitForEndOfFrame();

		reader.onStateExit.AddListener(() => ++called);
		stateSwitch.State = PlayState.Play;

		Assert.AreEqual(1, called);
	}

	[UnityTest]
	public IEnumerator OnStateExitOnStart()
	{
		var called = 0;
		var reader = new GameObject("reader").AddComponent<PlayStateReaderMB>();
		var stateSwitch = ScriptableObject.CreateInstance<PlayStateSwitchSO>();

		stateSwitch.State = PlayState.Play;
		reader.stateSwitch = stateSwitch;
		reader.state = PlayState.Paused;
		reader.onStateExit = new UnityEngine.Events.UnityEvent();
		reader.onStateExit.AddListener(() => ++called);

		yield return new WaitForEndOfFrame();

		Assert.AreEqual(1, called);
	}

	[UnityTest]
	public IEnumerator OnStateExitNotWhenNotExiting()
	{
		var called = 0;
		var reader = new GameObject("reader").AddComponent<PlayStateReaderMB>();
		var stateSwitch = ScriptableObject.CreateInstance<PlayStateSwitchSO>();

		reader.stateSwitch = stateSwitch;
		reader.state = PlayState.Paused;

		yield return new WaitForEndOfFrame();

		reader.onStateExit.AddListener(() => ++called);
		stateSwitch.State = PlayState.Paused;

		Assert.AreEqual(0, called);
	}

	[UnityTest]
	public IEnumerator OnStateExitOnlyOnRealExit()
	{
		var called = 0;
		var reader = new GameObject("reader").AddComponent<PlayStateReaderMB>();
		var stateSwitch = ScriptableObject.CreateInstance<PlayStateSwitchSO>();

		stateSwitch.State = PlayState.Paused;
		reader.stateSwitch = stateSwitch;
		reader.state = PlayState.Paused;

		yield return new WaitForEndOfFrame();

		reader.onStateExit.AddListener(() => ++called);
		stateSwitch.State = PlayState.Play;
		stateSwitch.State = PlayState.Menu;

		Assert.AreEqual(1, called);
	}

	[UnityTest]
	public IEnumerator OnStateExitUnsubscribeWhenDestroyed()
	{
		var called = 0;
		var reader = new GameObject("reader").AddComponent<PlayStateReaderMB>();
		var stateSwitch = ScriptableObject.CreateInstance<PlayStateSwitchSO>();

		stateSwitch.State = PlayState.Paused;
		reader.stateSwitch = stateSwitch;
		reader.state = PlayState.Paused;

		yield return new WaitForEndOfFrame();

		reader.onStateExit.AddListener(() => ++called);

		Object.Destroy(reader);

		yield return new WaitForEndOfFrame();

		stateSwitch.State = PlayState.Play;

		Assert.AreEqual(0, called);
	}
}
