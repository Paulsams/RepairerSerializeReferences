using UnityEngine.UIElements;

namespace ChoiceReferenceEditor.Repairer
{
    public abstract class UnityObjectBaseContainer
    {
        protected readonly VisualElement _container;

        protected UnityObjectBaseContainer(VisualElement container)
        {
            _container = container;
            // TODO: проверить, что если я уберу, то не будет ли это ничего ломать
            Disable();
        }

        public virtual void Enable() => _container.style.display = DisplayStyle.Flex;

        public virtual void Disable() => _container.style.display = DisplayStyle.None;
    }
}
