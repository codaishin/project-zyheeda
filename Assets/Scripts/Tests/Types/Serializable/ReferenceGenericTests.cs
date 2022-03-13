using System.Reflection;
using NUnit.Framework;
using UnityEngine;

public class ReferenceGenericTests : TestCollection
{
	interface MockInterface { }
	class MockComponent : MonoBehaviour, MockInterface { }
	class MockScriptableObject : ScriptableObject, MockInterface { }

	[Test]
	public void WrapComponent() {
		var comp = new GameObject().AddComponent<MockComponent>();
		var obj = Reference<MockInterface>.PointToComponent(comp);
		Assert.AreSame(comp, obj.Value);
	}

	[Test]
	public void WrapScriptableObject() {
		var so = ScriptableObject.CreateInstance<MockScriptableObject>();
		var obj = Reference<MockInterface>.PointToScriptableObject(so);
		Assert.AreSame(so, obj.Value);
	}

	[Test]
	public void ThrowWhenTypeMismatch() {
		var obj = new Reference<MockInterface>();
		var value = new GameObject();
		var objBoxed = (object)obj;

		objBoxed
			.GetType()
			.GetField("value", BindingFlags.NonPublic | BindingFlags.Instance)
			.SetValue(objBoxed, value);
		obj = (Reference<MockInterface>)objBoxed;

		Assert.Throws<System.InvalidCastException>(() => _ = obj.Value);
	}

	[Test]
	public void ValueNonWhenNotSet() {
		var obj = new Reference<MockInterface>();

		Assert.Null(obj.Value);
	}
}
