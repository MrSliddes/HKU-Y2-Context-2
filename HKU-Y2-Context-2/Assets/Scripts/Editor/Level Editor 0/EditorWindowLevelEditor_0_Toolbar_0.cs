using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SLIDDES.Editor.Window
{
    /// <summary>
    /// Included in here for now to keep it 1 file
    /// </summary>
    public class EditorWindowLevelEditor_0_Toolbar_0 : EditorWindow
    {
        /// <summary>
        /// Is this toolbar currently inUse?
        /// </summary>
        public static bool inUse;

        /// <summary>
        /// Reference to the toolbar_0_ExecuteInEditMode GameObject in the scene
        /// </summary>
        public GameObject toolbar_0_ExecuteInEditModeGameObject;
        /// <summary>
        /// The object to create when clicking
        /// </summary>
        public static GameObject objectToCreate;

        // Editor
        private readonly int editorSpacePixels = 10;
        /// <summary>
        /// Used for editor scrollbar
        /// </summary>
        private Vector2 editorScrollPosition;

        private void Awake()
        {
            inUse = true;
        }

        private void OnDestroy()
        {
            inUse = false;
            if(toolbar_0_ExecuteInEditModeGameObject != null) DestroyImmediate(toolbar_0_ExecuteInEditModeGameObject);
        }

        public void OnGUI()
        {
            // Values
            // Change sceneView Camera if not in playmode
            if(!Application.isPlaying)
            {
                Vector3 position = UnityEditor.EditorWindow.GetWindow<SceneView>().pivot; // Position
                position.z = -10;
                UnityEditor.EditorWindow.GetWindow<SceneView>().pivot = position;
                UnityEditor.EditorWindow.GetWindow<SceneView>().in2DMode = true;
            }

            // Object that runs in editor mode (allows user to create objects while in edit mode)
            if(Toolbar_0_ExecuteInEditMode.Instance == null)
            {
                // Remove old ref if it exists
                if(toolbar_0_ExecuteInEditModeGameObject != null) DestroyImmediate(toolbar_0_ExecuteInEditModeGameObject);

                // Create new reference object
                toolbar_0_ExecuteInEditModeGameObject = new GameObject();
                toolbar_0_ExecuteInEditModeGameObject.transform.position = Vector3.zero;
                toolbar_0_ExecuteInEditModeGameObject.name = "[Level Editor 0] (Dont remove!)";
                toolbar_0_ExecuteInEditModeGameObject.AddComponent<Toolbar_0_ExecuteInEditMode>();
                Toolbar_0_ExecuteInEditMode.Instance = toolbar_0_ExecuteInEditModeGameObject.GetComponent<Toolbar_0_ExecuteInEditMode>();
            }

            // Window code goes here
            EditorGUILayout.BeginVertical(); // Make it look like unity inspector
            editorScrollPosition = EditorGUILayout.BeginScrollView(editorScrollPosition);
            GUILayout.Space(editorSpacePixels);

            // Get asset GUIDs from folder with type GameObject
            if(EditorWindowLevelEditor_0.toolbar_0_fileDirectory != null) // Prevent wierd bug
            {
                string[] folderContent = AssetDatabase.FindAssets("t:GameObject", new[] { EditorWindowLevelEditor_0.toolbar_0_fileDirectory });

                // Display loaded assets amount
                EditorGUILayout.LabelField("Loaded: " + folderContent.Length + " Assets");

                // Display assets
                EditorGUILayout.BeginHorizontal();

                GameObject[] prefabs = new GameObject[folderContent.Length];
                // Get prefabs
                for(int i = 0; i < folderContent.Length; i++)
                {
                    prefabs[i] = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(folderContent[i]), typeof(GameObject)) as GameObject;
                    CreateItemButton(prefabs[i]);

                }

                EditorGUILayout.EndHorizontal();
            }

            // Close off
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        private void CreateItemButton(GameObject item)
        {
            EditorGUILayout.BeginVertical();
            Color c = GUI.color;
            if(objectToCreate == item)
            {
                GUI.color = Color.green;
            }
            if(GUILayout.Button(AssetPreview.GetAssetPreview(item), GUILayout.Width(64), GUILayout.Height(64)))
            {
                // Select button
                if(objectToCreate == item)
                {
                    // User clicked already selected button, deselect it
                    objectToCreate = null;
                }
                else
                {
                    objectToCreate = item;
                }
                if(toolbar_0_ExecuteInEditModeGameObject != null) toolbar_0_ExecuteInEditModeGameObject.GetComponent<Toolbar_0_ExecuteInEditMode>().objectToCreate = objectToCreate;
                Repaint();
            }
            GUILayout.Label(item.name, EditorStyles.miniLabel);
            GUI.color = c;
            EditorGUILayout.EndVertical();
        }

    }
}