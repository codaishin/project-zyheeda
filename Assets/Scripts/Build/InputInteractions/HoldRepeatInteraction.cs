using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
[InitializeOnLoad]
#endif
public class RepeatHoldInteraction : IInputInteraction
{
	public float pressPoint;
	public float holdTime;
	public int frequency = 50;

	private float performedTimeout;

	private delegate void InputAction(ref InputInteractionContext context);

	private void Start(ref InputInteractionContext context) {
		this.performedTimeout = 1f / this.frequency;

		context.Started();
		context.SetTimeout(this.holdTime);
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
		InputAction processPhase = context.phase switch {
			InputActionPhase.Waiting
			when context.ControlIsActuated(this.pressPoint) =>
				this.Start,
			InputActionPhase.Started or InputActionPhase.Performed
			when !context.ControlIsActuated(this.pressPoint) =>
				this.Cancel,
			InputActionPhase.Started or InputActionPhase.Performed
			when context.timerHasExpired =>
				this.HoldRepeat,
			_ =>
				this.DoNothing,
		};
		processPhase(ref context);
	}

	public void Reset() { }

	static RepeatHoldInteraction() {
		InputSystem.RegisterInteraction<RepeatHoldInteraction>();
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	static void OnRuntimeMethodLoad() {
		InputSystem.RegisterInteraction<RepeatHoldInteraction>();
	}
}
