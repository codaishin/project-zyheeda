public interface IPausable
{
	bool Paused { get; set; }
}

public interface IPausable<TPauseYield> : IPausable
{
	TPauseYield Pause { get; }
}
