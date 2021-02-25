using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SLIDDES.Editor.Window
{
    public class EditorWindowLevelEditor_0 : EditorWindow
    {
        // TODO
        // toggle to disable editor, but still keep windows
        // toolbar 0 searchbar
        // toolbar 0 tags
        // https://answers.unity.com/questions/1098979/custom-editor-show-asset-folders-in-objectfield.html

        // Editor Window Level Editor 0

        /// <summary>
        /// Is level editor in use?
        /// </summary>
        public static bool inUse;

        // Editor Window Toolbar 0
        public static EditorWindow windowToolbar_0;
        public static string toolbar_0_fileDirectory;
        /// <summary>
        /// The default file directory
        /// </summary>
        private readonly static string toolbar_0_fileDirectoryDefault = "Assets/Prefabs/Level Editor";

        // EditorGUILayout
        private readonly int editorSpacePixels = 10;
        /// <summary>
        /// Used for editor scrollbar
        /// </summary>
        private Vector2 editorScrollPosition;

        // Editor values
        private bool editorFoldout_0 = true;

        // Add menu item named "My Window" to the Window menu
        [MenuItem("Window/SLIDDES/Level Editor/0 (Side Scroller)")]
        public static void ShowWindow()
        {
            //Show existing window instance. If one doesn't exist, make one.
            EditorWindow window = EditorWindow.GetWindow(typeof(EditorWindowLevelEditor_0), false, "Lvl Editor 0"); // Name
            window.minSize = new Vector2(100, 140);
            windowToolbar_0 = EditorWindow.GetWindow(typeof(EditorWindowLevelEditor_0_Toolbar_0), false, "Lvl Editor Toolbar 0");

            // Load values
            //toolbar_0_fileDirectoryDefault = Application.dataPath + toolbar_0_fileDirectoryDefault; // Is here cause cannot set Application.dataPath at var declaration
        }

        private void Awake()
        {
            inUse = true;
        }

        private void OnDestroy()
        {
            inUse = false;
            // When the window is destroyed, remove the delegate
            // so that it will no longer do any drawing.
            SceneView.duringSceneGui -= this.OnSceneGUI;

            GetWindow<EditorWindowLevelEditor_0_Toolbar_0>()?.Close(); // Close toolbar window too
        }

        public void OnGUI()
        {
            // Values
            

            // Window code goes here
            EditorGUILayout.BeginVertical(); // Make it look like unity inspector
            editorScrollPosition = EditorGUILayout.BeginScrollView(editorScrollPosition);
            GUILayout.Space(editorSpacePixels);

            // Toolbar 0
            inUse = EditorGUILayout.Toggle("In Use", inUse);
            editorFoldout_0 = EditorGUILayout.BeginFoldoutHeaderGroup(editorFoldout_0, "Level Editor 0 Side Scroller");
            if(editorFoldout_0)
            {
                if(GUILayout.Button("Show Toolbar 0", GUILayout.Height(30)))
                {
                    windowToolbar_0 = EditorWindow.GetWindow(typeof(EditorWindowLevelEditor_0_Toolbar_0), false, "Lvl Editor Toolbar 0");
                }
                // Toolbar 0 file directory
                EditorGUILayout.BeginHorizontal(EditorStyles.inspectorDefaultMargins);
                toolbar_0_fileDirectory = EditorGUILayout.TextField("Toolbar 0 File Directory", toolbar_0_fileDirectory);
                if(string.IsNullOrEmpty(toolbar_0_fileDirectory) || string.IsNullOrWhiteSpace(toolbar_0_fileDirectory)) toolbar_0_fileDirectory = toolbar_0_fileDirectoryDefault; // If empty reset
                if(GUILayout.Button("Reset", GUILayout.Width(45)))
                {
                    toolbar_0_fileDirectory = toolbar_0_fileDirectoryDefault;
                    Repaint();
                }
                EditorGUILayout.EndHorizontal();
                // Check if directory exists
                if(System.IO.Directory.Exists(Application.dataPath.Replace("Assets", "") + toolbar_0_fileDirectory))
                {
                    // Exists
                }
                else
                {
                    // Display warning message
                    EditorGUILayout.HelpBox("File Directory not found at: " + toolbar_0_fileDirectory, MessageType.Error);
                }
            }

            // Close off
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        // Window has been selected
        void OnFocus()
        {
            // Remove delegate listener if it has previously been assigned. (Not thread save?)
            SceneView.duringSceneGui -= this.OnSceneGUI;
            // Add (or re-add) the delegate.
            SceneView.duringSceneGui += this.OnSceneGUI;
            Debug.Log("Selected LVL Editor");
        }

        void OnSceneGUI(SceneView sceneView)
        {
            // Do your drawing here using Handles.
            Handles.BeginGUI();
            // Do your drawing here using GUI.
            GUILayout.Label("Level Editor Side Scroller 0 in use.", GUI.skin.box);
            Handles.EndGUI();
        }
    }
}

// Used links
// Drawing SceneGUI https://answers.unity.com/questions/58018/drawing-to-the-scene-from-an-editorwindow.html
// Find assets https://docs.unity3d.com/2020.1/Documentation/ScriptReference/AssetDatabase.FindAssets.html
// Get assets without resources folder https://gamedev.stackexchange.com/questions/160497/how-to-instantiate-prefab-outside-resources-folder/160537
// Load asset at path https://docs.unity3d.com/2020.1/Documentation/ScriptReference/AssetDatabase.LoadAssetAtPath.html