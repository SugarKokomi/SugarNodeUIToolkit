using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;
using PortElement = UnityEditor.Experimental.GraphView.Port;
using System;
using System.CodeDom;

namespace SugarNode.Editor
{
    public class NodePortElement : PortElement, IEdgeConnectorListener
    {
        protected static Type GetPortType(Type objectType)
        {
            // Type objectType = fieldInfo.FieldType;
            // 检查该类型是否是泛型类 Port<T> 的实例
            if (objectType.IsGenericType)
            {
                Type tType = objectType.GetGenericTypeDefinition();
                if (tType == typeof(InputPort<>) || tType == typeof(OutputPort<>))
                    return objectType.GetGenericArguments()[0];
                else return null;
            }
            else return null;
        }
        public static NodePortElement Create(Direction portDirection, int maxConnectionCount, string attachedGUID, Type type)
        {
            var ret = new NodePortElement(
                portDirection,
                type
            );
            ret.m_EdgeConnector = new EdgeConnector<Edge>(ret);
            ret.AddManipulator(ret.m_EdgeConnector);
            ret.guid = attachedGUID;
            ret.maxConnectionCount = maxConnectionCount;
            return ret;
        }
        string guid;
        int maxConnectionCount;
        protected int MaxConnectionCount => maxConnectionCount;
        // private NodeElement nodeElement;
        // protected NodeElement NodeElement => nodeElement;
        public NodePortElement(Direction portDirection, Type type)
            : base(Orientation.Horizontal, portDirection, Capacity.Multi, type)
        {
        }
        //=================================================
        public virtual void OnDropOutsidePort(Edge edge, Vector2 position)
        {
        }
        /// <summary> 当成功创建了一条连接线时 </summary>
        public virtual void OnDrop(GraphView graphView, Edge edge)
        {
            GraphViewChange m_GraphViewChange = new GraphViewChange();
            List<Edge> m_EdgesToCreate = new List<Edge>();
            List<Edge> m_EdgesToDelete = new List<Edge>();
            m_EdgesToCreate.Add(edge);

            if (edge.input.connections.Count() >= MaxConnectionCount)
            {
                m_EdgesToDelete.Add(edge.input.connections.FirstOrDefault());
            }
            if (edge.output.connections.Count() >= MaxConnectionCount)
            {
                m_EdgesToDelete.Add(edge.output.connections.FirstOrDefault());
            }
            if (m_EdgesToDelete.Count > 0)
            {
                graphView.DeleteElements(m_EdgesToDelete);
            }

            if (graphView.graphViewChanged != null)
            {
                m_EdgesToCreate = graphView.graphViewChanged(m_GraphViewChange).edgesToCreate;
            }

            foreach (Edge item in m_EdgesToCreate)
            {
                graphView.AddElement(item);
                edge.input.Connect(item);
                edge.output.Connect(item);
            }

        }
    }
}