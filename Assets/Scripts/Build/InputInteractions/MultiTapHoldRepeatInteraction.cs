using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
[InitializeOnLoad]
#endif
public class MultiTapHoldRepeatInteraction : IInputInteraction
{
	private enum State { Hold, TapHold, TapWait }

	public int tapCount = 2;
	public float maxTapSpacing;
	public float maxTapDuration;
	public float pressPoint;
	public float holdTime;
	public int frequency = 50;

	private float performedTimeout;
	private int wasTappedCount = 0;
	private State state = State.Hold;

	private delegate void InputAction(ref InputInteractionContext context);

	private void SetStateAndPressTimeout(ref InputInteractionContext context) {
		(State state, float timeout) = this.wasTappedCount == this.tapCount
			? (State.Hold, this.holdTime)
			: (State.TapHold, this.maxTapDuration);

		this.state = state;
		context.SetTimeout(timeout);
	}

	private void Start(ref InputInteractionContext context) {
		this.performedTimeout = 1f / this.frequency;
		this.wasTappedCount = 0;

		context.Started();
		this.SetStateAndPressTimeout(ref context);
	}

	private void Release(ref InputInteractionContext context) {
		this.state = State.TapWait;
		++this.wasTappedCount;
		context.SetTimeout(this.maxTapSpacing);
	}

	private void Press(ref InputInteractionContext context) {
		this.state = State.TapHold;
		this.SetStateAndPressTimeout(ref context);
	}
	private void HoldRepeat(ref InputInteractionContext context) {
		context.PerformedAndStayPerformed();
		context.SetTimeout(this.performedTimeout);
	}

	private void Cancel(ref InputInteractionContext context) {
		context.Canceled();
	}

	private void DoNothing(ref InputInteractionContext _) { }

	public void Process(ref InputInteractionContext context) {
		InputAction processPhase = (context.phase, this.state) switch {
			(InputActionPhase.Waiting, _)
			when context.ControlIsActuated(this.pressPoint) =>
				this.Start,
			(_, State.TapHold or State.TapWait)
			when context.timerHasExpired =>
				this.Cancel,
			(_, State.TapHold)
			when !context.ControlIsActuated(this.pressPoint) =>
				this.Release,
			(_, State.TapWait)
			when context.ControlIsActuated(this.pressPoint) =>
				this.Press,
			(_, State.Hold)
			when !context.ControlIsActuated(this.pressPoint) =>
				this.Cancel,
			(_, State.Hold)
			when context.timerHasExpired =>
				this.HoldRepeat,
			_ =>
				this.DoNothing,
		};
		processPhase(ref context);
	}

	public void Reset() { }

	static MultiTapHoldRepeatInteraction() {
		InputSystem.RegisterInteraction<MultiTapHoldRepeatInteraction>();
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	static void OnRuntimeMethodLoad() {
		InputSystem.RegisterInteraction<MultiTapHoldRepeatInteraction>();
	}
}
