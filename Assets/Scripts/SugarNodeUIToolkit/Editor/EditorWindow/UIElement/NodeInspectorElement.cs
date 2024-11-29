using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace SugarNode.Editor
{
    public class NodeInspectorElement : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<NodeInspectorElement, UxmlTraits> { }
        UnityEditor.Editor editor;
        internal void UpdateSelection(NodeElement nodeElement)
        {
            Clear();
            UnityEngine.Object.DestroyImmediate(editor);
            editor = UnityEditor.Editor.CreateEditor(nodeElement.node);
            Add(new IMGUIContainer(DrawNodeInspector));
        }
        void DrawNodeInspector()
        {
            if (editor.target)
                editor.OnInspectorGUI();
        }
    }
}