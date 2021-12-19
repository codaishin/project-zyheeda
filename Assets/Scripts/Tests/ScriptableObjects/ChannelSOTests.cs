using NUnit.Framework;
using UnityEngine;

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
