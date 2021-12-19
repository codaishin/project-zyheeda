using UnityEngine;

public abstract class BasePlayerAnimatorMB<TAnimator> :
	BaseAnimatorMB<TAnimator>
	where TAnimator : IAnimator
{
	public void Walk(bool value) {
		this.animator!.SetBool("walk", value);
	}
}

public class PlayerAnimatorMB : BasePlayerAnimatorMB<WAnimator> { }
