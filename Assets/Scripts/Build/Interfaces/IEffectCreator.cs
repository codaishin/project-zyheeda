public interface IEffectCreator<TSheet>
{
	Effect Create(TSheet source, TSheet target);
}
