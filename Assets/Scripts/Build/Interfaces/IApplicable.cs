public interface IApplicable
{
	void Apply();
	void Release();
}

public interface IApplicable<T>
{
	void Apply(T value);
	void Release(T value);
}
