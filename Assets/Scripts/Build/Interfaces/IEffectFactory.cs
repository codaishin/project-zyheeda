public interface IEffectFactory<TSheet>
	where TSheet : ISections
{
	Effect Create(TSheet source, TSheet target, float intensity);
}
