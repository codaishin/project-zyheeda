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
	private IEnumerator<YieldInstruction?>? currentCoroutine;
	private Func<IEnumerable<YieldInstruction?>>? instructions;
	private bool running;

	public CoroutineRunnerMB? runner;
	public BaseInstructionsSO? instructionsSO;
	public Reference agent;
	public OverrideMode overrideMode;

	private MonoBehaviour OnRunnerOrSelf =>
		this.runner != null
			? this.runner
			: this;

	private void Start() {
		this.instructions = this.instructionsSO!.GetInstructionsFor(
			this.agent.GameObject,
			this.IsRunning
		);
	}

	private IEnumerator<YieldInstruction?> GetCoroutine() {
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

	private bool IsRunning() {
		return this.running;
	}

	public void Apply() {
		Action stop = this.overrideMode switch {
			OverrideMode.All => this.StopAll,
			OverrideMode.Own => this.StopOnw,
			_ => this.StopNone,
		};
		stop();

		this.running = true;
		this.currentCoroutine = this.GetCoroutine();
		this.OnRunnerOrSelf.StartCoroutine(this.currentCoroutine);
	}

	public void Release() {
		this.running = false;
	}
}
