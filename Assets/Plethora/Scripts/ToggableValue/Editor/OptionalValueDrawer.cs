#if UNITY_EDITOR
namespace PlethoraV2.Editor
{
    using PlethoraV2.Values;
    using UnityEditor;
    using UnityEngine;

    [CustomPropertyDrawer(typeof(ToggableValue<>))]
    public class ToggableValueDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var valueProperty = property.FindPropertyRelative("value");
            return EditorGUI.GetPropertyHeight(valueProperty);
        }
        public override void OnGUI(
            Rect position,
            SerializedProperty property,
            GUIContent label
            )
        {
            var valueProperty = property.FindPropertyRelative("value");
            var enableProperty = property.FindPropertyRelative("enabled");

            position.width -= 24;
            EditorGUI.BeginDisabledGroup(!enableProperty.boolValue);
            EditorGUI.PropertyField(position, valueProperty, label, true);
            EditorGUI.EndDisabledGroup();


            position.x += position.width + 24;
            position.width = position.height = EditorGUI.GetPropertyHeight(enableProperty);
            position.x -= position.width;


            EditorGUI.PropertyField(position, enableProperty, GUIContent.none);
        }
    }
}
#endif