using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ModHpSOTests : TestCollection
{
	private class MockSheet : ISections
	{
		public Health health;

		public bool UseSection<TSection>(RefAction<TSection> action)
		{
			bool useHealth(RefAction<Health> call) {
				call(ref this.health);
				return true;
			}

			return action switch {
				RefAction<Health> call => useHealth(call),
				_ => false,
			};
		}
	}

	[Test]
	public void Apply()
	{
		var target = new MockSheet{ health = new Health { hp = 42 } };
		var modHp = ScriptableObject.CreateInstance<ModHpSO>();

		modHp.Apply(default, target, 4);

		Assert.AreEqual(38, target.health.hp);
	}

	[Test]
	public void Maintain()
	{
		var target = new MockSheet{ health = new Health { hp = 42 } };
		var modHp = ScriptableObject.CreateInstance<ModHpSO>();

		modHp.Maintain(default, target, 4, 1f);

		Assert.AreEqual(38, target.health.hp);
	}

	[Test]
	public void MaintainDelta()
	{
		var target = new MockSheet{ health = new Health { hp = 42 } };
		var modHp = ScriptableObject.CreateInstance<ModHpSO>();

		modHp.Maintain(default, target, 4, 0.5f);

		Assert.AreEqual(40, target.health.hp);
	}

	[Test]
	public void ApplyNotInverted()
	{
		var target = new MockSheet{ health = new Health { hp = 42 } };
		var modHp = ScriptableObject.CreateInstance<ModHpSO>();

		modHp.invert = false;
		modHp.Apply(default, target, 4);

		Assert.AreEqual(46, target.health.hp);
	}

	[Test]
	public void MaintainDeltaNotInverted()
	{
		var target = new MockSheet{ health = new Health { hp = 42 } };
		var modHp = ScriptableObject.CreateInstance<ModHpSO>();

		modHp.invert = false;
		modHp.Maintain(default, target, 4, 0.5f);

		Assert.AreEqual(44, target.health.hp);
	}

	[Test]
	public void RevertDoesNotThrow()
	{
		var modHp = ScriptableObject.CreateInstance<ModHpSO>();

		Assert.DoesNotThrow(() => modHp.Revert<MockSheet>(default, default, default));
	}
}
