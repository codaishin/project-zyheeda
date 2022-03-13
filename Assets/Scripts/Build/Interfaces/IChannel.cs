using System;

public interface IChannel
{
	void AddListener(Action action);
	void RemoveListener(Action action);
}
