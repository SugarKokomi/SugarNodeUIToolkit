using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor;

namespace SugarNode.Editor
{
    public class NodeElement : UnityEditor.Experimental.GraphView.Node
    {
        public Action<NodeElement> OnNodeSelected;
        public Node node;
        public Port input;//测试
        public Port output;//测试
        public TextField textField;//测试
        public NodeElement(Node node)
        {
            this.node = node;
            this.title = node.name;
            this.viewDataKey = node.guid;
            style.left = node.gridPos.x;
            style.top = node.gridPos.y;

            CreateInputPorts();
            CreateOutputPorts();
            textField = new TextField();
            mainContainer.Add(textField);
        }
        private void CreateInputPorts()
        {
            input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));

            if (input != null)
            {
                // 将端口名设置为空
                input.portName = "";
                inputContainer.Add(input);
            }
        }
        private void CreateOutputPorts()
        {
            output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(bool));
            if (output != null)
            {
                output.portName = "";
                outputContainer.Add(output);
            }
        }
        public override void SetPosition(Rect newPos)
        {
            // 将视图中节点位置设置为最新位置newPos
            base.SetPosition(newPos);
            // 将最新位置记录到运行时节点树中持久化存储
            node.gridPos.x = newPos.xMin;
            node.gridPos.y = newPos.yMin;
            EditorUtility.SetDirty(node);
        }
        public override void OnSelected()
        {
            base.OnSelected();
            Selection.activeObject = node;
        }
    }

}