using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BaseModHpSOTests : TestCollection
{
	private class MockResistance : IResistance
	{
		private Dictionary<EffectTag, float> values = new Dictionary<EffectTag, float>();

		public float this[EffectTag tag] {
			get => this.values.TryGetValue(tag, out float v) switch {
				true => v,
				false => 0f,
			};
			set => this.values[tag] = value;
		}
	}

	private class MockSheet : ISections
	{
		public Health health;
		public MockResistance resistance = new MockResistance();

		public Action UseSection<TSection>(RefAction<TSection> action, Action fallback)
		{
			return action switch {
				RefAction<Health> use => () => use(ref this.health),
				RefAction<MockResistance> use => () => use(ref this.resistance),
				_ => fallback,
			};
		}
	}

	private class MockModHpSO : BaseModHpSO<MockResistance> { }

	[Test]
	public void Apply()
	{
		var target = new MockSheet{ health = new Health { hp = 42 } };
		var modHp = ScriptableObject.CreateInstance<MockModHpSO>();

		modHp.Apply(default, target, 4);

		Assert.AreEqual(38, target.health.hp);
	}

	[Test]
	public void Maintain()
	{
		var target = new MockSheet{ health = new Health { hp = 42 } };
		var modHp = ScriptableObject.CreateInstance<MockModHpSO>();

		modHp.Maintain(default, target, 4, 1f);

		Assert.AreEqual(38, target.health.hp);
	}

	[Test]
	public void MaintainDelta()
	{
		var target = new MockSheet{ health = new Health { hp = 42 } };
		var modHp = ScriptableObject.CreateInstance<MockModHpSO>();

		modHp.Maintain(default, target, 4, 0.5f);

		Assert.AreEqual(40, target.health.hp);
	}

	[Test]
	public void ApplyNotInverted()
	{
		var target = new MockSheet{ health = new Health { hp = 42 } };
		var modHp = ScriptableObject.CreateInstance<MockModHpSO>();

		modHp.invert = false;
		modHp.Apply(default, target, 4);

		Assert.AreEqual(46, target.health.hp);
	}

	[Test]
	public void MaintainDeltaNotInverted()
	{
		var target = new MockSheet{ health = new Health { hp = 42 } };
		var modHp = ScriptableObject.CreateInstance<MockModHpSO>();

		modHp.invert = false;
		modHp.Maintain(default, target, 4, 0.5f);

		Assert.AreEqual(44, target.health.hp);
	}

	[Test]
	public void RevertDoesNotThrow()
	{
		var modHp = ScriptableObject.CreateInstance<MockModHpSO>();

		Assert.DoesNotThrow(() => modHp.Revert<MockSheet>(default, default, default));
	}

	[Test]
	public void ApplyDamageReducedByResistance()
	{
		var target = new MockSheet();
		var modHp = ScriptableObject.CreateInstance<MockModHpSO>();

		target.health.hp = 42;
		target.resistance[EffectTag.Heat] = 0.5f;
		modHp.tag = EffectTag.Heat;
		modHp.Apply(default, target, 4);

		Assert.AreEqual(40, target.health.hp);
	}

	[Test]
	public void MaintainDamageReducedByResistance()
	{
		var target = new MockSheet();
		var modHp = ScriptableObject.CreateInstance<MockModHpSO>();

		target.health.hp = 42;
		target.resistance[EffectTag.Heat] = 0.7f;
		modHp.tag = EffectTag.Heat;
		modHp.Maintain(default, target, 5, 2f);

		Assert.AreEqual(39, target.health.hp);
	}
}
