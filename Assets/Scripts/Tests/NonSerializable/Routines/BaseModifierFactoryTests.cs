using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Routines
{
	public class BasePluginTests : TestCollection
	{
		class MockData : Data { }

		class MockMB : MonoBehaviour { }

		class MockPlugin : BasePlugin<MockMB, MockData>
		{
			public Func<GameObject, MockMB> getConcreteAgent =
				obj => obj.RequireComponent<MockMB>();
			public Func<Data, MockData> getData =
				data => data.As<MockData>()!;
			public Func<MockMB, MockData, Action?> getAction =
				(_, __) => null;

			public override MockMB GetConcreteAgent(GameObject agent) =>
				this.getConcreteAgent(agent);
			public override MockData GetData(Data data) =>
				this.getData(data);
			protected override Action? GetAction(MockMB agent, MockData data) =>
				this.getAction(agent, data);
		}

		[UnityTest]
		public IEnumerator GetConcreteAgent() {
			var calledAgent = null as GameObject;
			var calledMB = null as MonoBehaviour;
			var plugin = new MockPlugin();
			var mockMB = new GameObject().AddComponent<MockMB>();
			var agent = new GameObject().AddComponent<MockMB>();

			plugin.getConcreteAgent = agent => {
				calledAgent = agent;
				return mockMB;
			};
			plugin.getAction = (agent, _) => {
				calledMB = agent;
				return null;
			};

			yield return new WaitForEndOfFrame();

			var modifierFn = plugin.GetPluginFnFor(agent.gameObject);

			Assert.AreSame(agent.gameObject, calledAgent);

			modifierFn(new MockData());

			Assert.AreSame(mockMB, calledMB);
		}

		[UnityTest]
		public IEnumerator GetPluginData() {
			var calledData = null as Data;
			var calledMockData = null as MockData;
			var plugin = new MockPlugin();
			var data = new Data();
			var mockdata = new MockData();
			var agent = new GameObject().AddComponent<MockMB>();


			plugin.getData = data => {
				calledData = data;
				return mockdata;
			};
			plugin.getAction = (_, data) => {
				calledMockData = data;
				return null;
			};

			yield return new WaitForEndOfFrame();

			_ = plugin.GetPluginFnFor(agent.gameObject)(data);

			Assert.AreSame(data, calledData);
			Assert.AreSame(mockdata, calledMockData);
		}

		[UnityTest]
		public IEnumerator ReturnPluginCallbacks() {
			var called = 0;
			var plugin = new MockPlugin();
			var agent = new GameObject().AddComponent<MockMB>();

			plugin.getAction = (_, __) => () => ++called;

			yield return new WaitForEndOfFrame();

			var modifierFn = plugin.GetPluginFnFor(agent.gameObject);
			var modifier = modifierFn(new Data())!;

			modifier();

			Assert.AreEqual(1, called);
		}
	}
}
