using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class RayCastHitPointMBTests : TestCollection
{
	private class MockRayProvider : BaseRayProviderMB
	{
		public Ray ray;
		public override Ray Ray => this.ray;
	}

	private MockRayProvider RaySource =>
		new GameObject("raySource").AddComponent<MockRayProvider>();

	public IEnumerator RequiresRayCastHitMB() {
		var mapper = new GameObject("obj").AddComponent<RayCastHitPointMB>();

		Assert.NotNull(mapper.GetComponent<RayCastHitMB>());

		yield break;
	}

	[UnityTest]
	public IEnumerator MapHitNone() {
		var mapper = new GameObject("obj").AddComponent<RayCastHitPointMB>();
		mapper.GetComponent<RayCastHitMB>().raySource = this.RaySource;

		yield return new WaitForEndOfFrame();

		Assert.IsInstanceOf<None<Vector3>>(mapper.Map(Maybe.None<RaycastHit>()));
	}

	[UnityTest]
	public IEnumerator MapHitCollider() {
		var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		Physics.Raycast(new Ray(Vector3.up, Vector3.down), out RaycastHit hit);

		yield return new WaitForEndOfFrame();

		var mapper = new GameObject("obj").AddComponent<RayCastHitPointMB>();
		mapper.GetComponent<RayCastHitMB>().raySource = this.RaySource;

		yield return new WaitForEndOfFrame();

		var maybe = mapper.Map(Maybe.Some(hit));

		maybe.Match(
			some: point => Assert.AreEqual(hit.point, point),
			none: () => Assert.Fail("Map returned None")
		);
	}
}
