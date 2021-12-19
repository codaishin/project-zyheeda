using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(WAnimator))]
public class WAnimatorDrawer : PropertyDrawer
{
	public override void OnGUI(
		Rect rect,
		SerializedProperty property,
		GUIContent label
	) {
		SerializedProperty animator = property.FindPropertyRelative("animator");

		EditorGUI.BeginProperty(rect, label, property);
		EditorGUI.PropertyField(rect, animator, label);
		EditorGUI.EndProperty();
	}
}
