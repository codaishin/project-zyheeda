using NUnit.Framework;
using UnityEngine;

public class HitSourceSOTests : TestCollection
{
	private class MockMB : MonoBehaviour { }

	[Test]
	public void TryHitHitSource() {
		var hitSource = ScriptableObject.CreateInstance<HitSourceSO>();
		var source = new GameObject().AddComponent<MockMB>();

		Assert.AreSame(source, hitSource.Try(source));
	}
	[Test]
	public void TryHitHitSourcePosition() {
		var hitSource = ScriptableObject.CreateInstance<HitSourceSO>();
		var source = new GameObject().AddComponent<MockMB>();

		Assert.AreEqual(
			source.transform.position,
			hitSource.TryPoint(source.transform)
		);
	}
}
