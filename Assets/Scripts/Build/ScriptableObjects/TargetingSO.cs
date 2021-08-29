using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Targeting")]
public class TargetingSO : BaseTargetingSO<CharacterSheetMB>
{
	private class Selector : IEnumerable<WaitForEndOfFrame>
	{
		private bool active = true;

		public TargetingSO targeting;
		public List<CharacterSheetMB> targets;
		public CharacterSheetMB source;
		public int count;

		private bool IsActive {
			get => this.targets.Count < this.count && this.active;
		}

		private void Cancel() {
			this.active = targets.RemoveLast();
		}

		private void AddTarget() {
			if (this.Hit(out CharacterSheetMB target) && this.NotFinished(target)) {
				targets.Add(target);
			}
		}

		private bool NotFinished(CharacterSheetMB target) {
			if (this.targeting.doubleSelectFinishes && this.targets.Contains(target)) {
				this.count = this.targets.Count;
				return false;
			}
			return true;
		}

		private bool Hit(out CharacterSheetMB target) {
			return this.targeting.hitter.Hit.Try(this.source, out target);
		}

		public IEnumerator<WaitForEndOfFrame> GetEnumerator() {
			this.targeting.cancelSelect.Listeners += this.Cancel;
			this.targeting.selectTarget.Listeners += this.AddTarget;

			while (this.IsActive) {
				yield return new WaitForEndOfFrame();
			}

			this.targeting.cancelSelect.Listeners -= this.Cancel;
			this.targeting.selectTarget.Listeners -= this.AddTarget;
		}

		IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
	}

	public BaseHitSO hitter;
	public EventSO selectTarget;
	public EventSO cancelSelect;
	public bool doubleSelectFinishes;

	protected override IEnumerable<WaitForEndOfFrame> DoSelect(
		CharacterSheetMB source,
		List<CharacterSheetMB> targets,
		int count
	) {
		return new Selector {
			targeting = this,
			targets = targets,
			source = source,
			count = count,
		};
	}
}
