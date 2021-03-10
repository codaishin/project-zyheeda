using System;

[Serializable]
public class EffectData : IEffectCreator<CharacterSheetMB>
{
	public float duration;
	public EffectTag tag;
	public ConditionStacking stack;
	public BaseEffectBehaviourSO behaviour;

	public EffectTag Tag => this.tag;
	public bool StackDuration => this.stack == ConditionStacking.Duration;

	public Effect Create(CharacterSheetMB source, CharacterSheetMB target)
	{
		Effect effect = new Effect{ duration = this.duration };
		effect.OnApply += () => this.behaviour.Apply(source, target);
		effect.OnMaintain += d => this.behaviour.Maintain(source, target, d);
		effect.OnRevert += () => this.behaviour.Revert(source, target);
		return effect;
	}
}
