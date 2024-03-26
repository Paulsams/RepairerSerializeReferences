using UnityEngine.UIElements;

namespace ChoiceReferenceEditor.Repairer
{
    public class DataObjectContainer
    {
        private readonly VisualElement _dataObjectContainer;
        private readonly VisualElement _miscInfoContainer;

        private readonly LongField _referenceIdField;
        private readonly TextField _serializedDataField;

        public readonly UnityObjectContainer UnityObjectContainer;
        public readonly SceneObjectContainer SceneObjectContainer;

        private readonly ChangerIndexContainer _changerIndex;

        private BaseContainer _currentContainerForObject;

        private IReadonlyCollectionWithEvent<MissingTypeData> _missingTypesData;

        public MissingTypeData CurrentMissingType => _missingTypesData[_changerIndex.CurrentIndex];

        public DataObjectContainer(VisualElement mainContentContainer)
        {
            _dataObjectContainer = mainContentContainer
                .Q<VisualElement>("DataObjectContainer")
                .Q<VisualElement>("ContentContainer")
                .contentContainer;
            _miscInfoContainer = _dataObjectContainer.Q<VisualElement>("MiscInfoContainer");

            _referenceIdField = _miscInfoContainer.Q<LongField>("ReferenceId");
            _serializedDataField = _miscInfoContainer.Q<TextField>("SerializedData");

            _changerIndex = new ChangerIndexContainer(mainContentContainer, "");
            _changerIndex.ChangedIndex += OnChangedIndex;

            SceneObjectContainer = new SceneObjectContainer(_dataObjectContainer);
            UnityObjectContainer = new UnityObjectContainer(_dataObjectContainer);
        }

        public void Reset()
        {
            _currentContainerForObject?.Disable();
            _currentContainerForObject = null;
        }

        public void ChangeCollectionData(IReadonlyCollectionWithEvent<MissingTypeData> missingTypesData)
        {
            _missingTypesData = missingTypesData;
            _changerIndex.ChangeCollection(_missingTypesData);

            UpdateContent();
        }

        public void UpdateContent()
        {
            Reset();
            if (_missingTypesData == null || _missingTypesData.Count == 0)
                return;

            MissingTypeData missingType = CurrentMissingType;

            _referenceIdField.SetValueWithoutNotify(missingType.Data.referenceId);
            _serializedDataField.SetValueWithoutNotify(missingType.Data.serializedData);

            _currentContainerForObject = missingType.UnityObject.ChangeContent(this);
            _currentContainerForObject.Enable();
        }
        
        private void OnChangedIndex() => UpdateContent();
    }
}
