public interface IEffectData
{
	Effect GetEffect<TSheet>(TSheet source, TSheet target)
		where TSheet : ISections;
}
