public delegate void RefAction<TSection>(ref TSection section);

public interface ISections
{
	bool UseSection<TSection>(RefAction<TSection> action);
}
