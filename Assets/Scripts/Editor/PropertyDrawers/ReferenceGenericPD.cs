using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

using UnityObject = UnityEngine.Object;


[CustomPropertyDrawer(typeof(Reference<>))]
public class ReferenceGenericPD : PropertyDrawer
{
	private Type GetValueType() {
		return this.fieldInfo.FieldType switch {
			Type type when type.IsArray =>
				type
					.GetElementType()
					.GetGenericArguments()[0],
			Type type when typeof(IEnumerable).IsAssignableFrom(type) =>
				type
					.GetGenericArguments()[0]
					.GetGenericArguments()[0],
			Type type =>
				type
					.GetGenericArguments()[0],
		};
	}

	private static (Component?, string?) GetComponent(GameObject obj, Type type) {
		Component? component = obj.GetComponent(type);
		return component != null
			? (component, null)
			: (null, $"{obj}: must have a component that implements '{type}'");
	}

	private static (UnityObject?, string?) ObjectIfTypeMatches(
		UnityObject obj,
		Type type
	) {
		return type.IsAssignableFrom(obj.GetType())
			? (obj, null)
			: (null, $"{obj}: must implement '{type}'");
	}

	private static (UnityObject?, string?) InstanceOrNull(
		UnityObject? assigned,
		Type type
	) {
		return assigned switch {
			GameObject obj =>
				ReferenceGenericPD.GetComponent(obj, type),
			UnityObject obj =>
				ReferenceGenericPD.ObjectIfTypeMatches(obj, type),
			null =>
				(null, null),
		};
	}

	public override void OnGUI(
		Rect position,
		SerializedProperty property,
		GUIContent label
	) {
		Type type = this.GetValueType();
		SerializedProperty value = property.FindPropertyRelative("value");
		UnityObject? assigned = EditorGUI.ObjectField(
			position,
			label,
			value.objectReferenceValue,
			typeof(UnityObject),
			true
		);
		string? error;
		(value.objectReferenceValue, error) = ReferenceGenericPD.InstanceOrNull(
			assigned,
			type
		);
		if (error != null) {
			Debug.LogError(error);
		}
	}
}
