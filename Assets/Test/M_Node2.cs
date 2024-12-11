using SugarNode;
using UnityEngine;

[NodeDefaultName("2号对话节点")]
[CreateMenu("测试对话图/2号节点(带有两个分支)")]
[NodeWidth(300)]
[NodeColor(0.8f,0,0)]
public class M_Node2 : BaseNode
{
    public int num = 123;
    public string test_Tips = "日奈";
    public InputPort<int> input = new InputPort<int>("Input");
    public OutputPort defaultOutput = new OutputPort("默认分支");
    public OutputPort<string> output1 = new OutputPort<string>("1号分支");
    public OutputPort<string> output2 = new OutputPort<string>("2号分支");
    protected override void Init()
    {
        InitPort(input);
        InitPort(defaultOutput);
        InitPort(output1);
        InitPort(output2);
    }
    public override object GetValue(OutputPort port)
    {
        if (port == defaultOutput)
            return "你获取了默认分支的值";
        else if (port == output1)
            return num.ToString();
        else if (port == output2)
            return (num + 10086).ToString();
        else return string.Empty;
    }
    protected override string ShowTips() => "sensei:ᕕ(◠ڼ◠)ᕗ\n老婆:" + test_Tips;
}
