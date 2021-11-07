using System;

public class WasNullException : Exception
{
	public WasNullException(object on) : base($"Null exception on {on}") { }
}
