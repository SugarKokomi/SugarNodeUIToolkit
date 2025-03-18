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
            var container = new Foldout();
            container.text = property.displayName;
            var listContainer = new VisualElement();
            listContainer.style.flexDirection = FlexDirection.Column;
            container.Add(listContainer);
            var buttonGroup = new VisualElement();
            buttonGroup.style.flexDirection = FlexDirection.Row;
            container.Add(buttonGroup);

            var list = property.FindPropertyRelative("_serializedList");
            UpdateListUI(listContainer,list);

            var addButton = new Button();
            addButton.text = "+";
            var removeButton = new Button();
            removeButton.text = "-";
            addButton.RegisterCallback<ClickEvent>(evt =>
            {
                list.InsertArrayElementAtIndex(list.arraySize);
                list.serializedObject.ApplyModifiedProperties();
                UpdateListUI(listContainer, list);
            });
            removeButton.RegisterCallback<ClickEvent>(evt =>
            {
                if (list.arraySize > 0)
                {
                    list.DeleteArrayElementAtIndex(list.arraySize - 1);
                    list.serializedObject.ApplyModifiedProperties();
                    UpdateListUI(listContainer, list);
                }
            });
            buttonGroup.Add(addButton);
            buttonGroup.Add(removeButton);
            container.Add(buttonGroup);
            return container;
        }   
        private void UpdateListUI(VisualElement container, SerializedProperty list)
        {
            container.Clear(); // 清除现有元素
            foreach(SerializedProperty listItem in list)
            {
                PropertyField propertyField = new PropertyField(listItem);
                // propertyField.BindProperty(listItem);
                container.Add(propertyField);
            }
        }
    }
}   
