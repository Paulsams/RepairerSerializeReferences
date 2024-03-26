using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace ChoiceReferenceEditor.Repairer
{
    public class UnityObjectContainer : BaseContainer
    {
        private readonly ObjectField _prefabField;

        public UnityObjectContainer(VisualElement contentContainer)
            : base(contentContainer.Q<VisualElement>("UnityObjectContainer"))
        {
            _prefabField = Container.Q<ObjectField>("UnityObject");
        }

        public void ChangeContent(UnityObjectData unityObject)
        {
            _prefabField.SetValueWithoutNotify(unityObject.UnityObject);
        }
    }
}
