using UnityEditor;
using UnityEngine;

namespace ChoiceReferenceEditor.Repairer
{
    public class PrefabObjectData : BaseUnityObjectData
    {
        public readonly GameObject Prefab;
        public override GUID AssetGuid { get; }

        public PrefabObjectData(GameObject componentInPrefab, string pathToPrefab)
        {
            Prefab = componentInPrefab;
            AssetGuid = AssetDatabase.GUIDFromAssetPath(pathToPrefab);
        }

        public override UnityObjectBaseContainer ChangeContent(DataObjectContainer dataObjectContainer)
        {
            dataObjectContainer.PrefabObjectContainer.ChangeContent(this);
            return dataObjectContainer.PrefabObjectContainer;
        }
    }
}
