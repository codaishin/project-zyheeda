public interface IApplicable
{
	void Apply();
}

public interface IApplicable<T>
{
	void Apply(T value);
}
