using UnityEditor;
using UnityEngine.InputSystem;

[InitializeOnLoad]
public class RepeatHoldInteraction : IInputInteraction
{
	public float pressPoint;
	public float holdTime;
	public int frequency = 50;

	private double startTime;
	private float performedTimeout;

	private delegate void InputAction(ref InputInteractionContext context);

	private void Start(ref InputInteractionContext context) {
		if (!context.ControlIsActuated(this.pressPoint)) return;

		this.startTime = context.time;
		this.performedTimeout = 1f / this.frequency;
		context.Started();
		context.SetTimeout(this.holdTime);
	}

	private void Perform(ref InputInteractionContext context) {
		if (!context.ControlIsActuated(this.pressPoint)) {
			context.Canceled();
			return;
		}
		if (context.time - this.startTime >= this.holdTime) {
			context.PerformedAndStayPerformed();
			context.SetTimeout(this.performedTimeout);
		}
	}

	private void DoNothing(ref InputInteractionContext _) { }

	public void Process(ref InputInteractionContext context) {
		InputAction processPhase = context.phase switch {
			InputActionPhase.Waiting => this.Start,
			InputActionPhase.Started or InputActionPhase.Performed => this.Perform,
			_ => this.DoNothing,
		};
		processPhase(ref context);
	}

	public void Reset() { }

	static RepeatHoldInteraction() {
		InputSystem.RegisterInteraction<RepeatHoldInteraction>();
	}
}
