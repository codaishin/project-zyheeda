public delegate void SectionAction<TSection>(ref TSection section);

public interface ISections
{
	void UseSection<TSection>(SectionAction<TSection> action, bool required = false);
}
