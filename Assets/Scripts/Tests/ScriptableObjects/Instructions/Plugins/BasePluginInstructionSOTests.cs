using NUnit.Framework;

public class BasePluginInstructionSOTests : TestCollection
{
	[Test]
	public void PluginCallbacksAdd() {
		var called = "";
		var a = new PluginCallbacks() {
			onBegin = () => called += "a",
			onBeforeYield = () => called += "a",
			onAfterYield = () => called += "a",
			onEnd = () => called += "a",
		};
		var b = new PluginCallbacks() {
			onBegin = () => called += "b",
			onBeforeYield = () => called += "b",
			onAfterYield = () => called += "b",
			onEnd = () => called += "b",
		};
		var c = a + b;

		c.onBegin?.Invoke();

		Assert.AreEqual("ab", called);

		called = "";
		c.onBeforeYield?.Invoke();

		Assert.AreEqual("ab", called);

		called = "";
		c.onAfterYield?.Invoke();

		Assert.AreEqual("ab", called);

		called = "";
		c.onEnd?.Invoke();

		Assert.AreEqual("ab", called);
	}
}
