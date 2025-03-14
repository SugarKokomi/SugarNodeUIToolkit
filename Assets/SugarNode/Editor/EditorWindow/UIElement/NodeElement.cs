using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using PortElement = UnityEditor.Experimental.GraphView.Port;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Reflection;


#if ODIN_INSPECTOR
using Sirenix.OdinInspector.Editor;
#endif

namespace SugarNode.Editor
{
    public class NodeElement : UnityEditor.Experimental.GraphView.Node
    {
        // public static Action<NodeElement> OnNodeSelected;
        internal Node node;
        internal Dictionary<PortElement, string> uiPairsPort;
        internal static Dictionary<string, PortElement> uiPairsPortReverse;//前者的反向字典
        VisualElement content;
        // StyleSheet styleSheet;
        public NodeElement(Node node) :
            base(NodeEditorUtility.Instance.GetSugarNodeRootFolder() +
            "/Editor/Resources/NodeElement_TreeAsset.uxml")
        {
            this.node = node;
            this.title = node.name;
            this.viewDataKey = node.guid;

            style.left = node.gridPos.x;
            style.top = node.gridPos.y;

            uiPairsPort = new Dictionary<PortElement, string>();
            uiPairsPortReverse ??= new Dictionary<string, PortElement>();

            // InitPort();
            SetTipsText();
            SetWidth();
            SetColor();

            content = this.Q("Contents");
            DrawFullGUI();
        }
        private static Type GetPortType(Port port)
        {
            Type objectType = port.GetType();
            // 检查该类型是否是泛型类 Port<T> 的实例
            if (objectType.IsGenericType)
            {
                Type tType = objectType.GetGenericTypeDefinition();
                if (tType == typeof(InputPort<>) || tType == typeof(OutputPort<>))
                    return objectType.GetGenericArguments()[0];
                else return null;
            }
            else return null;
        }
        public override void SetPosition(Rect newPos)
        {
            // 将视图中节点位置设置为最新位置newPos
            base.SetPosition(newPos);
            // 将最新位置记录到运行时节点树中持久化存储
            node.gridPos.x = newPos.xMin;
            node.gridPos.y = newPos.yMin;
            EditorUtility.SetDirty(node);
        }
        public override void OnSelected()
        {
            base.OnSelected();
            // OnNodeSelected?.Invoke(this);
            // NodeEditorWindow.Instance.SetSelectionNoEvent(this.node);
            Selection.activeObject = node;
            // ShowGUI();
        }
        public override void OnUnselected()
        {
            base.OnUnselected();
            // HideGUI();

            // 定义一个泛型类
            /* Type genericType = typeof(InputPort<>);
            Debug.Log($"IsGenericType: {genericType.IsGenericType}"); // true
            Debug.Log($"IsGenericTypeDefinition: {genericType.IsGenericTypeDefinition}"); // true
            Debug.Log($"IsClass: {genericType.IsClass}"); // true

            // 定义一个泛型实例
            Type genericInstance = typeof(InputPort<int>);
            Debug.Log($"IsGenericType: {genericInstance.IsGenericType}"); // true
            Debug.Log($"IsGenericTypeDefinition: {genericInstance.IsGenericTypeDefinition}"); // false
            Debug.Log($"IsClass: {genericInstance.IsClass}"); // true

            // 非泛型类
            Type nonGenericType = typeof(string);
            Debug.Log($"IsGenericType: {nonGenericType.IsGenericType}"); // false
            Debug.Log($"IsClass: {nonGenericType.IsClass}"); // true */

        }

        private void SetTipsText()
        {
            var titleTips = this.Q<Label>("title-Tips");
            titleTips.text = node.GetTips();
        }
        private void SetWidth()
        {
            /* var root = this.Q("BackGround");
            float minWidth = NodeAttributeHandler.Instance.GetNodeWidth(node.GetType());
            root.style.minWidth = minWidth > 0 ? minWidth : StyleKeyword.Auto; */
        }
        private void SetColor()
        {
            var body = this.Q("NodeBody");
            var title = this.Q("title-label");
            var color = NodeAttributeHandler.Instance.GetNodeColor(node.GetType());

            var color_1 = color * 0.2f;
            color_1.a = 1;
            var bgColor = new StyleColor(color_1);

            var color_2 = color * 0.5f;
            color_2.a = 1;
            var titleColor = new StyleColor(color_2);


            body.style.borderTopColor = titleColor;
            body.style.borderBottomColor = titleColor;
            body.style.borderLeftColor = titleColor;
            body.style.borderRightColor = titleColor;
            title.style.backgroundColor = titleColor;
            body.style.backgroundColor = bgColor;
        }
        UnityEditor.Editor editor;
        IMGUIContainer imGUI;
        private void DrawFullGUI()
        {
            void DrawNodeInspector()
            {
#if ODIN_INSPECTOR
                if (!editor)
                    editor = OdinEditor.CreateEditor(node);
#else
            if (!editor)
                editor = UnityEditor.Editor.CreateEditor(node);
#endif
                editor.OnInspectorGUI();
            }
            // imGUI = new IMGUIContainer(DrawNodeInspector);
            // content.Add(imGUI);
            content.Add(DrawPortWithVisualElement());
            content.style.display = DisplayStyle.Flex;
            inputContainer.style.display = DisplayStyle.None;
            outputContainer.style.display = DisplayStyle.None;
            // DrawPortWithVisualElement();
        }
        private void HideGUI()
        {
            // content.Remove(imGUI);
            // imGUI = null;
            // UnityEngine.Object.DestroyImmediate(editor);
            content.Clear();
            content.style.display = DisplayStyle.None;
            // inputContainer.style.display = DisplayStyle.Flex;
            // outputContainer.style.display = DisplayStyle.Flex;

        }

        SerializedObject serializedObject;
        private VisualElement DrawPortWithVisualElement()
        {
            var visualElement = new VisualElement();
            visualElement.style.flexDirection = FlexDirection.Column;
            serializedObject = new SerializedObject(node);
            var serializedProperty = serializedObject.GetIterator();
            while (true)
            {
                bool next;
                if (serializedProperty.name == "m_Script")//跳过脚本自己的序列化
                {
                    serializedProperty.NextVisible(true);
                    continue;
                }
                var propertyType = serializedProperty.serializedObject.targetObject.GetType()
                    .GetField(serializedProperty.name,
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Public |
                    System.Reflection.BindingFlags.Instance)
                    ?.FieldType;

                // 检查字段是否带有 CustomPortDrawerAttribute 属性
                bool isCustomPort = false, isCustomPortList = false;
                if (propertyType != null)
                {
                    isCustomPort = typeof(Port).IsAssignableFrom(propertyType);
                    isCustomPortList =
                        (propertyType.IsArray && typeof(Port).IsAssignableFrom(propertyType.GetElementType())) ||//是不是Port[]
                        (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(List<>) &&//是不是List<>
                            typeof(Port).IsAssignableFrom(propertyType.GetGenericArguments()[0]));//是不是List<T>
                }

                if (isCustomPort)
                {
                    visualElement.Add(DrawPort(serializedProperty));
                    next = serializedProperty.NextVisible(false);
                }
                else if(isCustomPortList)
                {
                    
                    var field = new PropertyField(serializedProperty);
                    field.BindProperty(serializedProperty);
                    visualElement.Add(field);
                    next = serializedProperty.NextVisible(false);
                }
                else
                {
                    var field = new PropertyField(serializedProperty);
                    field.BindProperty(serializedProperty);
                    field.style.paddingLeft = 30;
                    field.style.paddingRight = 30;
                    visualElement.Add(field);
                    next = serializedProperty.NextVisible(true);
                }
                if (!next) break;
            }
            MarkDirtyRepaint();
            return visualElement;
        }
        private VisualElement DrawPort(SerializedProperty serializedProperty)
        {
            var field = new PropertyField(serializedProperty);
            field.BindProperty(serializedProperty);
            return field;
        }
    }
}