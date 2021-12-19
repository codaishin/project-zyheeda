using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAnimatorMB<TAnimator> :
	MonoBehaviour
	where TAnimator :
		IAnimator
{
	public TAnimator? animator;
}
