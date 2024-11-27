using UnityEngine;

[CreateAssetMenu(fileName = "NewMyScriptableObject", menuName = "MyScriptableObject")]
public class MyScriptableObject : ScriptableObject
{
    public string myString;
    public int myInt;
    public float myFloat;
}