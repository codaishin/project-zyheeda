using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Routines
{
	public class BaseModifierFactoryTests : TestCollection
	{
		class MockData : Data { }

		class MockMB : MonoBehaviour { }

		class MockModifierFactory : BaseModifierFactory<MockMB, MockData>
		{
			public Func<GameObject, MockMB> getConcreteAgent =
				obj => obj.RequireComponent<MockMB>();
			public Func<Data, MockData> getPluginData =
				data => data.As<MockData>()!;
			public Func<MockMB, MockData, Action?> getModifier =
				(_, __) => null;

			public override MockMB GetConcreteAgent(GameObject agent) =>
				this.getConcreteAgent(agent);
			public override MockData GetRoutineData(Data data) =>
				this.getPluginData(data);
			protected override Action? GetAction(MockMB agent, MockData data) =>
				this.getModifier(agent, data);
		}

		[UnityTest]
		public IEnumerator GetConcreteAgent() {
			var calledAgent = null as GameObject;
			var calledMB = null as MonoBehaviour;
			var factory = new MockModifierFactory();
			var mockMB = new GameObject().AddComponent<MockMB>();
			var agent = new GameObject().AddComponent<MockMB>();

			factory.getConcreteAgent = agent => {
				calledAgent = agent;
				return mockMB;
			};
			factory.getModifier = (agent, _) => {
				calledMB = agent;
				return null;
			};

			yield return new WaitForEndOfFrame();

			var modifierFn = factory.GetModifierFnFor(agent.gameObject);

			Assert.AreSame(agent.gameObject, calledAgent);

			modifierFn(new MockData());

			Assert.AreSame(mockMB, calledMB);
		}

		[UnityTest]
		public IEnumerator GetPluginData() {
			var calledData = null as Data;
			var calledMockData = null as MockData;
			var factory = new MockModifierFactory();
			var data = new Data();
			var mockdata = new MockData();
			var agent = new GameObject().AddComponent<MockMB>();


			factory.getPluginData = data => {
				calledData = data;
				return mockdata;
			};
			factory.getModifier = (_, data) => {
				calledMockData = data;
				return null;
			};

			yield return new WaitForEndOfFrame();

			_ = factory.GetModifierFnFor(agent.gameObject)(data);

			Assert.AreSame(data, calledData);
			Assert.AreSame(mockdata, calledMockData);
		}

		[UnityTest]
		public IEnumerator ReturnPluginCallbacks() {
			var called = 0;
			var factory = new MockModifierFactory();
			var agent = new GameObject().AddComponent<MockMB>();

			factory.getModifier = (_, __) => () => ++called;

			yield return new WaitForEndOfFrame();

			var modifierFn = factory.GetModifierFnFor(agent.gameObject);
			var modifier = modifierFn(new Data())!;

			modifier();

			Assert.AreEqual(1, called);
		}
	}
}
