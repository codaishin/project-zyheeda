public abstract class BasePlayerAnimatorMB<TAnimator> :
	BaseAnimatorMB<TAnimator>
	where TAnimator : IAnimator
{
	public void StartMove(float blendWalkRun) {
		this.animator!.SetBool("move", true);
		this.animator!.SetFloat("blendWalkRun", blendWalkRun);
	}

	public void StopMove() {
		this.animator!.SetBool("move", false);
		this.animator!.SetFloat("blendWalkRun", 0);
	}
}

public class PlayerAnimatorMB : BasePlayerAnimatorMB<WAnimator> { }
