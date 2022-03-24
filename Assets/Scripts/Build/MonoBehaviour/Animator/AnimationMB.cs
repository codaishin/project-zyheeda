using System.Collections.Generic;
using UnityEngine;

public interface IAnimationStates
{
	void Set(Animation.State state);
	void Blend(Animation.BlendState state, float weight);
}

public interface IAnimationStance
{
	void Set(Animation.Stance layer, bool value);
}

public static class Animation
{
	public enum State
	{
		Idle = 0,
		WalkOrRun = 1,
		ShootRifle = 2,
	}

	public enum BlendState
	{
		WalkOrRun,
	}

	private static Dictionary<BlendState, string> blendStateParameterMap = new() {
		{ BlendState.WalkOrRun, "blendWalkRun" },
	};

	public static string Parameter(this BlendState state) {
		return Animation.blendStateParameterMap
			.TryGetValue(state, out string value)
				? value
				: throw new System.ArgumentException();
	}

	public enum Stance
	{
		Default = default,
		HoldRifle,
	}

	private static Dictionary<Stance, string> layerNameMap = new() {
		{ Stance.Default, "default" },
		{ Stance.HoldRifle, "holdRifle" },
	};

	public static string Name(this Stance layer) {
		return Animation.layerNameMap
			.TryGetValue(layer, out string value)
				? value
				: throw new System.ArgumentException();
	}
}

[RequireComponent(typeof(Animator))]
public class AnimationMB : MonoBehaviour, IAnimationStates, IAnimationStance
{
	private Animator? animator;

	private void Awake() {
		this.animator = this.RequireComponent<Animator>();
	}

	public void Set(Animation.State state) {
		this.animator!.SetInteger("state", (int)state);
	}

	public void Set(Animation.Stance stance, bool value) {
		if (stance == Animation.Stance.Default) {
			return;
		}
		string name = stance.Name();
		int index = this.animator!.GetLayerIndex(name);

		this.animator!.SetLayerWeight(index, value ? 1 : 0);
		this.animator!.SetBool(name, value);
	}

	public void Blend(Animation.BlendState state, float weight) {
		this.animator!.SetFloat(state.Parameter(), weight);
	}
}
