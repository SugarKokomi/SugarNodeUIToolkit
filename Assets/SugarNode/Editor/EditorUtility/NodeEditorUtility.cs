
using System.Linq;
using UnityEditor;

namespace SugarNode.Editor
{
    internal class NodeEditorUtility
    {
        static NodeEditorUtility m_instance;
        internal static NodeEditorUtility Instance { get { m_instance ??= new NodeEditorUtility(); return m_instance; } set { m_instance = value; } }
        string rootFolder;
        internal string GetSugarNodeRootFolder()
        {
            if (string.IsNullOrEmpty(rootFolder))
            {
                string folderGuid = AssetDatabase.FindAssets("t:Folder " + nameof(SugarNode)).FirstOrDefault();
                rootFolder = AssetDatabase.GUIDToAssetPath(folderGuid);
            }
            return rootFolder;
        }
    }
}