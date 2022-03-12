using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Reference))]
public class ReferencePropertyDrawer : PropertyDrawer
{
	private static (GameObject?, ReferenceSO?) Assign(Object assigned) {
		return assigned switch {
			null => (null, null),
			GameObject obj => (obj, null),
			ReferenceSO refSO => (null, refSO),
			_ => throw new System.ArgumentException(
				$"{assigned}: must either be a GameObject or a ReferenceSO"
			),
		};
	}

	public override
	void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		SerializedProperty prObj = property.FindPropertyRelative("gameObject");
		SerializedProperty prRef = property.FindPropertyRelative("referenceSO");
		Object value = EditorGUI.ObjectField(
			position,
			label,
			prObj.objectReferenceValue ?? prRef.objectReferenceValue,
			typeof(Object),
			true
		);

		(prObj.objectReferenceValue, prRef.objectReferenceValue) =
			ReferencePropertyDrawer.Assign(value);
	}
}
