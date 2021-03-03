﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SLIDDES.LevelEditor.SideScroller3D
{
    /// <summary>
    /// Side Scroller 3D
    /// </summary>
    public class SS3D : EditorWindow
    {
        /// <summary>
        /// Is this toolbar currently inUse?
        /// </summary>
        public static bool inUse;
        /// <summary>
        /// The current tool selected (draw, erase, etc)
        /// </summary>
        public static int currentToolIndex;

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
        private string searchbarResult = "";
        /// <summary>
        /// The size of the asset picture
        /// </summary>
        private Vector2 editorAssetDisplaySize = new Vector2(48, 48);
        /// <summary>
        /// Used for editor scrollbar
        /// </summary>
        private Vector2 editorScrollPosition;
        private bool editorFoldoutTool = true;
        private bool editorFoldoutAssets = true;
        private bool editorFoldoutSettings;
        private bool showAllZLayers;
        private int searchbarResultAmount;
        private int currentAssetViewIndex;

        private int zLayerIndex;
        private string assetsFileDirectory;
        private readonly string assetFileDirectoryDefault = "Assets/Prefabs/Level Editor";


        [MenuItem("Window/SLIDDES/Level Editor/Side Scroller 3D", false)]
        public static void ShowWindow()
        {
            //Show existing window instance. If one doesn't exist, make one.
            EditorWindow window = GetWindow(typeof(SS3D), false, "SS3D", true); // Name
            window.minSize = new Vector2(500, 140);
        }

        private void Awake()
        {
            inUse = true;
            // Load values
            assetsFileDirectory = EditorPrefs.GetString("toolbar_0_fileDirectory", assetFileDirectoryDefault);
        }

        #region OnEnable, OnDestroy, OnFocus

        private void OnEnable()
        {
            objectToCreate = null;
        }

        private void OnDestroy()
        {
            inUse = false;
            if(toolbar_0_ExecuteInEditModeGameObject != null) DestroyImmediate(toolbar_0_ExecuteInEditModeGameObject);
            
            // When the window is destroyed, remove the delegate
            SceneView.duringSceneGui -= this.OnSceneGUI;
        }

        private void OnFocus()
        {
            // Remove delegate listener if it has previously been assigned. (Not thread save?)
            SceneView.duringSceneGui -= this.OnSceneGUI;
            // Add (or re-add) the delegate.
            SceneView.duringSceneGui += this.OnSceneGUI;

            // Based on SS3DEIEM currentToolIndex update this one
            //NewToolIndex(SS3DEIEM.currentToolIndex);
        }

        #endregion

        public void OnGUI()
        {
            // Window code goes here
            EditorGUILayout.BeginVertical(EditorStyles.inspectorDefaultMargins); // Make it look like unity inspector
            editorScrollPosition = EditorGUILayout.BeginScrollView(editorScrollPosition);
            EditorGUILayout.Space();

            // Object that runs in editor mode (allows user to create objects while in edit mode)
            if(SS3DEIEM.Instance == null)
            {
                // Remove old ref if it exists
                if(toolbar_0_ExecuteInEditModeGameObject != null) DestroyImmediate(toolbar_0_ExecuteInEditModeGameObject);

                // Create new reference object
                toolbar_0_ExecuteInEditModeGameObject = new GameObject();
                toolbar_0_ExecuteInEditModeGameObject.transform.position = Vector3.zero;
                toolbar_0_ExecuteInEditModeGameObject.name = "[SS3D] Temporary GameObject";
                toolbar_0_ExecuteInEditModeGameObject.AddComponent<SS3DEIEM>();
                SS3DEIEM.Instance = toolbar_0_ExecuteInEditModeGameObject.GetComponent<SS3DEIEM>();
            }

            #region Events
            Event e = Event.current;

            if(e.isKey)
            {
                if(e.type == EventType.KeyDown)
                {
                    if(e.keyCode == KeyCode.F6) inUse = !inUse;
                }
                else if(e.keyCode == KeyCode.B)
                {
                    NewToolIndex(0);
                }
                else if(e.keyCode == KeyCode.D)
                {
                    NewToolIndex(1);
                }
            }
            #endregion

            #region Tools
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorStyles.foldout.fontStyle = FontStyle.Bold;
            editorFoldoutTool = EditorGUILayout.Foldout(editorFoldoutTool, " Tools", true);

            if(editorFoldoutTool)
            {
                EditorGUILayout.BeginHorizontal();
                // In use button
                Color c = GUI.color;
                if(inUse) GUI.color = Color.green; else GUI.color = Color.red;
                if(GUILayout.Button(new GUIContent("In Use", "Toggle Editor On/Off"), GUILayout.Width(100)))
                {
                    inUse = !inUse;
                    SS3DEIEM.inUse = inUse;
                }
                GUI.color = c;
                EditorGUILayout.Space(10);

                // Tools select
                GUIContent[] g = new GUIContent[] { new GUIContent("", Resources.Load<Texture2D>("d_Grid.PaintTool"), "Paint With Left Mouse Button (B)\nOnly Applies To Current Z Layer!"),
                                                    new GUIContent("", Resources.Load<Texture2D>("d_Grid.EraserTool"), "Erase With Left Mouse Button (D)\nOnly Applies To Current Z Layer!" )};
                NewToolIndex(GUILayout.Toolbar(currentToolIndex, g, GUILayout.MaxWidth(100)));
                EditorGUILayout.Space();

                // Z index
                EditorGUIUtility.labelWidth = 10;
                int prevLayerIndex = zLayerIndex; // Prevent updating zLayerVisablity every frame
                zLayerIndex = EditorGUILayout.IntField("Z", zLayerIndex);
                SS3DEIEM.Instance.zIndex = zLayerIndex;
                if(zLayerIndex != prevLayerIndex) UpdateZLayerVisability();
                // Z index layer visablility
                Texture2D tEye; if(showAllZLayers) tEye = Resources.Load<Texture2D>("d_scenevis_visible_hover"); else tEye = Resources.Load<Texture2D>("d_scenevis_hidden_hover");
                if(GUILayout.Button(new GUIContent("", tEye, "Show All Z Layers Or Only Current Layer")))
                {
                    showAllZLayers = !showAllZLayers;
                    UpdateZLayerVisability();
                }
                EditorGUILayout.Space();

                // 2D view button
                if(GUILayout.Button("2D View", GUILayout.Width(60)))
                {
                    // This stuff apperently cannot auto run, disabled 
                    if(!GetWindow<SceneView>().in2DMode)
                    {
                        Vector3 position = GetWindow<SceneView>().pivot; // Position
                        position.z = -10;
                        GetWindow<SceneView>().pivot = position;
                        GetWindow<SceneView>().in2DMode = true;
                    }
                    else
                    {
                        GetWindow<SceneView>().in2DMode = false;
                    }
                }


                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }
            if(editorFoldoutTool) EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
            #endregion

            #region Asset display
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorStyles.foldout.fontStyle = FontStyle.Bold;
            editorFoldoutAssets = EditorGUILayout.Foldout(editorFoldoutAssets, " Assets", true);

            if(editorFoldoutAssets)
            {
                // Get asset GUIDs from folder with type GameObject
                if(assetsFileDirectory != null) // Prevent wierd bug
                {
                    string[] folderContent = AssetDatabase.FindAssets("t:GameObject", new[] { assetsFileDirectory });

                    EditorGUILayout.BeginHorizontal();
                    // Searchbar
                    EditorGUIUtility.labelWidth = 80;
                    searchbarResult = EditorGUILayout.TextField("Search Asset", searchbarResult, GUILayout.Width(250));
                    // Toggle asset view
                    if(GUILayout.Button(new GUIContent("", Resources.Load<Texture2D>("d_UnityEditor.SceneHierarchyWindow"), "Change View Layout")))
                    {
                        currentAssetViewIndex++;
                        if(currentAssetViewIndex > 1) currentAssetViewIndex = 0;
                    }
                    // Display loaded assets amount
                    EditorGUILayout.LabelField("Loaded: " + folderContent.Length + " Assets", EditorStyles.helpBox);
                    searchbarResultAmount = 0; // updated at CreateItemButton, not working, doesnt update after calc
                    //EditorGUILayout.LabelField("Showing: " + searchbarResultAmount.ToString() + " Assets", EditorStyles.helpBox);
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space(editorSpacePixels);

                    // Display assets
                    EditorGUILayout.BeginVertical();

                    GameObject[] prefabs = new GameObject[folderContent.Length];

                    switch(currentAssetViewIndex)
                    {
                        case 0:
                            // Display as grid
                            int maxRowAmount = Mathf.Clamp(Mathf.FloorToInt(Screen.width / editorAssetDisplaySize.x) - 2, 1, 99);
                            int closer = maxRowAmount;
                            bool closedLayout = true;
                            // Get prefabs
                            for(int i = 0; i < folderContent.Length; i++)
                            {
                                closer--;
                                if(i % maxRowAmount == 0)
                                {
                                    EditorGUILayout.BeginHorizontal();
                                    closedLayout = false;
                                }
                                prefabs[i] = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(folderContent[i]), typeof(GameObject)) as GameObject;
                                CreateItemButton(prefabs[i]);
                                if(closer <= 0)
                                {
                                    EditorGUILayout.EndHorizontal();
                                    closer = maxRowAmount;
                                    closedLayout = true;
                                }
                            }
                            if(!closedLayout) EditorGUILayout.EndHorizontal();
                            break;
                        case 1:
                            // Display vertically
                            for(int i = 0; i < folderContent.Length; i++)
                            {
                                prefabs[i] = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(folderContent[i]), typeof(GameObject)) as GameObject;
                                CreateItemButton(prefabs[i]);
                            }
                            break;
                        default:
                            break;
                    }

                    EditorGUILayout.EndVertical();
                }
            }
            EditorGUILayout.EndVertical();
            #endregion

            #region Settings
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorStyles.foldout.fontStyle = FontStyle.Bold;
            editorFoldoutSettings = EditorGUILayout.Foldout(editorFoldoutSettings, " Settings", true);

            if(editorFoldoutSettings)
            {
                // File directory
                EditorGUILayout.BeginHorizontal();
                EditorGUIUtility.labelWidth = 80;
                assetsFileDirectory = EditorGUILayout.DelayedTextField("Asset Folder", assetsFileDirectory);
                if(string.IsNullOrEmpty(assetsFileDirectory) || string.IsNullOrWhiteSpace(assetsFileDirectory)) assetsFileDirectory = assetFileDirectoryDefault;
                if(GUILayout.Button("Reset", GUILayout.Width(45)))
                {
                    assetsFileDirectory = assetFileDirectoryDefault;
                    Repaint();
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
            #endregion

            // Close off
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            // Do your drawing here using Handles.
            Handles.BeginGUI();
            // Do your drawing here using GUI.
            if(inUse)
            {
                if(GUI.Button(new Rect(10, 10, 120, 20), "Disable Editor (F6)"))
                {
                    inUse = false;
                }
            }
            // Event
            Event e = Event.current;
            if(e.type == EventType.KeyDown)
            {
                if(e.keyCode == KeyCode.F6) inUse = !inUse;
            }
            Handles.EndGUI();
        }

        /// <summary>
        /// Create selectable GUI button
        /// </summary>
        /// <param name="item"></param>
        private void CreateItemButton(GameObject item)
        {
            // Hide button if searchbarResult is not the same
            if(searchbarResult != "")
            {
                if(!item.name.ToLower().Contains(searchbarResult.ToLower())) return;
            }
            searchbarResultAmount++;

            // Based on currentAssetViewIndex show button
            Color c = GUI.color;
            switch(currentAssetViewIndex)
            {
                case 0:
                    // Grid button
                    EditorGUILayout.BeginVertical();
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
                        SS3DEIEM.objectToCreate = objectToCreate;
                        Repaint();
                    }
                    EditorStyles.label.wordWrap = true;
                    EditorGUILayout.LabelField(item.name, EditorStyles.wordWrappedLabel);
                    GUI.color = c;
                    EditorGUILayout.EndVertical();
                    break;
                case 1:
                    // Vertaclly layerd button
                    EditorGUILayout.BeginHorizontal();                    
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
                        SS3DEIEM.objectToCreate = objectToCreate;
                        Repaint();
                    }
                    EditorStyles.label.wordWrap = true;
                    EditorGUILayout.LabelField(item.name, EditorStyles.wordWrappedLabel);
                    GUI.color = c;
                    EditorGUILayout.EndHorizontal();
                    break;
                default: Debug.LogError("Button building"); break;
            }

        }

        /// <summary>
        /// Switches the current toolindex of this and EIEM (execute in edit mode)
        /// </summary>
        /// <param name="newIndex"></param>
        private void NewToolIndex(int newIndex)
        {
            // Ignore if SS3DEIEM updated its currentToolIndex
            if(currentToolIndex != SS3DEIEM.currentToolIndex) return;

            currentToolIndex = newIndex;
            SS3DEIEM.currentToolIndex = newIndex;
            Repaint();
        }

        /// <summary>
        /// Updates the current z layer visabilty
        /// </summary>
        private void UpdateZLayerVisability()
        {
            if(SS3DEIEM.Instance == null) Debug.LogError("No Toolbar 0 EIEM found!");            
            if(showAllZLayers)
            {
                // Show all
                foreach(Transform child in SS3DEIEM.Instance.parentOfItems)
                {
                    child.gameObject.SetActive(true);
                }
            }
            else
            {
                // Hide all but 1
                foreach(Transform child in SS3DEIEM.Instance.parentOfItems)
                {
                    child.gameObject.SetActive(false);
                }
                SS3DEIEM.Instance.parentOfItems.Find(zLayerIndex.ToString())?.gameObject.SetActive(true);
            }
        }
    }
}

// Used links
// Drawing SceneGUI https://answers.unity.com/questions/58018/drawing-to-the-scene-from-an-editorwindow.html
// Find assets https://docs.unity3d.com/2020.1/Documentation/ScriptReference/AssetDatabase.FindAssets.html
// Get assets without resources folder https://gamedev.stackexchange.com/questions/160497/how-to-instantiate-prefab-outside-resources-folder/160537
// Load asset at path https://docs.unity3d.com/2020.1/Documentation/ScriptReference/AssetDatabase.LoadAssetAtPath.html