using NUnit.Framework;

public class PluginDataTests
{
	class PluginDataRoot : PluginData { public PluginDataRoot() : base() { } }

	class PluginDataA : PluginData { }

	class PluginDataB : PluginData { }

	[Test]
	public void AsSelf() {
		var root = new PluginDataRoot();

		Assert.AreSame(root, root.As<PluginDataRoot>());
	}

	[Test]
	public void AsRoot() {
		var root = new PluginDataRoot();
		var a = PluginData.Add<PluginDataA>(root);

		Assert.AreSame(root, a.As<PluginDataRoot>());
	}

	[Test]
	public void AsSourceRoot() {
		var root = new PluginDataRoot();
		var a = PluginData.Add<PluginDataA>(root);
		var b = PluginData.Add<PluginDataB>(a);

		Assert.AreSame(root, b.As<PluginDataRoot>());
	}

	[Test]
	public void PreventDoubleAdd() {
		var root = new PluginDataRoot();
		var a = PluginData.Add<PluginDataA>(root);
		var b = PluginData.Add<PluginDataA>(a);

		Assert.AreSame(a, b.As<PluginDataA>());
		Assert.AreSame(a, b);
	}

	[Test]
	public void PreventDoubleIndirectAdd() {
		var root = new PluginDataRoot();
		var a = PluginData.Add<PluginDataA>(root);
		var b = PluginData.Add<PluginDataB>(a);
		var c = PluginData.Add<PluginDataA>(b);

		Assert.AreSame(a, c.As<PluginDataA>());
		Assert.AreSame(b, c);
	}
}
