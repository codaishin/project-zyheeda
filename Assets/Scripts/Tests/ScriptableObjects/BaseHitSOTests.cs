using NUnit.Framework;
using UnityEngine;

public class BaseHitSOTests : TestCollection
{
	private class MockHit : IHit
	{
		public bool Try<T>(T source, out T target) =>
			throw new System.NotImplementedException();
	}

	private class MockHitSO : BaseHitSO<MockHit> {}

	[Test]
	public void InitHitObject()
	{
		var mockSO = ScriptableObject.CreateInstance<MockHitSO>();

		Assert.NotNull(mockSO.hit);
	}

	[Test]
	public void NonGenericHastHit()
	{
		var mockSO = ScriptableObject.CreateInstance<MockHitSO>();

		Assert.AreSame(mockSO.hit, mockSO.Hit);
	}
}
