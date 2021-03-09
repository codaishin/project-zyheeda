public class EffectData : IEffectCreator
{
	public EffectTag effectTag;
	public BaseEffectBehaviourSO effectBehaviour;

	public EffectTag EffectTag => this.effectTag;

	public Effect Create(CharacterSheetMB source, CharacterSheetMB target)
	{
		Effect effect = new Effect();
		effect.OnApply += () => effectBehaviour.Apply(source, target);
		effect.OnMaintain += d => effectBehaviour.Maintain(source, target, d);
		effect.OnRevert += () => effectBehaviour.Revert(source, target);
		return effect;
	}
}
