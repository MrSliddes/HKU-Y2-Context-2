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
        /// Contains the searchbar result
        /// </summary>
        private string searchbarResult;
        /// <summary>
        /// The size of the asset picture
        /// </summary>
        private Vector2 editorAssetDisplaySize = new Vector2(48, 48);
        /// <summary>
        /// Used for editor scrollbar
        /// </summary>
        private Vector2 editorScrollPosition;


        public static void ShowWindow()
        {
            //Show existing window instance. If one doesn't exist, make one.
            EditorWindow window = EditorWindow.GetWindow(typeof(EditorWindowLevelEditor_0), false, "Lvl Editor 0"); // Name
            window.minSize = new Vector2(500, 140);
        }

        private void Awake()
        {
            inUse = true;
        }

        #region OnEnable & OnDestroy

        private void OnEnable()
        {
            objectToCreate = null;
        }

        private void OnDestroy()
        {
            inUse = false;
            if(toolbar_0_ExecuteInEditModeGameObject != null) DestroyImmediate(toolbar_0_ExecuteInEditModeGameObject);
        }

        #endregion

        public void OnGUI()
        {
            // Window code goes here
            EditorGUILayout.BeginVertical(EditorStyles.inspectorDefaultMargins); // Make it look like unity inspector
            editorScrollPosition = EditorGUILayout.BeginScrollView(editorScrollPosition);
            GUILayout.Space(editorSpacePixels);

            // Values
            //Change sceneView Camera if not in playmode >>> Somehow disabled the textfields...
                //if(!Application.isPlaying)
                //{
                //    Vector3 position = UnityEditor.EditorWindow.GetWindow<SceneView>().pivot; // Position
                //    position.z = -10;
                //    UnityEditor.EditorWindow.GetWindow<SceneView>().pivot = position;
                //    UnityEditor.EditorWindow.GetWindow<SceneView>().in2DMode = true;
                //}

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

            #region Top banner
            EditorGUILayout.BeginHorizontal();

            // Searchbar
            EditorGUIUtility.labelWidth = 80;
            searchbarResult = EditorGUILayout.TextField("Search Asset", searchbarResult, GUILayout.Width(250));

            // 2D view button
            GUILayout.FlexibleSpace();
            if(GUILayout.Button("2D View", GUILayout.Width(60)))
            {
                // This stuff apperently cannot auto run, disabled 
                Vector3 position = GetWindow<SceneView>().pivot; // Position
                position.z = -10;
                GetWindow<SceneView>().pivot = position;
                GetWindow<SceneView>().in2DMode = true;
            }

            EditorGUILayout.EndHorizontal();
            #endregion

            // Get asset GUIDs from folder with type GameObject
            if(EditorWindowLevelEditor_0.toolbar_0_fileDirectory != null) // Prevent wierd bug
            {
                string[] folderContent = AssetDatabase.FindAssets("t:GameObject", new[] { EditorWindowLevelEditor_0.toolbar_0_fileDirectory });

                // Display loaded assets amount
                EditorGUILayout.LabelField("Loaded: " + folderContent.Length + " Assets", EditorStyles.helpBox);

                // Display assets
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                GameObject[] prefabs = new GameObject[folderContent.Length];
                Debug.Log(Screen.width);
                Debug.Log(Mathf.FloorToInt(Screen.width / editorAssetDisplaySize.x));
                int maxRowAmount = Mathf.FloorToInt(Screen.width / editorAssetDisplaySize.x) - 2;
                int closer = maxRowAmount;
                // Get prefabs
                for(int i = 0; i < folderContent.Length; i++)
                {
                    closer--;
                    if(i % maxRowAmount == 0)
                    {
                        EditorGUILayout.BeginHorizontal();
                    }
                    prefabs[i] = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(folderContent[i]), typeof(GameObject)) as GameObject;
                    CreateItemButton(prefabs[i]);
                    if(closer <= 0)
                    {
                        EditorGUILayout.EndHorizontal();
                        closer = maxRowAmount;
                    }
                }

                EditorGUILayout.EndVertical();
            }

            // Close off
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        private void CreateItemButton(GameObject item)
        {
            // Hide button if searchbarResult is not the same
            if(searchbarResult != "" && !item.name.ToLower().Contains(searchbarResult.ToLower())) return;

            EditorGUILayout.BeginVertical();
            Color c = GUI.color;
            if(objectToCreate == item)
            {
                GUI.color = Color.green;
            }
            if(GUILayout.Button(AssetPreview.GetAssetPreview(item), GUILayout.Width(editorAssetDisplaySize.x), GUILayout.Height(editorAssetDisplaySize.y)))
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
            //GUILayout.Label(item.name, EditorStyles.miniLabel);
            EditorStyles.label.wordWrap = true;
            EditorGUILayout.LabelField(item.name, EditorStyles.wordWrappedLabel);
            GUI.color = c;
            EditorGUILayout.EndVertical();
        }

    }
}