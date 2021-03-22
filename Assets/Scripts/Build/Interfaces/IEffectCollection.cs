public interface IEffectCollection<TSheet>
{
	void Apply(TSheet source, TSheet target);
}
