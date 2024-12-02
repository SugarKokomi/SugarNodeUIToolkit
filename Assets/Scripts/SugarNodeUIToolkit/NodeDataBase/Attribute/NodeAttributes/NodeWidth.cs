using System;
namespace SugarNode
{
    /// <summary> 用于修改节点的宽度 </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class NodeWidthAttribute : Attribute
    {
        public float width;
        /// <summary>
        /// 在网格空间下指定节点的宽度
        /// </summary>
        /// <param name="width">1代表宽为1个网格（粗稀线）</param>
        public NodeWidthAttribute(float width)
        {
            this.width = width;
        }
    }
}