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
        private Vector3 mousePositionScene;

        private void Awake()
        {
            // Set instance
            if(Instance != null) Destroy(Instance.gameObject);
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

        private void GetMousePositionScene()
        {
            Vector3 mousePosition = Event.current.mousePosition;
            Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);
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

        private void OnDrawGizmos()
        {
            // Get mouse position
            //GetMousePositionScene();
            
            // Draw cube
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(mousePositionScene, Vector3.one);
        }

        private void OnSceneGUI()
        {
            Debug.Log("scene");
            //Vector3 mousePosition = Event.current.mousePosition;
            //Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);
            //mousePositionScene = ray.origin;
            //Vector3 mousePosition = Event.current.mousePosition;
            //mousePosition.y = SceneView.currentDrawingSceneView.camera.pixelHeight - mousePosition.y;
            //mousePosition = SceneView.currentDrawingSceneView.camera.ScreenToWorldPoint(mousePosition);
            //mousePosition.y = -mousePosition.y;
            //mousePositionScene = mousePosition;
        }

        void OnEnable()
        {
            SceneView.duringSceneGui += this.OnScene;
            Tools.hidden = true;
            // Assign parent
            CreateParentItems();
        }

        private void OnDestroy()
        {
            SceneView.duringSceneGui -= this.OnScene;
            Tools.hidden = false;
        }

        void OnScene(SceneView scene)
        {
            GetMousePositionScene();
            // move custom gameobject here to show mousepos as alternative of Gizmos

            Event e = Event.current;

            // Input mouse button left (Add)
            if(e.type == EventType.MouseDown && e.button == 0)
            {
                // Place item                
                if(objectToCreate != null)
                {
                    GameObject a = Instantiate(objectToCreate, mousePositionScene, Quaternion.identity);
                    if(parentOfItems == null) CreateParentItems();
                    a.transform.SetParent(parentOfItems);
                }
                else
                {
                    Debug.LogWarning("[Level Editor 0] Please select an item to place.");
                }                
            }

            // Input mouse button right (Remove)
            if(e.type == EventType.MouseDown && e.button == 1)
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
                //e.Use();
            }
        }
    }
}

// Links
// Mouse pos in sceneView https://forum.unity.com/threads/how-to-get-mouseposition-in-scene-view.208911/
// Snap grid https://answers.unity.com/questions/1446220/snap-object-to-grid-with-offset.html
// Mouse click from scene https://answers.unity.com/questions/1260602/how-to-get-mouse-click-world-position-in-the-scene.html
// Hide tools https://forum.unity.com/threads/hiding-default-transform-handles.86760/
#endif