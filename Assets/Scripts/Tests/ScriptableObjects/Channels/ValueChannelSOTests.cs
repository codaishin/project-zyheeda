using NUnit.Framework;
using UnityEngine;

public class ValueChannelSOTests : TestCollection
{
	private class MockChannelSO : ValueChannelSO<int> { }

	[Test]
	public void RegisterEvent() {
		var called = 0;
		var channelSO = ScriptableObject.CreateInstance<MockChannelSO>();

		channelSO.Listeners += v => called = v;
		channelSO.Raise(42);

		Assert.AreEqual(42, called);
	}

	[Test]
	public void RaiseEmptyNoException() {
		var channelSO = ScriptableObject.CreateInstance<MockChannelSO>();
		Assert.DoesNotThrow(() => channelSO.Raise(42));
	}
}
