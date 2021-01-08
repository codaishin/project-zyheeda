using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BaseItemMBTests : TestCollection
{
	private class MockItemMB : BaseItemMB
	{
		public override
		IEnumerator<WaitForFixedUpdate> Apply(SkillMB skill, GameObject target)
		{
			throw new System.NotImplementedException();
		}
	}

	private class MockEffectMB : BaseEffectMB
	{
		public override void Apply(in SkillMB skill, in GameObject target)
		{
			throw new System.NotImplementedException();
		}
	}

	[Test]
	public void EffectsNotNull()
	{
		var item = new GameObject("item").AddComponent<MockItemMB>();
		Assert.NotNull(item.Effects);
	}
}
