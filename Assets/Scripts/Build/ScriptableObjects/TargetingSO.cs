using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Targeting")]
public class TargetingSO : BaseTargetingSO<CharacterSheetMB>
{
	private class Selector : IEnumerable<WaitForEndOfFrame>
	{
		private BaseHitSO hitter;
		private ChannelSO selectTarget;
		private ChannelSO cancelSelect;
		private List<CharacterSheetMB> targets;
		private CharacterSheetMB source;
		private int count;
		private bool doubleSelectFinishes;
		private bool active = true;

		private bool IsActive => this.targets.Count < this.count && this.active;

		public Selector(
			TargetingSO targeting,
			List<CharacterSheetMB> targets,
			CharacterSheetMB source,
			int count
		) {
			if (targeting.hitter == null) throw this.NullError();
			if (targeting.cancelSelect == null) throw this.NullError();
			if (targeting.selectTarget == null) throw this.NullError();
			this.hitter = targeting.hitter;
			this.cancelSelect = targeting.cancelSelect;
			this.selectTarget = targeting.selectTarget;
			this.doubleSelectFinishes = targeting.doubleSelectFinishes;
			this.targets = targets;
			this.source = source;
			this.count = count;
		}

		private void Cancel() {
			this.active = targets.RemoveLast();
		}

		private void AddTarget(CharacterSheetMB target) {
			if (this.NotFinished(target)) {
				targets.Add(target);
			}
		}

		private void TryAddTarget() {
			this.TryHit().Match(
				some: this.AddTarget,
				none: () => { }
			);
		}

		private bool NotFinished(CharacterSheetMB target) {
			if (this.doubleSelectFinishes && this.targets.Contains(target)) {
				this.count = this.targets.Count;
				return false;
			}
			return true;
		}

		private Maybe<CharacterSheetMB> TryHit() {
			if (this.hitter == null) throw this.NullError();
			return this.hitter.Hit.Try(this.source);
		}

		public IEnumerator<WaitForEndOfFrame> GetEnumerator() {
			this.cancelSelect.Listeners += this.Cancel;
			this.selectTarget.Listeners += this.TryAddTarget;

			while (this.IsActive) {
				yield return new WaitForEndOfFrame();
			}

			this.cancelSelect.Listeners -= this.Cancel;
			this.selectTarget.Listeners -= this.TryAddTarget;
		}

		IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
	}

	public BaseHitSO? hitter;
	public ChannelSO? selectTarget;
	public ChannelSO? cancelSelect;
	public bool doubleSelectFinishes;

	protected override IEnumerable<WaitForEndOfFrame> DoSelect(
		CharacterSheetMB source,
		List<CharacterSheetMB> targets,
		int count
	) {
		return new Selector(this, targets, source, count);
	}
}
