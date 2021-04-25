using System.Collections.Generic;
using UnityEngine;

public class TargetingSO : BaseTargetingSO
{
	public BaseHitSO hitter;
	public EventSO selectTarget;
	public EventSO cancelSelect;
	public bool upToMaxCount;

	public override
	IEnumerator<WaitForEndOfFrame> Select(CharacterSheetMB source, List<CharacterSheetMB> targets, int maxCount = 1)
	{
		bool canceled = false;

		void select() {
			if (this.hitter.Hit.Try(source, out CharacterSheetMB target)) {
				if (this.upToMaxCount && targets.Contains(target)) {
					maxCount = targets.Count;
				} else {
					targets.Add(target);
				}
			}
		}
		void cancel() {
			if (targets.Count > 0) {
				targets.RemoveAt(targets.Count - 1);
			} else {
				canceled = true;
			}
		}

		this.selectTarget.Listeners += select;
		this.cancelSelect.Listeners += cancel;

		while(targets.Count < maxCount && !canceled) {
			yield return new WaitForEndOfFrame();
		}

		this.selectTarget.Listeners -= select;
		this.cancelSelect.Listeners -= cancel;
	}
}
