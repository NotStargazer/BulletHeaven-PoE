using Gameplay.Enemies;
using UnityEditor;
using UnityEngine.UIElements;

namespace Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(FlexibleStateMachine.StateType<IState>))]
    public class StateTypeIStatePropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement();
            EditorStateMachineUtility.CreateTypeSelector<IState>(root, property, "State");
            return root;
        }
    }
}