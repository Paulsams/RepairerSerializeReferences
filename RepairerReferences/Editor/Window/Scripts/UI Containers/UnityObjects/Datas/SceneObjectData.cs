using UnityEditor;
using UnityEngine.SceneManagement;

namespace ChoiceReferenceEditor.Repairer
{
    public class SceneObjectData : BaseUnityObjectData
    {
        public readonly int LocalIdentifierInFile;
        public readonly string SceneName;
    
        public override GUID AssetGuid { get; }

        public SceneObjectData(int localIdentifierInFile, Scene scene)
        {
            LocalIdentifierInFile = localIdentifierInFile;
            SceneName = scene.name;
            AssetGuid = AssetDatabase.GUIDFromAssetPath(scene.path);
        }

        public override UnityObjectBaseContainer ChangeContent(DataObjectContainer dataObjectContainer)
        {
            dataObjectContainer.SceneObjectContainer.ChangeContent(this);
            return dataObjectContainer.SceneObjectContainer;
        }
    }
}
