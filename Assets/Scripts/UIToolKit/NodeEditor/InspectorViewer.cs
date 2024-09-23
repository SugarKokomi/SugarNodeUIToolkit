using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;

public class InspectorViewer : VisualElement
{
    public new class UxmlFactory : UxmlFactory<InspectorViewer,VisualElement.UxmlTraits>{}
    Editor editor;
    public InspectorViewer(){

    }
    internal void UpdateSelection(NodeView nodeView ){
        Clear();
        UnityEngine.Object.DestroyImmediate(editor);
        editor = Editor.CreateEditor(nodeView.node);
        IMGUIContainer container = new IMGUIContainer(() => { 
            if(editor.target){
            editor.OnInspectorGUI();
            }
        });
        Add(container);
    }   
}
