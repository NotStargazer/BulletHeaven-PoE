using System.Collections.Generic;
using UI.Elements.Skill_Tree;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Editor.Inspectors.SkillTree
{
    [CustomEditor(typeof(UI.Components.SkillTree))]
    public class SkillTreeInspector : UnityEditor.Editor
    {
        private SkillNode _attachNode;
        private SkillNode _selectedNode;
        private readonly HashSet<SkillNode> _calculatedNodes = new();

        private void DrawSelected()
        {
            var camera = SceneView.currentDrawingSceneView.camera;
            var zoom = camera.pixelHeight / camera.orthographicSize;
            var rt = _selectedNode.GetComponent<RectTransform>();
            var nodeSize = rt.sizeDelta / 2;
            var pos = camera.WorldToScreenPoint(_selectedNode.transform.position);
            var basePos = new Vector2(pos.x, camera.pixelHeight - pos.y);
            nodeSize.y = -nodeSize.y;
            var btSize = new Vector2(nodeSize.x, nodeSize.x) * zoom;

            EditorGUI.BeginChangeCheck();
            var offset = new Vector3(-nodeSize.x - nodeSize.x * 1.5f, nodeSize.x);
            EditorSnapSettings.snapEnabled = true;
            var newPos = Handles.FreeMoveHandle(rt.position + offset, nodeSize.x * 2, Vector3.zero, DoMoveHandle);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(rt, "Move Skill Node");
                var delta = newPos - rt.position - offset;
                rt.position += delta;
                rt.position = Snapping.Snap(rt.position, new Vector3(10, 10, 10));
            }

            if (_selectedNode.NodeType == SkillType.Root)
            {
                return;
            }
            Handles.BeginGUI();
            var normalStyle = new GUIStyle("ButtonMid");
            GUI.backgroundColor = new Color(1, 0, 0);
            if (GUI.Button(new Rect(new Vector2(basePos.x + nodeSize.x / 2 * zoom, basePos.y + nodeSize.x / 2 * zoom),
                        btSize),
                    EditorGUIUtility.Load("Cross.png") as Texture, normalStyle))
            {
                Undo.RecordObject(_selectedNode.gameObject, "Delete Node");
                _selectedNode.RemoveConnections();
                DestroyImmediate(_selectedNode.gameObject);
                _selectedNode = null;
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GUILayout.Space(10);
            if (_selectedNode)
            {
                EditorGUILayout.BeginVertical(new GUIStyle("IN ThumbnailShadow"));
                EditorGUILayout.LabelField("Selected Node", new GUIStyle("LargeBoldLabel"));
                EditorGUI.BeginChangeCheck();
                var nodeType = EditorGUILayout.EnumPopup("Node Type", _selectedNode.NodeType);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(_selectedNode, "Change Node Type");
                    _selectedNode.SetSkillType((SkillType)nodeType);
                }
                if (GUILayout.Button("Add Connecting Node"))
                {
                    var node = Instantiate(_selectedNode);
                    node.SetSkillType(_selectedNode.NodeType == SkillType.Root ? SkillType.Travel : _selectedNode.NodeType);
                    var transform = node.GetComponent<RectTransform>();
                    ((UI.Components.SkillTree)target).AddNodeToTransform(transform);
                    transform.anchoredPosition = _selectedNode.GetComponent<RectTransform>().anchoredPosition + new Vector2(10, 10);
                    _selectedNode.Attach(node);
                    node.Attach(_selectedNode);
                    _selectedNode = node;
                }
                GUI.backgroundColor = Color.red;
                GUILayout.Space(10);
                if (GUILayout.Button("Remove Connections"))
                {
                    Undo.RecordObject(_selectedNode, "Remove All Connections");
                    _selectedNode.RemoveConnections();
                }
                EditorGUILayout.EndVertical();
            }
            GUILayout.Space(10);
            GUI.backgroundColor = Color.white;
            if (GUILayout.Button("Refresh all nodes"))
            {
                var scene = PrefabStageUtility.GetCurrentPrefabStage().prefabContentsRoot;
                var nodes = scene.GetComponentsInChildren<SkillNode>();

                foreach (var node in nodes)
                {
                    node.Tree = (UI.Components.SkillTree)target;
                    node.SetSkillType(node.NodeType);
                }
            }
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

                _calculatedNodes.Add(node);
                if (_selectedNode != node && !_attachNode)
                {
                    Handles.color = Color.yellow;
                    if (Handles.Button(rt.position, Quaternion.identity, nodeSize.x, nodeSize.x, DoSelectionHandle))
                    {
                        _selectedNode = node;
                    }
                }

                Handles.color = Color.gray;
                var normalStyle = new GUIStyle("ButtonMid");
                var linkTex = EditorGUIUtility.Load("Link.png") as Texture;
                var btSize = new Vector2(nodeSize.x, nodeSize.x) * zoom;

                if (_attachNode == node)
                {
                    continue;
                }

                Handles.BeginGUI();
                if (_attachNode)
                {
                    GUI.backgroundColor = new Color(2, 2, 0);
                }

                if (_attachNode && node.HasConnection(_attachNode))
                {
                    GUI.backgroundColor = new Color(1, 0, 0);
                }
                
                if (GUI.Button(new Rect(new Vector2(basePos.x + nodeSize.x / 2 * zoom, basePos.y - btSize.y - nodeSize.x / 2 * zoom),
                        btSize), linkTex, normalStyle))
                {
                    if (_attachNode && node.HasConnection(_attachNode))
                    {
                        Undo.RecordObjects(new Object[] { node, _attachNode }, "Unlink Skill Nodes");
                        _attachNode.Detach(node);
                        node.Detach(_attachNode);
                        _attachNode = null;
                    }
                    else if (_attachNode)
                    {
                        Undo.RecordObjects(new Object[] { node, _attachNode }, "Link Skill Nodes");
                        _attachNode.Attach(node);
                        node.Attach(_attachNode);
                        _attachNode = null;
                    }
                    else
                    {
                        _attachNode = node;
                    }
                }

                Handles.EndGUI();
            }

            if (_selectedNode && !_attachNode)
            {
                DrawSelected();
            }
        }

        private void DoSelectionHandle(int controlID, Vector3 position, Quaternion rotation, float size, EventType eventType)
        {
            switch (eventType)
            {
                case EventType.MouseMove:
                case EventType.Layout:
                    HandleUtility.AddControl(controlID, HandleUtility.DistanceToCircle(position, size));
                    break;
                case EventType.Repaint:
                    Handles.DrawWireDisc(position, new Vector3(0, 0, 1), size, 5);
                    break;
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
    }
}
