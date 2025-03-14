using System;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
using PortElement = UnityEditor.Experimental.GraphView.Port;
using UnityEngine;
using System.Collections.Generic;

namespace SugarNode.Editor
{
    public abstract class PortDrawer : PropertyDrawer
    {
        protected Type portValueType;
        protected bool foldout = false;
        /// <summary> 获取所附加的字段的输入值的泛型类型 </summary>
        protected Type GetPortValueType()
        {
            //fieldInfo.FieldType一定与Port相关。（以InputPort为例，不是InputPort就是InputPort<T>，抑或是InputPort[]或者InputPort<T>[]（List同理））
            Type portType = fieldInfo.FieldType;
            if (!typeof(Port).IsAssignableFrom(portType))
                portType = GetPortType(portType);
            // 检查该类型是否是泛型类 Port<T> 的实例
            if (portType.IsGenericType)
            {
                Type tType = portType.GetGenericTypeDefinition();
                if (tType == typeof(InputPort<>) || tType == typeof(OutputPort<>))
                    return portType.GetGenericArguments()[0];
                else return null;
            }
            else return null;
        }
        protected static Type GetPortType(Type type)
        {
            // 检查是否是数组
            if (type.IsArray)
                return type.GetElementType();
            // 检查是否是 List<T>
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
                return type.GetGenericArguments()[0];
            else return type;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // 使用字段的显示名称或者实例名称作为PortName
            /* string portName = property.FindPropertyRelative("portName").stringValue;
            portName = string.IsNullOrEmpty(portName) ? property.displayName : portName;
            var valueProperty = property.FindPropertyRelative("defaultValue");
            var guid = property.FindPropertyRelative("m_guid").stringValue;

            if (valueProperty != null)
            {
                EditorGUI.BeginProperty(position, new GUIContent(portName), property);
                foldout = EditorGUI.Foldout(position, foldout, $"{portName} GUID:{guid}");
                if (foldout)
                {
                    EditorGUI.PropertyField(position, valueProperty, GUIContent.none, true);
                }
                EditorGUI.EndProperty();
            }
            else EditorGUI.PropertyField(position, property, new GUIContent(portName), true); */
        }
        /* public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return property.CountInProperty() * 20;
        } */
        public abstract override VisualElement CreatePropertyGUI(SerializedProperty property);
        protected Label CreatePropertyName(SerializedProperty property)
        {
            string portName = property.FindPropertyRelative("portName").stringValue;
            Label label = new Label(string.IsNullOrEmpty(portName) ? property.displayName : portName);
            label.style.unityTextAlign = TextAnchor.MiddleCenter;
            return label;
        }
        protected PropertyField CreatePropertyValue(SerializedProperty property)
        {
            var defaultValue = property.FindPropertyRelative("defaultValue");
            if (defaultValue != null)
            {
                PropertyField propertyField = new PropertyField(defaultValue, string.Empty);
                propertyField.BindProperty(defaultValue);
                propertyField.style.flexGrow = 1;
                return propertyField;
            }
            return null;
        }
        protected PortElement CreatePropertyPort(SerializedProperty property, Direction direction)
        {
            int maxConnectionCount = property.FindPropertyRelative("m_maxConnectionCount").intValue;
            string guid = property.FindPropertyRelative("m_guid").stringValue;
            var port = NodePortElement.Create(
                direction,
                maxConnectionCount,
                guid,
                portValueType);
            port.portName = string.Empty;
            return port;
        }
    }
    [CustomPropertyDrawer(typeof(InputPort), true)]
    public class InputPortDrawer : PortDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            portValueType = GetPortValueType();
            var container = new VisualElement();
            container.style.flexDirection = FlexDirection.Row;
            container.Add(CreatePropertyPort(property, Direction.Input));
            container.Add(CreatePropertyName(property));
            container.Add(CreatePropertyValue(property));
            return container;
        }
    }
    [CustomPropertyDrawer(typeof(OutputPort), true)]
    public class OutputPortDrawer : PortDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            portValueType = GetPortValueType();
            var container = new VisualElement();
            container.style.flexDirection = FlexDirection.RowReverse;//反向水平排列
            container.Add(CreatePropertyPort(property, Direction.Output));
            container.Add(CreatePropertyName(property));
            return container;
        }
    }
}