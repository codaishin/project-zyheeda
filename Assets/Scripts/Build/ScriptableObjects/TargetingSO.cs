using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Targeting")]
public class TargetingSO : BaseTargetingSO<CharacterSheetMB>
{
	public BaseHitSO hitter;
	public EventSO selectTarget;
	public EventSO cancelSelect;
	public bool upToMaxCount;

	private int AddTarget(List<CharacterSheetMB> targets, CharacterSheetMB target, int maxCount)
	{
		if (this.upToMaxCount && targets.Contains(target)) {
			return targets.Count;
		}
		targets.Add(target);
		return maxCount;
	}

	protected override
	IEnumerator<WaitForEndOfFrame> DoSelect(CharacterSheetMB source, List<CharacterSheetMB> targets, int maxCount)
	{
		bool notCanceled = true;

		Action cancel = () => notCanceled = targets.RemoveLast();
		Action select = () => maxCount = this.hitter.Hit.Try(source, out CharacterSheetMB target)
			? this.AddTarget(targets, target, maxCount)
			: maxCount;

		this.cancelSelect.Listeners += cancel;
		this.selectTarget.Listeners += select;

		while (targets.Count < maxCount && notCanceled) {
			yield return new WaitForEndOfFrame();
		}

		this.cancelSelect.Listeners -= cancel;
		this.selectTarget.Listeners -= select;
	}
}
