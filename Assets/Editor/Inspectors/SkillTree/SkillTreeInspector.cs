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

            foreach (var node in nodes)
            {
                if (!node.Tree)
                {
                    node.Tree = target as UI.Components.SkillTree;
                }
                var rt = node.GetComponent<RectTransform>();
                var nodeSize = rt.sizeDelta / 2;
                var pos = camera.WorldToScreenPoint(node.transform.position);
                var basePos = new Vector2(pos.x, camera.pixelHeight - pos.y);
                nodeSize.y = -nodeSize.y;
                
                Handles.color = Color.red;
                foreach (var connection in node.Connections)
                {
                    if (_calculatedNodes.Contains(connection))
                    {
                        continue;
                    }
                    Handles.DrawLine(rt.position, connection.GetComponent<RectTransform>().position);
                }

                _calculatedNodes.Add(node);
                
                Handles.color = Color.white;
                Handles.DrawSolidDisc(rt.position, new Vector3(0, 0, 1), nodeSize.x);
                Handles.color = Color.black;
                Handles.DrawSolidDisc(rt.position, new Vector3(0, 0, 1), nodeSize.x * 0.75f);
                Handles.color = Color.gray;
                EditorGUI.BeginChangeCheck();
                var offset = new Vector3(-nodeSize.x - 6, nodeSize.x / 2 + 5);
                EditorSnapSettings.snapEnabled = true;
                var newPos = Handles.FreeMoveHandle(rt.position + offset, 20, Vector3.zero, DoMoveHandle);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(rt, "Move Skill Node");
                    var delta = newPos - rt.position - offset;
                    rt.position += delta;
                    rt.position = Snapping.Snap(rt.position, new Vector3(10, 10, 10));
                }
                
                var normalStyle = new GUIStyle("ButtonMid");
                Handles.BeginGUI();
                if (node != _attachNode && !node.HasConnection(_attachNode))
                {
                    if (_attachNode)
                    {
                        GUI.backgroundColor = new Color(1, 1, 0);
                    }

                    if (GUI.Button(new Rect(new Vector2(basePos.x + (nodeSize.x / 2 - 6) * zoom, basePos.y - (nodeSize.x - 6) * zoom),
                            new Vector2(10, 10) * zoom),
                            EditorGUIUtility.Load("Link.png") as Texture, normalStyle))
                    {
                        if (_attachNode)
                        {
                            Undo.RecordObjects(new Object[]{node, _attachNode}, "Link Skill Nodes");
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
                else if (node != _attachNode)
                {
                    GUI.backgroundColor = new Color(1, 0, 0);
                    
                    if (_attachNode)
                    {
                        if (GUI.Button(new Rect(new Vector2(basePos.x + (nodeSize.x / 2 - 6) * zoom, basePos.y - (nodeSize.x - 6) * zoom),
                                    new Vector2(10, 10) * zoom),
                                EditorGUIUtility.Load("Cross.png") as Texture, normalStyle))
                        {
                            Undo.RecordObjects(new Object[]{node, _attachNode}, "Unlink Skill Nodes");
                            _attachNode.Detach(node);
                            node.Detach(_attachNode);
                            _attachNode = null;
                        }
                    }
                }
                Handles.EndGUI();
            }
        }

        private void DoMoveHandle(int controlID, Vector3 position, Quaternion rotation, float size, EventType eventType)
        {
            switch (eventType)
            {
                case EventType.MouseMove:
                case EventType.Layout:
                    HandleUtility.AddControl(controlID, HandleUtility.DistanceToRectangle(position, rotation, size));
                    break;
                case EventType.Repaint:
                    Graphics.DrawTexture(new Rect(position, Vector2.one * size), EditorGUIUtility.Load("MoveTool.png") as Texture);
                    break;
            }
        }


        private void NodeUI()
        {
        }
    }
}
