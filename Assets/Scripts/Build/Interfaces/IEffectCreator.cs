public interface IEffectCreator<TSheet>
{
	EffectTag Tag { get; }

	bool StackDuration { get; }
	Effect Create(TSheet source, TSheet target);
}
