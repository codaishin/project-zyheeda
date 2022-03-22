using UnityEngine;

public interface IMovementAnimation
{
	void Begin();
	void WalkOrRunBlend(float value);
	void End();
}

public enum WalkStates
{
	Idle = 0,
	WalkOrRun = 1,
}

public class MovementAnimationMB : MonoBehaviour, IMovementAnimation
{
	public Animator? animator;

	public void End() {
		this.animator!.SetInteger("state", (int)WalkStates.Idle);
	}

	public void Begin() {
		this.animator!.SetInteger("state", (int)WalkStates.WalkOrRun);
	}

	public void WalkOrRunBlend(float value) {
		this.animator!.SetFloat("blendWalkRun", value);
	}
}
