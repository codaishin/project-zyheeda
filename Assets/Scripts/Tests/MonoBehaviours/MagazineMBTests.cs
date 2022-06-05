using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class MagazineMBTests : TestCollection
{
	class MockApplyMB : MonoBehaviour, IApplicable<Transform>
	{
		public Transform? calledApply;
		public void Apply(Transform value) => this.calledApply = value;
		public void Release(Transform _) { }
	}

	class MockMagazineMB : BaseMagazineMB<MockApplyMB> { }

	[UnityTest]
	public IEnumerator SpawnProjectile() {
		var magazine = new GameObject().AddComponent<MockMagazineMB>();
		var prefab = new GameObject().AddComponent<MockApplyMB>();
		var target = new GameObject().transform;
		var spawnPoint = new GameObject().transform;

		spawnPoint.position = Vector3.forward;

		magazine.prefab = prefab;
		magazine.spawnPoint = spawnPoint;

		yield return new WaitForEndOfFrame();

		magazine.Apply(target);

		yield return new WaitForEndOfFrame();

		var spawned = GameObject
			.FindObjectsOfType<MockApplyMB>(true)
			.Where(projectile => projectile != prefab)
			.ToArray();

		Assert.AreEqual(
			(1, Vector3.forward),
			(spawned.Length, spawned[0].transform.position)
		);
	}

	[UnityTest]
	public IEnumerator CallProjectileApply() {
		var magazine = new GameObject().AddComponent<MockMagazineMB>();
		var prefab = new GameObject().AddComponent<MockApplyMB>();
		var target = new GameObject().transform;
		var spawnPoint = new GameObject().transform;

		magazine.prefab = prefab;
		magazine.spawnPoint = spawnPoint;

		yield return new WaitForEndOfFrame();

		magazine.Apply(target);

		yield return new WaitForEndOfFrame();

		var spawned = GameObject
			.FindObjectsOfType<MockApplyMB>(true)
			.Where(projectile => projectile != prefab)
			.First();

		Assert.AreSame(target, spawned.calledApply);
	}

	[UnityTest]
	public IEnumerator CallProjectileApplyNextFrame() {
		var magazine = new GameObject().AddComponent<MockMagazineMB>();
		var prefab = new GameObject().AddComponent<MockApplyMB>();
		var target = new GameObject().transform;
		var spawnPoint = new GameObject().transform;

		magazine.prefab = prefab;
		magazine.spawnPoint = spawnPoint;

		yield return new WaitForEndOfFrame();

		magazine.Apply(target);

		var spawned = GameObject
			.FindObjectsOfType<MockApplyMB>(true)
			.Where(projectile => projectile != prefab)
			.First();

		Assert.Null(spawned.calledApply);
	}
}
