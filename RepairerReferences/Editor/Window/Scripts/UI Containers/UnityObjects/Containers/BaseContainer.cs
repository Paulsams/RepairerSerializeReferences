using UnityEngine.UIElements;

namespace ChoiceReferenceEditor.Repairer
{
    public abstract class BaseContainer
    {
        protected readonly VisualElement Container;

        protected BaseContainer(VisualElement container)
        {
            Container = container;
            // TODO: check that if I remove it, it won’t break anything
            Disable();
        }

        public virtual void Enable() => Container.style.display = DisplayStyle.Flex;

        public virtual void Disable() => Container.style.display = DisplayStyle.None;
    }
}
