using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ChannelSOTests : TestCollection
{
	[Test]
	public void RegisterEvent() {
		var called = 0;
		var channelSO = ScriptableObject.CreateInstance<ChannelSO>();

		channelSO.Listeners += () => ++called;
		channelSO.Raise();

		Assert.AreEqual(1, called);
	}

	[Test]
	public void RaiseEmptyNoException() {
		var channelSO = ScriptableObject.CreateInstance<ChannelSO>();
		Assert.DoesNotThrow(() => channelSO.Raise());
	}
}
