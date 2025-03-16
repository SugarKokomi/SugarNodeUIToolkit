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
    internal abstract class PortDrawer : PropertyDrawer
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
            // EditorGUI.PropertyField(position, property, label, true);
        }
        /* public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float defaultHeight = base.GetPropertyHeight(property, label);
            return property.CountInProperty() * defaultHeight;//无法正确处理[Space]和[Multiline]的高度
        } */
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => 0;
        public abstract override VisualElement CreatePropertyGUI(SerializedProperty property);
        protected string GetPropertyDisplayName(SerializedProperty property)
        {
            string portName = property.FindPropertyRelative("portName").stringValue;
            return string.IsNullOrEmpty(portName) ? property.displayName : portName;
        }
        protected VisualElement CreatePropertyValue(SerializedProperty property)
        {
                    // var outputPort = property.FindPropertyRelative("m_connectionsGUID");
                    // if (outputPort != null && outputPort.arraySize > 0) return null;
            PropertyField propertyField = new PropertyField(property);
            propertyField.BindProperty(property);
            // propertyField.style.flexGrow = 1;
            return propertyField;
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
            port.portName = GetPropertyDisplayName(property);
            return port;
        }
        protected static void SetBorder(VisualElement element, Color color, float width, float radius)
        {
            element.style.borderTopLeftRadius = radius;
            element.style.borderTopRightRadius = radius;
            element.style.borderBottomLeftRadius = radius;
            element.style.borderBottomRightRadius = radius;

            element.style.borderTopWidth = width;
            element.style.borderBottomWidth = width;
            element.style.borderLeftWidth = width;
            element.style.borderRightWidth = width;

            element.style.borderTopColor = color;
            element.style.borderBottomColor = color;
            element.style.borderLeftColor = color;
            element.style.borderRightColor = color;
        }
    }
    [CustomPropertyDrawer(typeof(InputPort), true)]
    internal class InputPortDrawer : PortDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            portValueType = GetPortValueType();
            var container = new VisualElement();
            // container.style.flexDirection = FlexDirection.Row;
            container.Add(CreatePropertyPort(property, Direction.Input));
            container.Add(CreatePropertyValue(property));
            SetBorder(container, new Color(0.5f, 0.5f, 0.5f), 1, 2);
            return container;
        }
    }
    [CustomPropertyDrawer(typeof(OutputPort), true)]
    internal class OutputPortDrawer : PortDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            Debug.Log(property.displayName);
            portValueType = GetPortValueType();
            var container = new VisualElement();
            // container.style.flexDirection = FlexDirection.RowReverse;//反向水平排列
            container.Add(CreatePropertyPort(property, Direction.Output));
            container.Add(CreatePropertyValue(property));
            SetBorder(container, new Color(0.5f, 0.5f, 0.5f), 1, 2);
            return container;
        }
    }
}