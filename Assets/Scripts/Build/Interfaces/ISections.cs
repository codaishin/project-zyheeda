using System;

public interface ISections
{
	Action UseSection<TSection>(RefAction<TSection> action, Action fallback);
}
