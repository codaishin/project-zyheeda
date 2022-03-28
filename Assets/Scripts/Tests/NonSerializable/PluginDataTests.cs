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
		var a = root.Extent<PluginDataA>();

		Assert.AreSame(root, a.As<PluginDataRoot>());
	}

	[Test]
	public void AsSourceRoot() {
		var root = new PluginDataRoot();
		var a = root.Extent<PluginDataA>();
		var b = a.Extent<PluginDataB>();

		Assert.AreSame(root, b.As<PluginDataRoot>());
	}

	[Test]
	public void PreventDoubleAdd() {
		var root = new PluginDataRoot();
		var a = root.Extent<PluginDataA>();
		var b = a.Extent<PluginDataA>();

		Assert.AreSame(a, b.As<PluginDataA>());
		Assert.AreSame(a, b);
	}

	[Test]
	public void PreventDoubleIndirectAdd() {
		var root = new PluginDataRoot();
		var a = root.Extent<PluginDataA>();
		var b = a.Extent<PluginDataB>();
		var c = b.Extent<PluginDataA>();

		Assert.AreSame(a, c.As<PluginDataA>());
		Assert.AreSame(a, c);
	}

	[Test]
	public void QueryFromSource() {
		var root = new PluginDataRoot();
		var a = root.Extent<PluginDataA>();

		Assert.AreSame(a, root.As<PluginDataA>());
	}

	[Test]
	public void QueryFromSourceMultipleAdds() {
		var root = new PluginDataRoot();
		var a = root.Extent<PluginDataA>();
		var b = root.Extent<PluginDataB>();

		Assert.AreSame(a, root.As<PluginDataA>());
		Assert.AreSame(b, root.As<PluginDataB>());
	}
}
