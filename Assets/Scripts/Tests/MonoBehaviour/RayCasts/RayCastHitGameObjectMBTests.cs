using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class RayCastToColliderMapperMBTests : TestCollection
{
	private class MockRayProvider : BaseRayProviderMB
	{
		public Ray ray;
		public override Ray Ray => this.ray;
	}

	private MockRayProvider RaySource =>
		new GameObject("raySource").AddComponent<MockRayProvider>();

	[UnityTest]
	public IEnumerator RequiresRayCastHitMB() {
		var mapper = new GameObject("obj").AddComponent<RayCastHitGameObjectMB>();

		Assert.NotNull(mapper.GetComponent<RayCastHitMB>());

		yield break;
	}

	[UnityTest]
	public IEnumerator MapHitNone() {
		var mapper = new GameObject("obj").AddComponent<RayCastHitGameObjectMB>();
		mapper.GetComponent<RayCastHitMB>().raySource = this.RaySource;

		yield return new WaitForEndOfFrame();

		Assert.IsInstanceOf<None<GameObject>>(mapper.Map(Maybe.None<RaycastHit>()));
	}

	[UnityTest]
	public IEnumerator MapHitCollider() {
		var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		Physics.Raycast(new Ray(Vector3.up, Vector3.down), out RaycastHit hit);

		yield return new WaitForEndOfFrame();

		var mapper = new GameObject("obj").AddComponent<RayCastHitGameObjectMB>();
		mapper.GetComponent<RayCastHitMB>().raySource = this.RaySource;

		yield return new WaitForEndOfFrame();

		var maybe = mapper.Map(Maybe.Some(hit));

		maybe.Match(
			some: gameObject => Assert.AreSame(hit.transform.gameObject, gameObject),
			none: () => Assert.Fail("Map returned None")
		);
	}
}
