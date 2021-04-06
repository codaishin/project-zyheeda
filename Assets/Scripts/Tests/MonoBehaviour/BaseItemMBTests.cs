using System.Linq;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BaseItemMBTests : TestCollection
{
	private class MockSheet {}

	private class MockSkillMB : MonoBehaviour, ISkill<MockSheet>
	{
		public MockSheet Sheet { get; set; }
		public void Begin(MockSheet target) => throw new System.NotImplementedException();
	}

	private class MockItem : BaseItemMB<MockSheet> {}

	[UnityTest]
	public IEnumerator SetSkillSheet()
	{
		var sheet = new MockSheet();
		var item = new GameObject("item").AddComponent<MockItem>();
		var skills = new MockSkillMB[] {
			item.gameObject.AddComponent<MockSkillMB>(),
			item.gameObject.AddComponent<MockSkillMB>(),
			item.gameObject.AddComponent<MockSkillMB>(),
		};

		yield return new WaitForEndOfFrame();

		item.Sheet = sheet;

		CollectionAssert.AreEqual(
			new MockSheet[] { sheet, sheet, sheet },
			skills.Select(s => s.Sheet)
		);
	}

	[UnityTest]
	public IEnumerator GetSheet()
	{
		var sheet = new MockSheet();
		var item = new GameObject("item").AddComponent<MockItem>();

		yield return new WaitForFixedUpdate();

		item.Sheet = sheet;

		Assert.AreSame(sheet, item.Sheet);
	}

	[UnityTest]
	public IEnumerator SetSheetBeforeStartNoThrow()
	{
		var item = new GameObject("item").AddComponent<MockItem>();

		Assert.DoesNotThrow(() => item.Sheet = new MockSheet());

		yield break;
	}

	[UnityTest]
	public IEnumerator SetSheetFromBeforeStart()
	{
		var sheet = new MockSheet();
		var item = new GameObject("item").AddComponent<MockItem>();

		item.Sheet = sheet;

		var skills = new MockSkillMB[] {
			item.gameObject.AddComponent<MockSkillMB>(),
			item.gameObject.AddComponent<MockSkillMB>(),
			item.gameObject.AddComponent<MockSkillMB>(),
		};

		yield return new WaitForFixedUpdate();

		CollectionAssert.AreEqual(
			new MockSheet[] { sheet, sheet, sheet },
			skills.Select(s => s.Sheet)
		);
	}
}
