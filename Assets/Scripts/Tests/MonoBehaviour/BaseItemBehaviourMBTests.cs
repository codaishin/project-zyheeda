using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BaseItemMBTests : TestCollection
{
	private class MockItemMB : BaseItemBehaviourMB
	{
		public override
		bool Apply(SkillMB skill, GameObject target, out IEnumerator<WaitForFixedUpdate> routine)
		{
			throw new System.NotImplementedException();
		}
	}

	private class MockEffectMB : BaseEffectSO
	{
		public override void Apply(in SkillMB skill, in GameObject target)
		{
			throw new System.NotImplementedException();
		}
	}

	[UnityTest]
	public IEnumerator EffectsNotNull()
	{
		var item = new GameObject("item").AddComponent<MockItemMB>();

		yield return new WaitForEndOfFrame();

		Assert.NotNull(item.effects);
	}
}
