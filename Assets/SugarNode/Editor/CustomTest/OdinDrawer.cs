/* using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

// 自定义类
[System.Serializable]
public class MyCustomClass
{
    public int intValue;
    public string stringValue;
}

// 自定义绘制器
public class MyCustomClassDrawer : OdinValueDrawer<MyCustomClass>
{
    protected override void DrawPropertyLayout(GUIContent label)
    {
        // 获取当前值
        MyCustomClass value = this.ValueEntry.SmartValue;

        // 绘制字段
        GUILayout.BeginVertical("box");
        {
            value.intValue = EditorGUILayout.IntField("Int Value", value.intValue);
            value.stringValue = EditorGUILayout.TextField("String Value", value.stringValue);
        }
        GUILayout.EndVertical();

        // 更新值
        this.ValueEntry.SmartValue = value;
    }
} */