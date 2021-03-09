using System;

[Serializable]
public class EffectData : IEffectCreator<CharacterSheetMB>
{
	public EffectTag tag;
	public ConditionStacking stack;
	public BaseEffectBehaviourSO behaviour;

	public EffectTag Tag => this.tag;
	public bool StackDuration => this.stack == ConditionStacking.Duration;

	public Effect Create(CharacterSheetMB source, CharacterSheetMB target)
	{
		Effect effect = new Effect();
		effect.OnApply += () => behaviour.Apply(source, target);
		effect.OnMaintain += d => behaviour.Maintain(source, target, d);
		effect.OnRevert += () => behaviour.Revert(source, target);
		return effect;
	}
}
