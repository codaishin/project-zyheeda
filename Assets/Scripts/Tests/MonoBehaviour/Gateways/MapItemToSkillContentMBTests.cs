using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class MapItemToSkillContentMBTests : TestCollection
{
	class MockSkill : BaseSkillMB<CharacterSheetMB>
	{
		protected override IEnumerator<WaitForFixedUpdate> ApplyCast(
			CharacterSheetMB target
		) {
			throw new NotImplementedException();
		}
		protected override void ApplyEffects(
			CharacterSheetMB source,
			CharacterSheetMB target
		) {
			throw new NotImplementedException();
		}
	}

	[UnityTest]
	public IEnumerator Map() {
		var gateway = new GameObject("gateway").AddComponent<MapItemToSkillContentMB>();
		var item = new GameObject("item").AddComponent<ItemMB>();
		var skills = new BaseSkillMB<CharacterSheetMB>[] {
			item.gameObject.AddComponent<MockSkill>(),
			item.gameObject.AddComponent<MockSkill>(),
		};

		yield return new WaitForEndOfFrame();

		CollectionAssert.AreEqual(skills, gateway.MapValueToContent(item));
	}
}
