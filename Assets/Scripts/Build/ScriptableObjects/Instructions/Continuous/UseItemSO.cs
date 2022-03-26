using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct ItemAgent
{
	public ILoadoutManager loadout;
	public IAnimationStates animation;
	public Func<Transform?> findTarget;
}

[CreateAssetMenu(menuName = "ScriptableObjects/Instructions/UseItem")]
public class UseItemSO : BaseInstructionsSO<ItemAgent>
{
	public Reference<IHit> hitter;

	protected override ItemAgent GetConcreteAgent(GameObject agent) {
		if (this.hitter.Value == null) {
			throw new NullReferenceException(
				$"hitter on {this} must be set, but was null"
			);
		}
		return new ItemAgent {
			loadout = agent.RequireComponent<ILoadoutManager>(true),
			animation = agent.RequireComponent<IAnimationStates>(true),
			findTarget = () => this.hitter.Value.Try(agent.transform),
		};
	}

	protected override RawInstructions Instructions(ItemAgent agent) {
		IEnumerable<WaitForSeconds> useItem(PluginData _) {
			IItem? item = agent.loadout.Current.Item;
			Transform? target = agent.findTarget();
			Action? use;

			if (target == null || item == null) {
				yield break;
			}

			use = item.GetUseOn(target);
			if (use == null) {
				yield break;
			}

			IEnumerable<WaitForSeconds> run = UseItemSO.GetRun(
				() => agent.animation.Set(item.ActiveState),
				() => agent.animation.Set(Animation.State.Idle),
				use,
				item.UseAfterSeconds,
				item.LeaveActiveStateAfterSeconds
			);

			foreach (WaitForSeconds wait in run) {
				yield return wait;
			}
		};

		return useItem;
	}

	private static IEnumerable<WaitForSeconds> GetRun(
		Action animationStart,
		Action animationEnd,
		Action use,
		float timeUse,
		float timeAnimationEnd
	) {
		(Action run, float time)[] actions = new (Action, float)[] {
			(use, timeUse),
			(animationEnd, timeAnimationEnd)
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
