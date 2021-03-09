public interface IEffectCreator<TSheet>
{
	EffectTag EffectTag { get; }
	Effect Create(TSheet source, TSheet target);
}
