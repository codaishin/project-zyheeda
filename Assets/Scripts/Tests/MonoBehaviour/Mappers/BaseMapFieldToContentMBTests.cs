using System;
using NUnit.Framework;
using UnityEngine;

public class BaseMapFieldToContentMBTests : TestCollection
{
	class MockContentMB : BaseHasValueMB<float> { }

	class MockMapMB : BaseMapFieldToContentMB<int, float>
	{
		public Func<int, float> map = _ => throw new NotImplementedException();

		public override float MapFieldToContentValue(int fieldValue) => this.map(
			fieldValue
		);
	}

	[Test]
	public void AsignMapReturnValue0_4() {
		var mapper = new GameObject("mapper").AddComponent<MockMapMB>();
		var content = new GameObject("content").AddComponent<MockContentMB>();
		content.Value = -1f;
		mapper.map = _ => 0.4f;
		mapper.content = content;

		mapper.Apply();

		Assert.AreEqual(0.4f, content.Value);
	}

	[Test]
	public void AsignMapReturnValue1_7() {
		var mapper = new GameObject("mapper").AddComponent<MockMapMB>();
		var content = new GameObject("content").AddComponent<MockContentMB>();
		content.Value = -1f;
		mapper.map = _ => 1.7f;
		mapper.content = content;

		mapper.Apply();

		Assert.AreEqual(1.7f, content.Value);
	}

	[Test]
	public void UseMapConversion() {
		var mapper = new GameObject("mapper").AddComponent<MockMapMB>();
		var content = new GameObject("content").AddComponent<MockContentMB>();
		content.Value = -1f;
		mapper.map = v => v * 2;
		mapper.value = 2;
		mapper.content = content;

		mapper.Apply();

		Assert.AreEqual(4f, content.Value);
	}
}
