public abstract class BasePlayerAnimatorMB<TAnimator> :
	BaseAnimatorMB<TAnimator>
	where TAnimator : IAnimator
{
	public void Move(bool value) {
		this.animator!.SetBool("move", value);
	}
}

public class PlayerAnimatorMB : BasePlayerAnimatorMB<WAnimator> { }
