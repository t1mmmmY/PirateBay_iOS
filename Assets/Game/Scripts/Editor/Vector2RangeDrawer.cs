using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomPropertyDrawer(typeof(Vector2Range))]
public class Vector2RangeDrawer : PropertyDrawer 
{
	public override float GetPropertyHeight (SerializedProperty  prop, GUIContent  label)
	{
		float extraHeight = 56.0f;  
		return base.GetPropertyHeight(prop, label) + extraHeight;
	}

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		Vector2Range range = attribute as Vector2Range;

		//EditorGUI.IntSlider(new Rect(position.x, position.y + 20, position.width, position.height), property, range.xMin, range.xMax, label);
		EditorGUI.LabelField(new Rect(position.x, position.y, position.width, position.height / 4), "Horizontal Cells count");

		int newX = EditorGUI.IntSlider(new Rect(position.x, position.y + position.height / 4, position.width, position.height / 4), 
		                               (int)property.vector2Value.x, range.xMin, range.xMax);

		EditorGUI.LabelField(new Rect(position.x, position.y + position.height / 2, position.width, position.height / 4), "Vertical Cells count");

		int newY = EditorGUI.IntSlider(new Rect(position.x, position.y + position.height * 3 / 4, position.width, position.height / 4), 
		                               (int)property.vector2Value.y, range.yMin, range.yMax);

		Vector2 newVector2 = new Vector2(newX, newY);
		property.vector2Value = newVector2;

//		if (EditorGUI.EndChangeCheck()) {
//			prop.vector2Value = newV2;
//		}
		//EditorGUI.Slider(position, property, range.xMin, range.xMax, label);
	}
}
