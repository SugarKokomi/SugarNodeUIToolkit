using UnityEngine;
using UnityEditor;

public class MyCustomWindow : EditorWindow
{
    private MyScriptableObject myScriptableObject;

    [MenuItem("Window/My Custom Window")]
    public static void ShowWindow()
    {
        GetWindow<MyCustomWindow>("My Custom Window");
    }

    private void OnEnable()
    {
        // 初始化 ScriptableObject
        myScriptableObject = ScriptableObject.CreateInstance<MyScriptableObject>();
    }

    private void OnGUI()
    {
        if (myScriptableObject == null)
        {
            return;
        }

        // 绘制 ScriptableObject 的 Inspector 界面
        Editor editor = Editor.CreateEditor(myScriptableObject);
        editor.DrawDefaultInspector();
    }
}