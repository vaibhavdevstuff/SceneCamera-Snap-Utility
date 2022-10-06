using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DominoCode.Tools
{

    [CustomEditor(typeof(SceneCameraSnapConfig))]
    [InitializeOnLoad]
    public class SceneCameraSnapEditor : Editor
    {
        #region Config Asset

        public static SceneCameraSnapConfig _config;

        [MenuItem("Tools/Scene Camera Snap")]
        public static void PositionSnaptWindow()
        {
            FetchConfig();

            var path = GetConfigPath();
            EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<SceneCameraSnapConfig>(path).GetInstanceID());
        }

        private static void FetchConfig()
        {
            while (true)
            {
                if (_config != null) return;

                var path = GetConfigPath();

                if (path == null)
                {
                    AssetDatabase.CreateAsset(CreateInstance<SceneCameraSnapConfig>(), $"Assets/{(nameof(SceneCameraSnapConfig))}.asset");
                    Debug.Log("A config file has been created at the root of your project.<b> You can move this anywhere you'd like.</b>");
                    continue;
                }

                _config = AssetDatabase.LoadAssetAtPath<SceneCameraSnapConfig>(path);

                break;
            }
        }

        private static string GetConfigPath()
        {
            var paths = AssetDatabase.FindAssets(nameof(SceneCameraSnapConfig)).Select(AssetDatabase.GUIDToAssetPath).Where(c => c.EndsWith(".asset")).ToList();
            if (paths.Count > 1) Debug.LogWarning("Multiple auto save config assets found. Delete one.");
            return paths.FirstOrDefault();
        }

        #endregion

        #region Inspector

        SceneCameraSnapConfig snapConfig;

        private int dataIndex = 0;

        private bool addSceneName = true;

        private string configName;
        private string currentSceneName;

        private string configMessage = "You can move this asset where ever you'd like.\nDo not rename this file.\nKeep the default name 'SceneCameraSnapConfig'\nFind asset in Menu options Tools >> Scene Camera Snap";

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            snapConfig = (SceneCameraSnapConfig)target;

            EditorGUILayout.Space(5);

            EditorGUILayout.HelpBox(configMessage, MessageType.Info);

            EditorGUILayout.Space(10);

            SaveOrDeleteData();

            EditorGUILayout.Space(20);

            if (snapConfig.cameraData.Count != 0)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                {
                    LoopList();

                    EditorGUILayout.Space(10);

                    DrawConfigList();
                }
                EditorGUILayout.EndVertical();
            }

            EditorUtility.SetDirty(snapConfig);
        }

        private void SaveOrDeleteData()
        {
            configName = EditorGUILayout.TextField("Name", configName);
            addSceneName = EditorGUILayout.Toggle("Add Scene Name", addSceneName);

            EditorGUILayout.Space(2);

            if (GUILayout.Button("Save Current Position", GUILayout.ExpandWidth(true), GUILayout.Height(30)))
            {
                Vector3 configPosition = SceneView.lastActiveSceneView.pivot;
                Quaternion configRotation = SceneView.lastActiveSceneView.rotation;

                CameraData campos = new CameraData();

                if (addSceneName)
                    currentSceneName = SceneManager.GetActiveScene().name + " - ";
                else
                    currentSceneName = "";

                if (string.IsNullOrEmpty(configName) || string.IsNullOrWhiteSpace(configName))
                    campos.name = currentSceneName + "Position " + snapConfig.cameraData.Count;
                else
                    campos.name = currentSceneName + configName;

                configName = "";

                campos.position = configPosition;
                campos.rotation = configRotation;

                snapConfig.cameraData.Add(campos);
            }

            if (snapConfig.cameraData.Count > 0 && GUILayout.Button("Delete All", GUILayout.ExpandWidth(true), GUILayout.Height(30)))
                snapConfig.cameraData.Clear();
        }

        private void LoopList()
        {
            if (dataIndex >= snapConfig.cameraData.Count)
                dataIndex = snapConfig.cameraData.Count - 1;

            CameraData config = snapConfig.cameraData[dataIndex];

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("<< Prev", GUILayout.Width(60), GUILayout.Height(25)))
            {
                dataIndex--;

                if (dataIndex < 0) dataIndex = snapConfig.cameraData.Count - 1;

                config = snapConfig.cameraData[dataIndex];
                SetConfigData(config);
            }

            if (GUILayout.Button(config.name, GUILayout.ExpandWidth(true), GUILayout.Height(25)))
                SetConfigData(config);

            if (GUILayout.Button("Next >>", GUILayout.Width(60), GUILayout.Height(25)))
            {
                dataIndex++;

                if (dataIndex >= snapConfig.cameraData.Count) dataIndex = 0;

                config = snapConfig.cameraData[dataIndex];
                SetConfigData(config);
            }

            EditorGUILayout.EndHorizontal();

        }

        private void DrawConfigList()
        {
            for (int i = 0; i < snapConfig.cameraData.Count; i++)
            {
                CameraData config = snapConfig.cameraData[i];

                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button(config.name))
                    SetConfigData(config);

                if (GUILayout.Button("X", GUILayout.Width(30), GUILayout.ExpandHeight(true)))
                {
                    snapConfig.cameraData.Remove(config);
                    break;
                }

                EditorGUILayout.EndHorizontal();
            }

        }

        private void SetConfigData(CameraData config)
        {
            SceneView.lastActiveSceneView.pivot = config.position;
            SceneView.lastActiveSceneView.rotation = config.rotation;
            SceneView.lastActiveSceneView.Repaint();
        }


        #endregion



    }
}