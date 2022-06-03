using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Routines;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ModifierData))]
public class ModifierDataPD : PropertyDrawer
{
	private static (ModifierFlags, string)[] options = new[] {
		(ModifierFlags.OnBegin, "on begin"),
		(ModifierFlags.OnBeginSubRoutine, "on begin subroutine"),
		(ModifierFlags.OnUpdateSubRoutine, "on update subroutine"),
		(ModifierFlags.OnEndSubroutine, "on end subroutine"),
		(ModifierFlags.OnEnd, "on end"),
	};
	private float baseHeight;

	private
	bool InArray() {
		return typeof(IEnumerable).IsAssignableFrom(this.fieldInfo.FieldType);
	}

	private
	bool NotFirst(string name) {
		return name != "Element 0";
	}

	private
	static
	IEnumerable<(ModifierFlags, bool)> Convert(
		ModifierFlags hook
	) {
		(ModifierFlags, bool) Convert(ModifierFlags option) {
			return (option, hook.HasFlag(option));
		}
		ModifierFlags Option((ModifierFlags, string) value) {
			var (option, _) = value;
			return option;
		}

		return ModifierDataPD.options.Select(Option).Select(Convert);
	}

	private
	static
	ModifierFlags Convert(
		IEnumerable<(ModifierFlags, bool)> values
	) {
		ModifierFlags Concat(ModifierFlags aggregate, (ModifierFlags, bool) current) {
			var (option, value) = current;
			return value ? aggregate | option : aggregate;
		}
		return values.Aggregate((ModifierFlags)0, Concat);
	}

	private
	Rect GUIHook(SerializedProperty property, Rect pos) {
		var side = this.baseHeight;
		var left = pos.x;
		var hook = (ModifierFlags)property.enumValueFlag;
		var values = ModifierDataPD.Convert(hook);

		(ModifierFlags, bool) GUIUpdate((ModifierFlags, bool) current, int index) {
			var (option, value) = current;
			pos = new Rect(left + index * side, pos.y, side, side);
			value = EditorGUI.Toggle(pos, value);
			return (option, value);
		}

		values = values.Select(GUIUpdate);

		property.enumValueFlag = (int)ModifierDataPD.Convert(values);

		return pos;
	}

	private
	void GUIFactory(SerializedProperty property, Rect pos, float right) {
		var width = right - pos.xMax;
		pos = new Rect(pos.xMax, pos.y, width, this.baseHeight);
		EditorGUI.PropertyField(pos, property, GUIContent.none);
	}

	private
	Rect GUIPrefixLabel(GUIContent label, Rect pos) {
		if (this.InArray()) {
			return pos;
		}
		return EditorGUI.PrefixLabel(pos, label);
	}

	private
	Rect GUIConnectors(int line, Rect pos) {
		var side = this.baseHeight;
		var left = pos.x;
		var indent = 0;

		for (; indent < line; ++indent) {
			pos = new Rect(left + indent * side, pos.y, side, side);
			EditorGUI.LabelField(pos, new GUIContent("║"));
		}
		pos = new Rect(left + indent * side, pos.y, side, side);
		EditorGUI.LabelField(pos, new GUIContent("╔"));
		return pos;
	}

	private
	Rect GUIDescriptionLabel(GUIContent label, Rect pos) {
		if (this.InArray() && this.NotFirst(label.text)) {
			return pos;
		}

		var left = pos.x;
		var right = pos.xMax;
		var side = this.baseHeight;

		string Name((ModifierFlags, string) value) {
			var (_, name) = value;
			return name;
		}

		string GUIPrependConnectors(string name, int line) {
			pos = this.GUIConnectors(line, pos);
			return name;
		}

		void GUIAppendName(string name) {
			pos = new Rect(pos.xMax, pos.y, right - pos.xMax, side);
			EditorGUI.LabelField(pos, name);
			pos = new Rect(left, pos.yMax, right - left, side);
		}

		ModifierDataPD.options
			.Select(Name)
			.Select(GUIPrependConnectors)
			.ForEach(GUIAppendName);

		return pos;
	}

	private
	Rect GUILabels(GUIContent label, Rect pos) {
		pos = this.GUIPrefixLabel(label, pos);
		pos = this.GUIDescriptionLabel(label, pos);
		return pos;
	}

	public
	override
	float GetPropertyHeight(SerializedProperty property, GUIContent label) {
		this.baseHeight = base.GetPropertyHeight(property, label);

		if (this.InArray() && this.NotFirst(label.text)) {
			return this.baseHeight;
		}
		return this.baseHeight * (ModifierDataPD.options.Length + 1);
	}

	public
	override
	void OnGUI(Rect pos, SerializedProperty property, GUIContent label) {
		var right = pos.xMax;
		var hook = property.FindPropertyRelative("hook");
		var factory = property.FindPropertyRelative("factory");

		pos = this.GUILabels(label, pos);
		pos = this.GUIHook(hook, pos);
		this.GUIFactory(factory, pos, right);
	}
}
