public interface IStoppable
{
	void Stop(int softStopAttempts);
}

public interface IStoppable<T>
{
	void Stop(T key, int softStopAttempts);
}
