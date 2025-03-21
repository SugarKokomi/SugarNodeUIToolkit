using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace SugarNode.Editor
{
    [CustomPropertyDrawer(typeof(ListPort<>))]
    public class ListPortElement : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // EditorGUI.PropertyField(position, property, label);
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => 0;
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var targetList = property.FindPropertyRelative("_serializedList");

            var container = new Foldout();
            container.text = property.displayName;
            {
                var listContainer = new VisualElement();
                listContainer.style.flexDirection = FlexDirection.Column;
                container.Add(listContainer);
                {
                    DrawAll(listContainer, targetList);
                }

                var buttonGroup = new VisualElement();
                buttonGroup.style.flexDirection = FlexDirection.Row;
                container.Add(buttonGroup);
                {
                    var addButton = new Button();
                    addButton.text = "+";
                    addButton.RegisterCallback<ClickEvent>(_ => AddPort(listContainer, targetList));
                    buttonGroup.Add(addButton);

                    var removeButton = new Button();
                    removeButton.text = "-";
                    removeButton.RegisterCallback<ClickEvent>(_ => RemovePort(listContainer, targetList));
                    buttonGroup.Add(removeButton);
                }
            }
            return container;
        }
        static void DrawAll(VisualElement container, SerializedProperty list)
        {
            container.Clear();
            foreach (SerializedProperty listItem in list)
                container.Add(new PropertyField(listItem));
        }
        static void AddPort(VisualElement container, SerializedProperty list)
        {
            list.InsertArrayElementAtIndex(list.arraySize);
            list.serializedObject.ApplyModifiedProperties();
            container.Add(new PropertyField(list.GetArrayElementAtIndex(list.arraySize - 1)));
        }
        static void RemovePort(VisualElement container, SerializedProperty list)
        {
            if (list.arraySize > 0)
            {
                list.DeleteArrayElementAtIndex(list.arraySize - 1);
                list.serializedObject.ApplyModifiedProperties();
                container.RemoveAt(container.childCount - 1);
            }
        }
    }
}
