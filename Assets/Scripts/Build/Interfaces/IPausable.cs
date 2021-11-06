public interface IPausable
{
	bool Paused { get; set; }
}

public interface IPausable<TPauseYield> :
	IPausable
	where TPauseYield : notnull
{
	TPauseYield Pause { get; }
}
