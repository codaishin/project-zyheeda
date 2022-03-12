using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

using UnityObject = UnityEngine.Object;


[CustomPropertyDrawer(typeof(Reference<>))]
public class ReferenceGenericPropertyDrawer : PropertyDrawer
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

	private static Component GetComponent(GameObject obj, Type type) {
		Component? component = obj.GetComponent(type);
		return component ?? throw new ArgumentException(
			$"{obj.name}: must have a component that implements '{type}'"
		);
	}

	private static UnityObject ObjectIfTypeMatches(UnityObject obj, Type type) {
		return type.IsAssignableFrom(obj.GetType())
			? obj
			: throw new ArgumentException(
				$"{obj.name}: must implement '{type}'"
			);
	}

	private static UnityObject? InstanceOrNull(UnityObject? assigned, Type type) {
		return assigned switch {
			GameObject obj =>
				ReferenceGenericPropertyDrawer.GetComponent(obj, type),
			UnityObject obj =>
				ReferenceGenericPropertyDrawer.ObjectIfTypeMatches(obj, type),
			null =>
				null,
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
		value.objectReferenceValue = ReferenceGenericPropertyDrawer.InstanceOrNull(
			assigned,
			type
		);
	}
}
