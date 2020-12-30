using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(Reference))]
public class GameObjectWrapperDrawer : PropertyDrawer
{
	private static
	(bool, bool) State(in SerializedProperty a, in SerializedProperty b)
	{
		if (a.objectReferenceValue) return (true, false);
		if (b.objectReferenceValue) return (false, true);
		return (true, true);
	}

	public override
	float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		property.isExpanded = false;
		return EditorGUI.GetPropertyHeight(property, label) * 2;
	}

	public override
	void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
	{
		SerializedProperty prObj = property.FindPropertyRelative("gameObject");
		SerializedProperty prRef = property.FindPropertyRelative("referenceSO");
		string text = label.text;
		(bool stObj, bool stRef) = GameObjectWrapperDrawer.State(prObj, prRef);

		rect = new Rect(rect.x, rect.y, rect.width, rect.height / 2);
		GUI.enabled = stObj;
		EditorGUI.PropertyField(rect, prObj, new GUIContent($"{text} Object"));
		rect = new Rect(rect.x, rect.y + rect.height ,rect.width, rect.height);
		GUI.enabled = stRef;
		EditorGUI.PropertyField(rect, prRef, new GUIContent($"{text} Reference"));
		GUI.enabled = true;
	}
}
