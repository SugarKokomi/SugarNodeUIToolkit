using SugarNode;
using UnityEngine;

public class M_Node1 : SugarNode.Node
{
    public string str = "Hello World!";
    public InputPort<string> input = new InputPort<string>();
    public OutputPort<string> output = new OutputPort<string>();
    protected override void Init()
    {
        input.GetValue = () => str;
        output.GetValue = () => str + 10086;

        InitPort(input);
        InitPort(output);
    }
}