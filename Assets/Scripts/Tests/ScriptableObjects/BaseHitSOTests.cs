using NUnit.Framework;
using UnityEngine;

public class BaseHitSOTests : TestCollection
{
	private class MockHit : IHit
	{
		public bool TryHit<TTarget>(out TTarget target) => throw new System.NotImplementedException();
	}

	private class MockHitSO : BaseHitSO<MockHit> {}

	[Test]
	public void InitHitObject()
	{
		var mockSO = ScriptableObject.CreateInstance<MockHitSO>();

		Assert.NotNull(mockSO.hit);
	}
}
