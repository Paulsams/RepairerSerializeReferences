using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System;
using System.Linq;
using Paulsams.MicsUtils;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine.SceneManagement;

namespace ChoiceReferenceEditor.Repairer
{
    public class RepairerSerializeReference : EditorWindow
    {
        private static readonly Vector2 _minSizeWindow = new Vector2(450, 450);

        [SerializeField] private VisualTreeAsset _treeAsset;

        private ListWithEvent<ContainerMissingTypes> _missingTypes;
        private MainContentContainer _mainContentContainer;

        [MenuItem("Window/RepairerSerializeReference")]
        public static void ShowWindow()
        {
            RepairerSerializeReference editorWindow = GetWindow<RepairerSerializeReference>();
            editorWindow.titleContent = new GUIContent("RepairerSerializeReference");
            editorWindow.minSize = _minSizeWindow;
        }

        private bool TryInit()
        {
            var allCurrentDirtyScenes = EditorSceneManager
                .GetSceneManagerSetup()
                .Where(sceneSetup => sceneSetup.isLoaded)
                .Select(sceneSetup => EditorSceneManager.GetSceneByPath(sceneSetup.path))
                .Where(scene => scene.isDirty)
                .ToArray();
            
            if (allCurrentDirtyScenes.Length != 0)
            {
                bool result = EditorUtility.DisplayDialog(
                    "Current active scene(s) is dirty",
                    "Please save all active scenes as they may be overwritten",
                    "Save active scene and Continue",
                    "Cancel update"
                );
                if (result == false)
                    return false;

                foreach (var dirtyScene in allCurrentDirtyScenes)
                    EditorSceneManager.SaveScene(dirtyScene);
            }
            
            var missingTypes = new Dictionary<TypeData, ContainerMissingTypes>();

            void AddMissingType(ManagedReferenceMissingType missingType, BaseUnityObjectData unityObject)
            {
                var typeData = new TypeData(missingType);
                var missingTypeData = new MissingTypeData(missingType, unityObject);
                if (missingTypes.TryGetValue(typeData, out var containerMissingTypes) == false)
                {
                    containerMissingTypes = new ContainerMissingTypes(typeData);
                    missingTypes.Add(typeData, containerMissingTypes);
                }

                containerMissingTypes.Add(missingTypeData);
            }

            Scene previewScene = EditorSceneManager.NewPreviewScene();

            foreach (var pathToPrefab in AssetDatabaseUtilities.GetPathToAllPrefabsAssets()
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
                    if (SerializationUtility.HasManagedReferencesWithMissingTypes(monoBehaviour))
                    {
                        foreach (var missingType in SerializationUtility.GetManagedReferencesWithMissingTypes(monoBehaviour))
                        {
                            var prefabObject = new PrefabObjectData(componentsPrefab[i].gameObject, pathToPrefab);

                            AddMissingType(missingType, prefabObject);
                        }
                    }
                }

                DestroyImmediate(copyPrefab);
            }

            EditorSceneManager.ClosePreviewScene(previewScene);

            try
            {
                foreach (var scene in AssetDatabaseUtilities.GetPathsToAllScenesInProject()
                             .Where((path) => path.path.StartsWith("Assets/")))
                {
                    var gameObjects = scene.GetRootGameObjects();

                    foreach (var objectOnScene in gameObjects)
                    {
                        foreach (var monoBehaviour in objectOnScene.GetComponentsInChildren<MonoBehaviour>())
                        {
                            if (SerializationUtility.HasManagedReferencesWithMissingTypes(monoBehaviour) == false)
                                continue;
                            
                            foreach (var missingType in SerializationUtility.GetManagedReferencesWithMissingTypes(monoBehaviour))
                            {
                                var sceneObject = new SceneObjectData(monoBehaviour.GetLocalIdentifierInFile(), scene);
                                AddMissingType(missingType, sceneObject);
                            }
                        }
                    }
                }
            }
            catch { /*ignored*/ }

            _missingTypes = new ListWithEvent<ContainerMissingTypes>(missingTypes
                .Select((typeAndContainer) => typeAndContainer.Value)
                .ToList());

            return true;
        }

        private void UpdateAll()
        {
            Dispose();
            if (TryInit())
                InitContainers();
        }

        private void InitContainers() => _mainContentContainer.Init(_missingTypes);

        private void CreateGUI()
        {
            VisualElement root = rootVisualElement;

            TemplateContainer editorWindow = _treeAsset.Instantiate();
            editorWindow.style.flexGrow = 1f;
            root.Add(editorWindow);

            Toolbar mainToolbar = editorWindow.Q<Toolbar>("MainToolbar");
            ToolbarButton updateButton = mainToolbar.Q<ToolbarButton>("Update");
            updateButton.clicked += UpdateAll;

            _mainContentContainer = new MainContentContainer(editorWindow);
            _mainContentContainer.ChangedContainerReferences += OnChangedContainerReferences;
            _mainContentContainer.ChangedSingleReference += OnChangedSingleReference;
        }

        private void Dispose()
        {
            _missingTypes = null;
            _mainContentContainer.Reset();
        }

        private void OnChangedSingleReference(Type type, MissingTypeData missingTypeData)
        {
            ManagedReferenceMissingType missingType = missingTypeData.Data;

            var repairer = new RepairerFile(type, missingTypeData.UnityObject.LocalAssetPath);
            repairer.Repair(() => repairer.CheckNeedLineAndReplacedIt(missingType));

            _missingTypes[_mainContentContainer.CurrentIndex].Remove(missingTypeData);
            if (_missingTypes[_mainContentContainer.CurrentIndex].ManagedReferencesMissingTypeDatas.Collection.Count == 0)
                _missingTypes.RemoveAt(_mainContentContainer.CurrentIndex);
        }

        private void OnChangedContainerReferences(Type type, ContainerMissingTypes containerMissingTypes)
        {
            Dictionary<string, List<ManagedReferenceMissingType>> filesData = new Dictionary<string, List<ManagedReferenceMissingType>>();

            foreach (var missingType in containerMissingTypes.ManagedReferencesMissingTypeDatas.Collection)
            {
                var localAssetPath = missingType.UnityObject.LocalAssetPath;
                if (filesData.TryGetValue(localAssetPath, out var missingTypes) == false)
                {
                    missingTypes = new List<ManagedReferenceMissingType>();
                    filesData.Add(localAssetPath, missingTypes);
                }

                missingTypes.Add(missingType.Data);
            }

            foreach (var fileData in filesData)
            {
                string localAssetPath = fileData.Key;
                List<ManagedReferenceMissingType> missingTypes = fileData.Value;

                var repairer = new RepairerFile(type, localAssetPath);
                repairer.Repair(() =>
                {
                    for (int i = missingTypes.Count - 1; i >= 0; --i)
                    {
                        if (repairer.CheckNeedLineAndReplacedIt(missingTypes[i]))
                        {
                            missingTypes.RemoveAt(i);
                            break;
                        }
                    }

                    return missingTypes.Count == 0;
                });
            }

            _missingTypes.RemoveAt(_mainContentContainer.CurrentIndex);
        }

        private void OnDisable() => Dispose();
    }
}
