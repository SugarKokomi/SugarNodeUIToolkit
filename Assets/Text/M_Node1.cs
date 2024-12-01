using SugarNode;
using UnityEngine;
public abstract class BaseNode : SugarNode.Node { }
[NodeDefaultName("1号对话节点")]
[CreateMenu("测试对话图/1号节点")]
public class M_Node1 : BaseNode
{
    public string str = "Hello World!";
    public InputPort<string> input = new InputPort<string>("输入", "Hello World!");
    public OutputPort<string> output = new OutputPort<string>("输出", "Hello World!");
    protected override void Init()
    {
        InitPort(input);
        InitPort(output);
    }
    protected override string ShowTips() => str;
}