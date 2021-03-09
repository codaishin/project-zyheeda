public interface IEffectCreator<TSheet>
{
	EffectTag EffectTag { get; }

	bool StackDuration { get; }
	Effect Create(TSheet source, TSheet target);
}
