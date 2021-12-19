using UnityEngine;

public class BaseAnimatorMB<TAnimator> :
	MonoBehaviour
	where TAnimator :
		IAnimator
{
	public TAnimator? animator;
}
