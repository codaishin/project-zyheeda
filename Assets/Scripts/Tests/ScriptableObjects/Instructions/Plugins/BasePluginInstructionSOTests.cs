using NUnit.Framework;

public class BasePluginInstructionSOTests : TestCollection
{
	[Test]
	public void PluginCallbacksAdd() {
		var called = "";
		var a = new PluginCallbacks() {
			onBegin = _ => called += "a",
			onBeforeYield = _ => called += "a",
			onAfterYield = _ => called += "a",
			onEnd = _ => called += "a",
		};
		var b = new PluginCallbacks() {
			onBegin = _ => called += "b",
			onBeforeYield = _ => called += "b",
			onAfterYield = _ => called += "b",
			onEnd = _ => called += "b",
		};
		var c = a + b;

		c.onBegin?.Invoke(new CorePluginData());

		Assert.AreEqual("ab", called);

		called = "";
		c.onBeforeYield?.Invoke(new CorePluginData());

		Assert.AreEqual("ab", called);

		called = "";
		c.onAfterYield?.Invoke(new CorePluginData());

		Assert.AreEqual("ab", called);

		called = "";
		c.onEnd?.Invoke(new CorePluginData());

		Assert.AreEqual("ab", called);
	}
}
