using System.Collections.Generic;
using UnityEngine;

public interface IAnimation
{
	void Set(Animation.State state);
	void Blend(Animation.BlendState state, float weight);
}

public static class Animation
{
	public enum State
	{
		Idle = 0,
		WalkOrRun = 1,
	}

	public enum BlendState
	{
		WalkOrRun,
	}

	private static Dictionary<BlendState, string> blendStateMap = new() {
		{ BlendState.WalkOrRun, "blendWalkRun" },
	};

	public static string Parameter(this BlendState state) {
		return Animation.blendStateMap
			.TryGetValue(state, out string value)
				? value
				: throw new System.ArgumentException();
	}
}

[RequireComponent(typeof(Animator))]
public class AnimationMB : MonoBehaviour, IAnimation
{
	private Animator? animator;

	private void Awake() {
		this.animator = this.RequireComponent<Animator>();
	}

	public void Set(Animation.State state) {
		this.animator!.SetInteger("state", (int)state);
	}

	public void Blend(Animation.BlendState state, float weight) {
		this.animator!.SetFloat(state.Parameter(), weight);
	}
}
