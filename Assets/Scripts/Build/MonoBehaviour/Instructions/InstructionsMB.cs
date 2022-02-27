using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InstructionsMB : MonoBehaviour
{
	private IEnumerator<YieldInstruction>? currentCoroutine;
	private CoroutineInstructions? instructions;

	public CoroutineRunnerMB? runner;
	public BaseInstructionsSO? instructionsSO;
	public Reference agent;
	public bool oneAtATime;

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

	public void Begin() {
		if (this.oneAtATime) {
			this.Cancel();
		}

		this.currentCoroutine = this.GetCoroutine();
		this.OnRunnerOrSelf.StartCoroutine(this.currentCoroutine);
	}

	public void Cancel() {
		if (this.currentCoroutine == null) {
			return;
		}
		this.OnRunnerOrSelf.StopCoroutine(this.currentCoroutine);
	}
}
