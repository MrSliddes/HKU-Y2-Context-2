#if UNITY_EDITOR // This script is only used in the editor, but it cannot be inside the Editor folder since it has [ExecuteInEditMode]
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SLIDDES.Editor.Window
{

    [ExecuteInEditMode]
    public class Toolbar_0_ExecuteInEditMode : MonoBehaviour
    {
        public static Toolbar_0_ExecuteInEditMode Instance { get; set; }

        /// <summary>
        /// The current object to create
        /// </summary>
        public GameObject objectToCreate;
        /// <summary>
        /// The parent of all created items
        /// </summary>
        public Transform parentOfItems;

        /// <summary>
        /// Current mouse position of sceneView in world position
        /// </summary>
        public Vector3 mousePositionScene;
        /// <summary>
        /// Current mouse GUIPoint position
        /// </summary>
        public Vector3 mousePositionGUIPoint;

        private void Awake()
        {
            // Set instance
            if(Instance != null) DestroyImmediate(Instance.gameObject);
            Instance = this;

            // Get parent gameobject
            //if(!GameObject.find)
            print("awake");
        }

        private void Start()
        {
            
        }

        private void Update()
        {
            
        }

        private void CreateParentItems()
        {
            GameObject parent = GameObject.Find("[Editor Level 0] Parent");
            if(parent == null)
            {
                // Create parent
                parent = new GameObject();
                parent.name = "[Editor Level 0] Parent";
                parent.transform.position = Vector3.zero;
                parentOfItems = parent.transform;
            }
            else
            {
                // Assign existing parent
                parentOfItems = parent.transform;
            }
        }

        private void GetMousePositionScene(SceneView scene)
        {
            Vector3 mousePosition = Event.current.mousePosition;

            // Check if mousePosition is in sceneView
            if(mousePosition.x >= 0 && mousePosition.x <= scene.camera.pixelWidth && mousePosition.y >= 0 && mousePosition.y <= scene.camera.pixelHeight) { } else return;
            mousePositionGUIPoint = mousePosition;
            Ray ray = HandleUtility.GUIPointToWorldRay(mousePositionGUIPoint);

            mousePositionScene = ray.origin;
            mousePositionScene = SnapToGrid(mousePositionScene) - new Vector3(0.5f, 0.5f, 0);
            mousePositionScene.z = 0;
        }

        private Vector3 SnapToGrid(Vector3 v)
        {
            Vector3 gridPos = v;
            int gridSize = 1;
            gridPos.x = Mathf.Round(v.x / gridSize) * gridSize + gridSize / 2f;
            gridPos.y = Mathf.Round(v.y / gridSize) * gridSize + gridSize / 2f;
            return gridPos;
        }

        /// <summary>
        /// Check if the position under mouse world space is occupied by gameobject
        /// </summary>
        /// <returns></returns>
        private bool PositionIsOccupied(SceneView scene)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(mousePositionGUIPoint);

            RaycastHit hit;
            if(Physics.Raycast(ray, out hit))
            {
                if(hit.transform.gameObject != null)
                {
                    print(hit.transform.gameObject.name);
                    return true;
                }
            }
            return false;
        }


        

        void OnEnable()
        {
            SceneView.duringSceneGui += this.OnScene;
            //Tools.hidden = true;
            // Assign parent
            CreateParentItems();
        }

        private void OnDestroy()
        {
            SceneView.duringSceneGui -= this.OnScene;
            //Tools.hidden = false;
        }

        void OnScene(SceneView scene)
        {
            //Cursor.SetCursor(Resources.Load<Texture2D>("d_eyeDropper.Large"), Vector2.zero, CursorMode.Auto); causes flikkering
            Event e = Event.current;
            
            if(e.type != EventType.MouseLeaveWindow) // Prevent Screen position out of view frustum Error
            {
                GetMousePositionScene(scene);
            }
            // move custom gameobject here to show mousepos as alternative of Gizmos

            
            if(Input.GetKeyDown(KeyCode.LeftControl))
            {
                Debug.Log("undo");
                Undo.PerformUndo();
            }

            // Disable drag with right mouse button
            if(e.isMouse && e.type == EventType.MouseDrag && e.button == 1)
            {
                //Event.current.Use();
            }

            // Get values
            var controlID = GUIUtility.GetControlID(FocusType.Passive);
            var eventType = e.GetTypeForControl(controlID);

            // Left mouse button
            if(e.button == 0)
            {
                if(eventType == EventType.MouseUp)
                {
                    //Debug.Log("Mouse Up!");
                    GUIUtility.hotControl = controlID;
                    e.Use();
                }
                else if(eventType == EventType.MouseDrag)
                {
                    //Debug.Log("Mouse Drag!");
                    e.Use();
                    PlaceItem(scene);
                }
                else if(eventType == EventType.MouseDown)
                {
                    //Debug.Log("Mouse Down!");
                    GUIUtility.hotControl = 0;
                    e.Use();
                    PlaceItem(scene);
                }
            }
            // Right mouse button
            else if(e.button == 1)
            {
                if(eventType == EventType.MouseUp)
                {
                    //Debug.Log("R Mouse Up!");
                    GUIUtility.hotControl = controlID;
                    e.Use();
                }
                else if(eventType == EventType.MouseDrag)
                {
                    //Debug.Log("R Mouse Drag!");
                    e.Use();
                    RemoveItem(e, scene);
                }
                else if(eventType == EventType.MouseDown)
                {
                    //Debug.Log("R Mouse Down!");
                    GUIUtility.hotControl = 0;
                    e.Use();
                    RemoveItem(e, scene);
                }
            }           

            
        }

        #region OnScene Functions

        private void PlaceItem(SceneView scene)
        {
            
            if(PositionIsOccupied(scene)) return;

            // Place item                
            if(objectToCreate != null)
            {
                GameObject a = Instantiate(objectToCreate, mousePositionScene, Quaternion.identity);
                if(parentOfItems == null) CreateParentItems();
                a.transform.SetParent(parentOfItems);
                Undo.RegisterCreatedObjectUndo(a, "Created " + a.name); // Add to undo stack
            }
            else
            {
                Debug.LogWarning("[Level Editor 0] Please select an item to place.");
            }
        }

        private void RemoveItem(Event e, SceneView scene)
        {
            Vector3 mousePos = e.mousePosition;
            float ppp = EditorGUIUtility.pixelsPerPoint;
            mousePos.y = scene.camera.pixelHeight - mousePos.y * ppp;
            mousePos.x *= ppp;

            Ray ray = scene.camera.ScreenPointToRay(mousePos);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit))
            {
                DestroyImmediate(hit.transform.gameObject);
            }
        }

        #endregion

        
    }

    /// <summary>
    /// When the user quits the application the gameobject in the scene needs to be destroyed
    /// </summary>
    [InitializeOnLoad]
    public class Toolbar_0_Quit
    { 
        private static void EditorQuit()
        {
            EditorApplication.quitting += Quit;
        }

        private static void Quit()
        {
            Debug.Log("[Level Editor 0] Quitting the Editor");
            // Check if the instance gameobject is still there => destroy it
            if(Toolbar_0_ExecuteInEditMode.Instance.gameObject != null)
                MonoBehaviour.DestroyImmediate(Toolbar_0_ExecuteInEditMode.Instance.gameObject);
        }
    }
}

// Links
// Mouse pos in sceneView https://forum.unity.com/threads/how-to-get-mouseposition-in-scene-view.208911/
// Snap grid https://answers.unity.com/questions/1446220/snap-object-to-grid-with-offset.html
// Mouse click from scene https://answers.unity.com/questions/1260602/how-to-get-mouse-click-world-position-in-the-scene.html
// Hide tools https://forum.unity.com/threads/hiding-default-transform-handles.86760/
// Disable right mouse drag https://gamedev.stackexchange.com/questions/179984/disable-default-unity-editor-behaviour-on-input
// Mouse down / drag / up https://forum.unity.com/threads/hurr-eventtype-mouseup-not-working-on-left-clicks.99909/

// Changing mouse icon https://docs.unity3d.com/ScriptReference/EditorGUIUtility.IconContent.html
// unity editor build in icons https://unitylist.com/p/5c3/Unity-editor-icons
#endif