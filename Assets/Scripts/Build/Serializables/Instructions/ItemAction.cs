using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct ItemActionData
{
	public Transform transform;
	public IAnimationStates states;
	public IHit hitter;
	public IApplicable<Transform> effect;
}


[Serializable]
public class ItemAction : BaseInstructions<ItemActionData>
{
	public Animation.Stance idleStance;
	public Animation.State activeState;
	public float useAfterSeconds;
	public float leaveActiveStateAfterSeconds;
	public Reference<IHit> hitter;
	public Reference<IApplicable<Transform>> effect;

	protected override ItemActionData GetConcreteAgent(GameObject agent) {
		if (this.hitter.Value == null) {
			throw new NullReferenceException(
				$"hitter on {this} must be set, but was null"
			);
		}
		if (this.effect.Value == null) {
			throw new NullReferenceException(
				$"effect on {this} must be set, but was null"
			);
		}
		return new ItemActionData {
			transform = agent.transform,
			states = agent.RequireComponent<IAnimationStates>(true),
			hitter = this.hitter.Value,
			effect = this.effect.Value,
		};
	}

	protected override PartialInstructionFunc PartialInstructions(
		ItemActionData agent
	) {

		IEnumerable<YieldInstruction>? action(PluginData _) {
			Transform? target = agent.hitter.Try(agent.transform);

			if (target == null) {
				return null;
			}

			return this.Instructions(
				() => agent.states.Set(this.activeState),
				() => agent.states.Set(Animation.State.Idle),
				() => this.effect.Value!.Apply(target)
			);
		}

		return action;
	}

	private IEnumerable<WaitForSeconds> Instructions(
		Action animationStart,
		Action animationEnd,
		Action use
	) {
		(Action run, float time)[] actions = new (Action, float)[] {
			(use, this.useAfterSeconds),
			(animationEnd, this.leaveActiveStateAfterSeconds)
		};
		float elapsed = 0f;

		animationStart();
		foreach ((Action run, float time) in actions.OrderBy(a => a.time)) {
			yield return new WaitForSeconds(time - elapsed);
			run();
			elapsed += time;
		}
	}
}
