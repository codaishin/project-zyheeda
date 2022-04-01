using NUnit.Framework;
using UnityEngine;

public class HitSourceSOTests : TestCollection
{
	private class MockMB : MonoBehaviour { }

	[Test]
	public void TryHitHitSource() {
		var hitSource = ScriptableObject.CreateInstance<HitSourceSO>();
		var source = new GameObject().AddComponent<MockMB>();
		var getMockMB = hitSource.Try<MockMB>(source.gameObject);

		Assert.AreSame(source, getMockMB());
	}
	[Test]
	public void TryHitHitSourcePosition() {
		var hitSource = ScriptableObject.CreateInstance<HitSourceSO>();
		var source = new GameObject().AddComponent<MockMB>();
		var getPoint = hitSource.TryPoint(source.gameObject);

		Assert.AreEqual(source.transform.position, getPoint());
	}
}
