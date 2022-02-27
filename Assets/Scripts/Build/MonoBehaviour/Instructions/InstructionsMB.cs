using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum OverrideMode
{
	None,
	Own,
	All,
}

public class InstructionsMB : MonoBehaviour
{
	private IEnumerator<YieldInstruction>? currentCoroutine;
	private CoroutineInstructions? instructions;

	public CoroutineRunnerMB? runner;
	public BaseInstructionsSO? instructionsSO;
	public Reference agent;
	public OverrideMode overrideMode;

	public UnityEvent? onBegin;
	public UnityEvent? onEnd;

	private MonoBehaviour OnRunnerOrSelf =>
		this.runner != null
			? this.runner
			: this;

	private void Start() {
		if (this.onBegin == null) this.onBegin = new UnityEvent();
		if (this.onEnd == null) this.onEnd = new UnityEvent();

		GameObject agent = this.agent.GameObject;
		this.instructions = this.instructionsSO!.InstructionsFor(agent);
	}

	private IEnumerator<YieldInstruction> GetCoroutine() {
		this.onBegin!.Invoke();
		foreach (YieldInstruction hold in this.instructions!.Invoke()) {
			yield return hold;
		}
		this.onEnd!.Invoke();
	}

	private void StopNone() { }

	private void StopAll() {
		this.OnRunnerOrSelf.StopAllCoroutines();
	}

	private void StopOnw() {
		if (this.currentCoroutine == null) return;

		this.OnRunnerOrSelf.StopCoroutine(this.currentCoroutine);
	}

	public void Begin() {
		Action stop = this.overrideMode switch {
			OverrideMode.All => this.StopAll,
			OverrideMode.Own => this.StopOnw,
			_ => this.StopNone,
		};
		stop();

		this.currentCoroutine = this.GetCoroutine();
		this.OnRunnerOrSelf.StartCoroutine(this.currentCoroutine);
	}
}
