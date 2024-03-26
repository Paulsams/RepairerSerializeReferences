using UnityEditor;
using UnityEngine;

namespace ChoiceReferenceEditor.Repairer
{
    public class UnityObjectData : BaseUnityObjectData
    {
        public readonly Object UnityObject;
        public override GUID AssetGuid { get; }

        public UnityObjectData(Object unityObject, string pathToPrefab)
        {
            UnityObject = unityObject;
            AssetGuid = AssetDatabase.GUIDFromAssetPath(pathToPrefab);
        }

        public override BaseContainer ChangeContent(DataObjectContainer dataObjectContainer)
        {
            dataObjectContainer.UnityObjectContainer.ChangeContent(this);
            return dataObjectContainer.UnityObjectContainer;
        }
    }
}
