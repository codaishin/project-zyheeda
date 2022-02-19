using System.ComponentModel;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

[InitializeOnLoad]
[DisplayName("Hold Repeat")]
public class RepeatHoldInteraction : IInputInteraction
{
	public float duration;
	public float pressPoint;

	private float durationOrDefault =>
		this.duration > 0.0
			? this.duration
			: InputSystem.settings.defaultHoldTime;
	private float pressPointOrDefault =>
		this.pressPoint > 0.0
			? this.pressPoint
			: InputSystem.settings.defaultButtonPressPoint;

	private double timePressed;

	public void Process(ref InputInteractionContext context) {
		switch (context.phase) {
			case InputActionPhase.Waiting:
				if (context.ControlIsActuated(this.pressPointOrDefault)) {
					this.timePressed = context.time;
					context.Started();
					context.SetTimeout(this.durationOrDefault);
				}
				return;
			case InputActionPhase.Started:
				if (!context.ControlIsActuated()) {
					context.Canceled();
					return;
				}
				if (context.time - this.timePressed >= this.durationOrDefault) {
					context.PerformedAndStayStarted();
					context.SetTimeout(Time.deltaTime);
				}
				return;
			case InputActionPhase.Performed:
				if (!context.ControlIsActuated(this.pressPointOrDefault)) {
					context.Canceled();
				}
				return;
		}
	}

	static RepeatHoldInteraction() {
		InputSystem.RegisterInteraction<RepeatHoldInteraction>();
	}

	public void Reset() {
		this.timePressed = 0;
	}
}
