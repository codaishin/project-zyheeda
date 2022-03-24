using System.Collections.Generic;
using UnityEngine;

public interface IAnimationStates
{
	void Set(Animation.State state);
	void Blend(Animation.BlendState state, float weight);
}

public interface IAnimationLayers
{
	void Set(Animation.Layer layer, float weight);
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

	private static Dictionary<BlendState, string> blendStateParameterMap = new() {
		{ BlendState.WalkOrRun, "blendWalkRun" },
	};

	public static string Parameter(this BlendState state) {
		return Animation.blendStateParameterMap
			.TryGetValue(state, out string value)
				? value
				: throw new System.ArgumentException();
	}

	public enum Layer
	{
		Default = default,
		HoldRifle,
	}

	private static Dictionary<Layer, string> layerNameMap = new() {
		{ Layer.Default, "Base Layer" },
		{ Layer.HoldRifle, "Hold Rifle Layer" },
	};

	public static string Name(this Layer layer) {
		return Animation.layerNameMap
			.TryGetValue(layer, out string value)
				? value
				: throw new System.ArgumentException();
	}
}

[RequireComponent(typeof(Animator))]
public class AnimationMB : MonoBehaviour, IAnimationStates, IAnimationLayers
{
	private Animator? animator;

	private void Awake() {
		this.animator = this.RequireComponent<Animator>();
	}

	public void Set(Animation.State state) {
		this.animator!.SetInteger("state", (int)state);
	}

	public void Set(Animation.Layer layer, float weight) {
		int index = this.animator!.GetLayerIndex(layer.Name());
		this.animator!.SetLayerWeight(index, weight);
	}

	public void Blend(Animation.BlendState state, float weight) {
		this.animator!.SetFloat(state.Parameter(), weight);
	}
}
