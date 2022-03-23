using UnityEngine;

public interface IStanceAnimation
{
	void Set(Stance stance, float weight);
}

public enum Stance
{
	Default = 0,
	HoldRifle = 1,
}

[RequireComponent(typeof(Animator))]
public class StanceAnimationMB : MonoBehaviour, IStanceAnimation
{
	private Animator? animator;

	public void Set(Stance layer, float weight) {
		this.animator!.SetLayerWeight((int)layer, weight);
	}

	private void Awake() {
		this.animator = this.RequireComponent<Animator>();
	}
}
