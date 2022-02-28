using NUnit.Framework;
using UnityEngine;

public class HitSourceSOTests : TestCollection
{
	private class MockMB : MonoBehaviour { }

	[Test]
	public void TryHitHitSource() {
		var hitSource = ScriptableObject.CreateInstance<HitSourceSO>();
		var source = new GameObject().AddComponent<MockMB>();

		hitSource.Try(source).Match(
			some: hit => Assert.AreSame(source, hit),
			none: () => Assert.Fail("No hit")
		);
	}
	[Test]
	public void TryHitHitSourcePosition() {
		var hitSource = ScriptableObject.CreateInstance<HitSourceSO>();
		var source = new GameObject().AddComponent<MockMB>();

		hitSource.TryPoint(source.transform).Match(
			some: hit => Assert.AreEqual(source.transform.position, hit),
			none: () => Assert.Fail("No hit")
		);
	}
}
