using NUnit.Framework;
using UnityEngine;

public class InputConfigSOTests : TestCollection
{
	[Test]
	public void ConfigIsSame() {
		var config = ScriptableObject.CreateInstance<InputConfigSO>();

		Assert.AreSame(config.Config, config.Config);
	}

	[Test]
	public void TestMovement() {
		var config = ScriptableObject.CreateInstance<InputConfigSO>();

		Assert.AreEqual(
			config.Config.Movement.Get(),
			config[InputEnum.Map.Movement]
		);
	}

	[Test]
	public void TestMouse() {
		var config = ScriptableObject.CreateInstance<InputConfigSO>();

		Assert.AreEqual(
			config.Config.Mouse.Get(),
			config[InputEnum.Map.Mouse]
		);
	}

	[Test]
	public void TestWalk() {
		var config = ScriptableObject.CreateInstance<InputConfigSO>();

		Assert.AreSame(config.Config.Movement.Walk, config[InputEnum.Action.Walk]);
	}

	[Test]
	public void TestRun() {
		var config = ScriptableObject.CreateInstance<InputConfigSO>();

		Assert.AreSame(config.Config.Movement.Run, config[InputEnum.Action.Run]);
	}

	[Test]
	public void TestMousePosition() {
		var config = ScriptableObject.CreateInstance<InputConfigSO>();

		Assert.AreSame(
			config.Config.Mouse.Position,
			config[InputEnum.Action.MousePosition]
		);
	}
}
