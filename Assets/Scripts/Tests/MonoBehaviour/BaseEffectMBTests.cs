using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BaseEffectMBTests : TestCollection
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

	}

	[Test]
	public void ExposeItemMB()
	{
		var item = new GameObject("item").AddComponent<MockItemMB>();
		var effect = item.gameObject.AddComponent<MockEffectMB>();

		Assert.AreSame(item, effect.Item);
	}
}
