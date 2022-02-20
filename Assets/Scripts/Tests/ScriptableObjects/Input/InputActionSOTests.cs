using NUnit.Framework;
using UnityEngine;

public class InputActionSOTests : TestCollection
{
	[Test]
	public void TestWalk() {
		var playerConf = ScriptableObject.CreateInstance<PlayerInputConfigSO>();
		var input = ScriptableObject.CreateInstance<InputActionSO>();

		input.playerConfigSO = playerConf;
		input.inputOption = InputOption.Walk;

		Assert.AreSame(playerConf.Config.Movement.Walk, input.Action);
	}

	[Test]
	public void TestRun() {
		var playerConf = ScriptableObject.CreateInstance<PlayerInputConfigSO>();
		var input = ScriptableObject.CreateInstance<InputActionSO>();

		input.playerConfigSO = playerConf;
		input.inputOption = InputOption.Run;

		Assert.AreSame(playerConf.Config.Movement.Run, input.Action);
	}

	[Test]
	public void TestMousePosition() {
		var playerConf = ScriptableObject.CreateInstance<PlayerInputConfigSO>();
		var input = ScriptableObject.CreateInstance<InputActionSO>();

		input.playerConfigSO = playerConf;
		input.inputOption = InputOption.MousePosition;

		Assert.AreSame(playerConf.Config.Mouse.Position, input.Action);
	}
}
