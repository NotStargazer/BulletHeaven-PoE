using System.Collections.Generic;
using UI.Elements.Skill_Tree;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Editor.Inspectors.SkillTree
{
    [CustomEditor(typeof(UI.Components.SkillTree))]
    public class SkillTreeInspector : UnityEditor.Editor
    {
        private SkillNode _attachNode;
        private readonly HashSet<SkillNode> _calculatedNodes = new();
        
        public override void OnInspectorGUI()
        {
            
        }
        
        private void OnSceneGUI()
        {
            var camera = SceneView.currentDrawingSceneView.camera;
            var zoom = camera.pixelHeight / camera.orthographicSize;
            var scene = PrefabStageUtility.GetCurrentPrefabStage().prefabContentsRoot;
            var nodes = scene.GetComponentsInChildren<SkillNode>();

            _calculatedNodes.Clear();

            Handles.BeginGUI();
            foreach (var node in nodes)
            {
                var rt = node.GetComponent<RectTransform>();
                var nodeSize = rt.sizeDelta / 4 * zoom;
                var pos = camera.WorldToScreenPoint(node.transform.position);
                var basePos = new Vector2(pos.x, camera.pixelHeight - pos.y);
                nodeSize.y = -nodeSize.y;
                var finalPos = nodeSize + basePos;
                
                Handles.color = Color.red;
                foreach (var connection in node.Connections)
                {
                    if (_calculatedNodes.Contains(connection))
                    {
                        continue;
                    }
                    var cPos = camera.WorldToScreenPoint(connection.GetComponent<RectTransform>().position);
                    var cBasePos = new Vector2(cPos.x, camera.pixelHeight - cPos.y);
                    Handles.DrawLine(basePos, cBasePos);
                }

                _calculatedNodes.Add(node);
                
                Handles.color = Color.white;
                Handles.DrawSolidDisc(basePos, new Vector3(0, 0, 1), nodeSize.x);
                Handles.color = Color.black;
                Handles.DrawSolidDisc(basePos, new Vector3(0, 0, 1), nodeSize.x * 0.75f);
                
                if (node != _attachNode && !node.HasConnection(_attachNode))
                {
                    if (GUI.Button(new Rect(new Vector2(finalPos.x - 10 * zoom, finalPos.y - 10 * zoom), new Vector2(20, 20) * zoom), EditorGUIUtility.Load("Link.png") as Texture))
                    {
                        if (_attachNode)
                        {
                            _attachNode.Attach(node);
                            node.Attach(_attachNode);
                            _attachNode = null;
                        }
                        else
                        {
                            _attachNode = node;
                        }
                    }
                }
            }
            Handles.EndGUI();
        }


        private void NodeUI()
        {
        }
    }
}
