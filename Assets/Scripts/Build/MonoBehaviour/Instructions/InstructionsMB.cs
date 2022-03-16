using System;
using System.Collections.Generic;
using UnityEngine;

public enum OverrideMode
{
	None,
	Own,
	All,
}

public class InstructionsMB : MonoBehaviour, IApplicable
{
	private IEnumerator<YieldInstruction>? currentCoroutine;
	private Func<IEnumerable<YieldInstruction>>? instructions;
	private bool run;

	public CoroutineRunnerMB? runner;
	public BaseInstructionsSO? instructionsSO;
	public Reference agent;
	public OverrideMode overrideMode;

	private MonoBehaviour OnRunnerOrSelf =>
		this.runner != null
			? this.runner
			: this;

	private void Start() {
		GameObject agent = this.agent.GameObject;
		this.instructions = this.instructionsSO!.GetInstructionsFor(
			agent,
			this.KeepRunning
		);
	}

	private IEnumerator<YieldInstruction> GetCoroutine() {
		return this.instructions!.Invoke().GetEnumerator();
	}

	private void StopNone() { }

	private void StopAll() {
		this.OnRunnerOrSelf.StopAllCoroutines();
	}

	private void StopOnw() {
		if (this.currentCoroutine == null) return;

		this.OnRunnerOrSelf.StopCoroutine(this.currentCoroutine);
	}

	private bool KeepRunning() {
		return this.run;
	}

	public void Apply() {
		Action stop = this.overrideMode switch {
			OverrideMode.All => this.StopAll,
			OverrideMode.Own => this.StopOnw,
			_ => this.StopNone,
		};
		stop();

		this.run = true;
		this.currentCoroutine = this.GetCoroutine();
		this.OnRunnerOrSelf.StartCoroutine(this.currentCoroutine);
	}

	public void Release() {
		this.run = false;
	}
}
