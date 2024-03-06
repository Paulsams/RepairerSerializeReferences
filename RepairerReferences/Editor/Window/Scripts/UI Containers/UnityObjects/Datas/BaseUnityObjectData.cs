using UnityEditor;

namespace ChoiceReferenceEditor.Repairer
{
    public abstract class BaseUnityObjectData
    {
        public string LocalAssetPath => AssetDatabase.GUIDToAssetPath(AssetGuid);
        public abstract GUID AssetGuid { get; }

        public abstract UnityObjectBaseContainer ChangeContent(DataObjectContainer dataObjectContainer);
    }
}
