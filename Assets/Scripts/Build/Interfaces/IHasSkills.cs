using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHasSkills<TSkill>
	where TSkill :
		IHasBegin
{
	TSkill[] Skills { get; }
}
