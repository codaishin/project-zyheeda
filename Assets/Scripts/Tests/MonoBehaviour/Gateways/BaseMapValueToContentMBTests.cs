using System.Linq;
using NUnit.Framework;
using UnityEngine;

public class BaseMapValueToContentMBTests : TestCollection
{
	class MockCollection : IHasValue<int[]>
	{
		public int[] Value { get; set; } = new int[0];
	}

	class MockGatewayMB : BaseMapValueToContentMB<string[], int[], MockCollection>
	{
		public override int[] MapValueToContent(string[] value) {
			return value.Select(s => s.Length).ToArray();
		}
	}

	[Test]
	public void SetElems() {
		var gateway = new GameObject("gateway").AddComponent<MockGatewayMB>();
		var content = new MockCollection();
		gateway.content = content;
		gateway.Value = new string[] { "123", "12", "1234" };

		CollectionAssert.AreEqual(new int[] { 3, 2, 4 }, content.Value);
	}

	[Test]
	public void Getource() {
		var gateway = new GameObject("gateway").AddComponent<MockGatewayMB>();
		var content = new MockCollection();
		var value = new string[0];
		gateway.content = content;
		gateway.Value = value;

		Assert.AreSame(value, gateway.Value);
	}
}
