using Gameplay.Enemies;
using UnityEditor;
using UnityEngine.UIElements;

namespace Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(FlexibleStateMachine.StateType<IExitCondition>))]
    public class StateTypeIExitConditionPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement();
            EditorStateMachineUtility.CreateTypeSelector<IExitCondition>(root, property, "Exit Condition");
            return root;
        }
    }
}