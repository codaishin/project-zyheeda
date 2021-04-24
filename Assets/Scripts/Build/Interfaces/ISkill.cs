public interface ISkill<TSheet>
{
	TSheet Sheet { get; set; }
	void Begin();
}
