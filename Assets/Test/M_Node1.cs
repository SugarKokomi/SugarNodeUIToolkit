using System;
using System.Collections.Generic;
using SugarNode;
using UnityEngine;
public abstract class BaseNode : SugarNode.Node { }
[NodeDefaultName("1号对话节点")]
[CreateMenu("测试对话图/1号节点")]
[NodeColor(0f,1,1)]
[NodeWidth(300)]
public class M_Node1 : BaseNode
{
    public string str = "Hello World!";
    public InputPort<string> input = new InputPort<string>();
    public OutputPort<Vector3> output = new OutputPort<Vector3>(new Vector3(1,2,3));
    // public InputPort<MyClass> input2 = new InputPort<MyClass>();
    public OutputPort<string> demoOut = new OutputPort<string>("defaultValue");
    // public List<OutputPort<string>> test;
    public List<MyClass> test2;
    public ListPort<OutputPort<int>> test = new ListPort<OutputPort<int>>()
    {
        new OutputPort<int>(4),
        new OutputPort<int>(5),
        new OutputPort<int>(6),
        new OutputPort<int>(7),
        new OutputPort<int>(8),
    };
    [Multiline(5)]
    public string sentence;
    protected override void Init()
    {
        // InitPort(input);
        // InitPort(output);
        // InitPort(input2);
    }
    protected override string ShowTips() => str;
    [Serializable]
    public class MyClass
    {
        public int INT = 123;
        public float FLOAT = 3.14f;
        [Multiline(3)]
        public string STRING = "str";
        public bool BOOLEAN = true;
    }
}