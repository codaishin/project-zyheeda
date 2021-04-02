public interface IEffectFactory
{
	Effect Create<TSheet>(TSheet source, TSheet target, float intensity)
		where TSheet : ISections;
}
