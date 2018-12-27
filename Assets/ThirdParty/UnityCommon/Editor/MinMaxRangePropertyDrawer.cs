using UnityEditor;
using UnityEngine;

namespace UnityCommon
{
    [CustomPropertyDrawer(typeof(MinMaxRangeAttribute))]
    public class MinMaxRangePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.Vector2)
            {
                var attr = attribute as MinMaxRangeAttribute;
                var range = property.vector2Value;
                var min = range.x;
                var max = range.y;
                EditorGUI.BeginChangeCheck();
                EditorGUI.MinMaxSlider(position, label, ref min, ref max, attr.Min, attr.Max);
                if (EditorGUI.EndChangeCheck())
                {
                    range.x = min;
                    range.y = max;
                    property.vector2Value = range;
                }
            }
            else EditorGUI.LabelField(position, label, "Use only with Vector2");
        }
    }
}
