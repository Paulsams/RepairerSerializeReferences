using Paulsams.MicsUtils;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace ChoiceReferenceEditor.Repairer
{
    public class SceneObjectContainer : UnityObjectBaseContainer
    {
        private readonly TextField _sceneNameField;
        private readonly Button _openSceneButton;
        private readonly ObjectField _objectField;

        private GUID _guidUnityCachedScene;
        private Dictionary<int, MonoBehaviour> _instanceIDToObject = new Dictionary<int, MonoBehaviour>();
        private SceneObjectData _currentSceneObject;

        public SceneObjectContainer(VisualElement contentContainer)
            : base(contentContainer.Q<VisualElement>("SceneObjectContainer"))
        {
            _sceneNameField = _container.Q<TextField>("SceneName");
            _openSceneButton = _container.Q<Button>("OpenScene");
            _objectField = _container.Q<ObjectField>("SceneObject");

            _openSceneButton.clicked += OnWantOpenScene;
        }

        public void ChangeContent(SceneObjectData sceneObject)
        {
            _currentSceneObject = sceneObject;
            _sceneNameField.SetValueWithoutNotify(_currentSceneObject.SceneName);

            if (_guidUnityCachedScene == _currentSceneObject.AssetGuid)
                ChangeStateButtonOpenSceneAndUnityObjectField(true);
            else
                ChangeStateCurrentScene(
                    EditorSceneManager
                        .GetSceneManagerSetup()
                        .Any(sceneSetup => sceneSetup.path == _currentSceneObject.LocalAssetPath)
                );
        }

        public override void Enable()
        {
            base.Enable();

            EditorSceneManager.sceneOpened += OnSceneOpened;
            EditorSceneManager.sceneClosed += OnSceneClosed;
        }

        public override void Disable()
        {
            base.Disable();

            EditorSceneManager.sceneOpened -= OnSceneOpened;
            EditorSceneManager.sceneClosed -= OnSceneClosed;
        }

        private void ChangeStateCurrentScene(bool state)
        {
            if (state)
                FindAllComponentInCurrentScene();
            ChangeStateButtonOpenSceneAndUnityObjectField(state);
        }

        private void FindAllComponentInCurrentScene()
        {
            _guidUnityCachedScene = _currentSceneObject.AssetGuid;
            _instanceIDToObject = EditorSceneManager
                .GetSceneByPath(_currentSceneObject.LocalAssetPath)
                .GetRootGameObjects()
                .SelectMany(gameObject => gameObject.GetComponentsInChildren<MonoBehaviour>())
                .ToDictionary((value) => value.GetLocalIdentifierInFile());
        }

        private void ChangeStateButtonOpenSceneAndUnityObjectField(bool state)
        {
            _openSceneButton.style.display = state ? DisplayStyle.None : DisplayStyle.Flex;
            _objectField.SetValueWithoutNotify(state
                ? _instanceIDToObject[_currentSceneObject.LocalIdentifierInFile]
                : null
            );
        }

        private void OnSceneOpened(Scene scene, OpenSceneMode mode)
        {
            if (_currentSceneObject.LocalAssetPath != scene.path)
                return;

            ChangeStateCurrentScene(true);
        }

        private void OnSceneClosed(Scene scene)
        {
            if (_currentSceneObject.LocalAssetPath != scene.path)
                return;
            
            ChangeStateCurrentScene(false);
        }

        private void OnWantOpenScene() => OpenScene(_currentSceneObject);

        private void OpenScene(SceneObjectData sceneObject) =>
            EditorSceneManager.OpenScene(sceneObject.LocalAssetPath, OpenSceneMode.Single);
    }
}
