using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SLIDDES.Controllers
{
    public class CameraController : MonoBehaviour
    {
        public CameraMode sideScroller;

        [Header("Tweak Values")]
        /// <summary>
        /// The smoothspeed at which the camera moves
        /// </summary>
        public float cameraSmoothSpeed = 0.125f;
        /// <summary>
        /// The offset of the camera from the target
        /// </summary>
        public Vector3 cameraOffset;
        /// <summary>
        /// The target of the camera
        /// </summary>
        public Transform target;

        /// <summary>
        /// Used as ref value
        /// </summary>
        private Vector3 velocity = Vector3.zero;

        // Start is called before the first frame update
        void Start()
        {
            if(target == null)
            {
                // Find player tag object
                target = GameObject.FindWithTag("Player").transform;
                if(target == null)
                {
                    Debug.LogWarning("[Camera Controller] No target...");
                }
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void FixedUpdate()
        {
            FollowTarget();
        }

        /// <summary>
        /// Follows the assigned target
        /// </summary>
        private void FollowTarget()
        {
            if(target == null) return;

            Vector3 desiredPos = target.transform.position + cameraOffset;

            #region WN camera limit stuff
            //Vector3 offsetBottom = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)); // The position of the camera bottom line
            //Vector3 offsetFromCenter = (Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, 0)) - Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0))) * 0.5f;

            //print(offsetFromCenter);
            //if((desiredPos.y + offsetFromCenter.y) >= cameraPaths[gameManager.currentArea].maxTransform.position.y)
            //{
            //    // Limit top
            //    desiredPos = new Vector3(desiredPos.x, cameraPaths[gameManager.currentArea].maxTransform.position.y - offsetFromCenter.y, 0);
            //}
            //else if((desiredPos.y - offsetFromCenter.y) < cameraPaths[gameManager.currentArea].minTransform.position.y)
            //{
            //    // Limit bottom
            //    desiredPos = new Vector3(desiredPos.x, cameraPaths[gameManager.currentArea].minTransform.position.y + offsetFromCenter.y, 0);

            //    // Limit top again if user makes screen very thin
            //    if((desiredPos.y + offsetFromCenter.y) >= cameraPaths[gameManager.currentArea].maxTransform.position.y)
            //    {
            //        // Limit top again
            //        desiredPos = new Vector3(desiredPos.x, cameraPaths[gameManager.currentArea].maxTransform.position.y - offsetFromCenter.y, 0);
            //    }
            //}
            #endregion

            Vector3 smoothPos = Vector3.SmoothDamp(transform.position, desiredPos, ref velocity, cameraSmoothSpeed);
            transform.position = smoothPos;
        }
    }

    public enum CameraMode
    {
        sideScroller
    }
}