using NUnit.Framework;
using Routines;

public class RoutineDataTests
{
	class DataRoot : RoutineData
	{
		public DataRoot() : base() { }
	}

	class DataA : RoutineData { }

	class DataB : RoutineData { }

	[Test]
	public void AsSelf() {
		var root = new DataRoot();

		Assert.AreSame(root, root.As<DataRoot>());
	}

	[Test]
	public void AsRoot() {
		var root = new DataRoot();
		var a = root.Extent<DataA>();

		Assert.AreSame(root, a.As<DataRoot>());
	}

	[Test]
	public void AsSourceRoot() {
		var root = new DataRoot();
		var a = root.Extent<DataA>();
		var b = a.Extent<DataB>();

		Assert.AreSame(root, b.As<DataRoot>());
	}

	[Test]
	public void PreventDoubleAdd() {
		var root = new DataRoot();
		var a = root.Extent<DataA>();
		var b = a.Extent<DataA>();

		Assert.AreSame(a, b.As<DataA>());
		Assert.AreSame(a, b);
	}

	[Test]
	public void PreventDoubleIndirectAdd() {
		var root = new DataRoot();
		var a = root.Extent<DataA>();
		var b = a.Extent<DataB>();
		var c = b.Extent<DataA>();

		Assert.AreSame(a, c.As<DataA>());
		Assert.AreSame(a, c);
	}

	[Test]
	public void QueryFromSource() {
		var root = new DataRoot();
		var a = root.Extent<DataA>();

		Assert.AreSame(a, root.As<DataA>());
	}

	[Test]
	public void QueryFromSourceMultipleAdds() {
		var root = new DataRoot();
		var a = root.Extent<DataA>();
		var b = root.Extent<DataB>();

		Assert.AreSame(a, root.As<DataA>());
		Assert.AreSame(b, root.As<DataB>());
	}
}
