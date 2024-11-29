using SugarNode;
using UnityEngine;

public class M_Node1 : SugarNode.Node
{
    public string str = "Hello World!";
    public InputPort<string> input;
    public OutputPort<string> output;
    protected override void Init()
    {
        input = new InputPort<string>();
        output = new OutputPort<string>();

        input.GetValue = () => str;
        output.GetValue = () => str + 10086;

        AddPort(input);
        AddPort(output);
    }
}