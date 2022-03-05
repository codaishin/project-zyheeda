using UnityEngine;

public interface IMovementAnimation
{
	void Move(float walkOrRunWeight);
	void Stop();
}

public class MovementAnimationMB : MonoBehaviour, IMovementAnimation
{
	public Animator? animator;

	public void Stop() {
		this.animator!.SetBool("move", false);
	}

	public void Move(float walkOrRunWeight) {
		this.animator!.SetBool("move", true);
		this.animator!.SetFloat("blendWalkRun", walkOrRunWeight);
	}
}
