using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class MapEquipmentToItemContentMBTests : TestCollection
{
	[UnityTest]
	public IEnumerator Map() {
		var gateway = new GameObject().AddComponent<MapEquipmentToItemContentMB>();
		var equipment = new Equipment();
		equipment[
			EquipmentSlot.MainHand
		] = new GameObject("main item").AddComponent<ItemMB>();
		equipment[
			EquipmentSlot.OffHand
		] = new GameObject("off item").AddComponent<ItemMB>();

		yield return new WaitForEndOfFrame();

		CollectionAssert.AreEqual(
			equipment.Select(r => r.value),
			gateway.MapValueToContent(equipment)
		);
	}
}
