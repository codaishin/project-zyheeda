using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class BaseItemBehaviourTests : TestCollection
{
	private class MockItemBehaviour : BaseItemBehaviour
	{
		public override
		bool Apply(BaseSkillMB skill, GameObject target, out IEnumerator<WaitForFixedUpdate> routine)
		{
			throw new System.NotImplementedException();
		}
	}

	private class MockEffectMB : BaseEffectSO
	{
		public override void Apply(in BaseSkillMB skill, in GameObject target)
		{
			throw new System.NotImplementedException();
		}
	}

	[Test]
	public void EffectsNotNull()
	{
		var behaviour = new MockItemBehaviour();

		Assert.NotNull(behaviour.effects);
	}
}
