using System.Collections.Generic;
using System.Linq;
using Paulsams.MicsUtils;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ChoiceReferenceEditor.Repairer
{
    public class CollectorMissingTypes
    {
        private readonly Dictionary<TypeData, ContainerMissingTypes> _missingTypes =
            new Dictionary<TypeData, ContainerMissingTypes>();

        public ListWithEvent<ContainerMissingTypes> Collect()
        {
            CollectByPrefabs();
            CollectByScriptableObjects();
            CollectByScenes();

            ListWithEvent<ContainerMissingTypes> answer = new ListWithEvent<ContainerMissingTypes>(_missingTypes
                .Select((typeAndContainer) => typeAndContainer.Value)
                .ToList());
            _missingTypes.Clear();
            return answer;
        }

        private void CollectByPrefabs()
        {
            Scene previewScene = EditorSceneManager.NewPreviewScene();

            foreach (var pathToPrefab in AssetDatabase.FindAssets(AssetDatabaseUtilities.FilterKeys.Prefabs)
                         .Where((path) => path.StartsWith("Assets/")))
            {
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(pathToPrefab);
                PrefabUtility.LoadPrefabContentsIntoPreviewScene(pathToPrefab, previewScene);
                var copyPrefab = previewScene.GetRootGameObjects()[0];

                var componentsPrefab = prefab.GetComponentsInChildren<MonoBehaviour>();
                var componentsCopyPrefab = copyPrefab.GetComponentsInChildren<MonoBehaviour>();

                for (int i = 0; i < componentsCopyPrefab.Length; ++i)
                {
                    var monoBehaviour = componentsCopyPrefab[i];
                    if (SerializationUtility.HasManagedReferencesWithMissingTypes(monoBehaviour) == false)
                        continue;

                    foreach (var missingType in
                             SerializationUtility.GetManagedReferencesWithMissingTypes(monoBehaviour))
                    {
                        var prefabObject = new UnityObjectData(componentsPrefab[i].gameObject, pathToPrefab);

                        AddMissingType(missingType, prefabObject);
                    }
                }

                Object.DestroyImmediate(copyPrefab);
            }

            EditorSceneManager.ClosePreviewScene(previewScene);
        }

        private void CollectByScriptableObjects()
        {
            foreach (var pathToPrefab in AssetDatabase.FindAssets(AssetDatabaseUtilities.FilterKeys.ScriptableObjects)
                         .Where((path) => path.StartsWith("Assets/")))
            {
                var scriptableObject = AssetDatabase.LoadAssetAtPath<ScriptableObject>(pathToPrefab);

                if (SerializationUtility.HasManagedReferencesWithMissingTypes(scriptableObject) == false)
                    continue;

                foreach (var missingType in SerializationUtility.GetManagedReferencesWithMissingTypes(scriptableObject))
                {
                    var prefabObject = new UnityObjectData(scriptableObject, pathToPrefab);
                    AddMissingType(missingType, prefabObject);
                }
            }
        }

        private void CollectByScenes()
        {
            try
            {
                foreach (var scene in AssetDatabaseUtilities.GetAllScenesInAssets())
                {
                    var gameObjects = scene.GetRootGameObjects();

                    foreach (var objectOnScene in gameObjects)
                    {
                        foreach (var monoBehaviour in objectOnScene.GetComponentsInChildren<MonoBehaviour>())
                        {
                            if (SerializationUtility.HasManagedReferencesWithMissingTypes(monoBehaviour) == false)
                                continue;

                            foreach (var missingType in SerializationUtility.GetManagedReferencesWithMissingTypes(
                                         monoBehaviour))
                            {
                                var sceneObject = new SceneObjectData(monoBehaviour.GetLocalIdentifierInFile(), scene);
                                AddMissingType(missingType, sceneObject);
                            }
                        }
                    }
                }
            }
            catch
            {
                /*ignored*/
            }
        }

        private void AddMissingType(ManagedReferenceMissingType missingType, BaseUnityObjectData unityObject)
        {
            var typeData = new TypeData(missingType);
            var missingTypeData = new MissingTypeData(missingType, unityObject);
            if (_missingTypes.TryGetValue(typeData, out var containerMissingTypes) == false)
            {
                containerMissingTypes = new ContainerMissingTypes(typeData);
                _missingTypes.Add(typeData, containerMissingTypes);
            }

            containerMissingTypes.Add(missingTypeData);
        }
    }
}