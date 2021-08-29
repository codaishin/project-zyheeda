using System.Collections.Generic;
using UnityEngine;

public class SkillMB<TSheet, TEffectCollection, TCast> :
	BaseSkillMB<TSheet>
	where TEffectCollection :
		IEffectCollection<TSheet>,
		new()
	where TCast :
		ICast<TSheet>,
		new()
{
	public TEffectCollection effectCollection = new TEffectCollection();
	public TCast cast = new TCast();

	protected override void ApplyEffects(TSheet source, TSheet target) {
		this.effectCollection.Apply(source, target);
	}

	protected override IEnumerator<WaitForFixedUpdate> ApplyCast(TSheet target) {
		return this.cast.Apply(target);
	}
}
