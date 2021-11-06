using NUnit.Framework;
using UnityEngine;

public class BaseHitSOTests : TestCollection
{
	private class MockHit : IHit
	{
		public Maybe<T> Try<T>(T source) where T : notnull =>
			throw new System.NotImplementedException();
	}

	private class MockHitSO : BaseHitSO<MockHit> { }

	[Test]
	public void InitHitObject() {
		var mockSO = ScriptableObject.CreateInstance<MockHitSO>();

		Assert.NotNull(mockSO.hit);
	}

	[Test]
	public void NonGenericHastHit() {
		var mockSO = ScriptableObject.CreateInstance<MockHitSO>();

		Assert.AreSame(mockSO.hit, mockSO.Hit);
	}
}
