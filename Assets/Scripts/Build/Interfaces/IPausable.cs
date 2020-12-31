using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPausable
{
	bool Paused { get; set; }
}

public interface IPausable<TPauseYield> : IPausable
{
	TPauseYield Pause { get; }
}
