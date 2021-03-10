using System;

[Serializable]
public class EffectData : IEffectCreator<CharacterSheetMB>
{
	public int intensity;
	public float duration;
	public EffectTag tag;
	public ConditionStacking stack;
	public BaseEffectBehaviourSO behaviour;

	public EffectTag Tag => this.tag;
	public bool StackDuration => this.stack == ConditionStacking.Duration;

	public Effect Create(CharacterSheetMB source, CharacterSheetMB target)
	{
		Effect effect = new Effect{ duration = this.duration };
		effect.OnApply += () => this.behaviour.Apply(source, target, this.intensity);
		effect.OnMaintain += d => this.behaviour.Maintain(source, target, this.intensity, d);
		effect.OnRevert += () => this.behaviour.Revert(source, target, this.intensity);
		return effect;
	}
}
