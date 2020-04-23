using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace BasicPractices
{

    [RequireComponent(typeof(ARRaycastManager))]
    public class ARTapToPlaceObject : MonoBehaviour
    {
        public GameObject gameObjectToInstantiate;
        GameObject spawnedObject;
        ARRaycastManager aRRaycastManager;
        Vector2 touchPosition;
        static List<ARRaycastHit> hits = new List<ARRaycastHit>();
        private void Awake()
        {
            aRRaycastManager = GetComponent<ARRaycastManager>();
        }

        bool TryGetTouchPosition(out Vector2 touchPos)
        {
            if (Input.touchCount > 0)
            {
                touchPos = Input.GetTouch(0).position;
                return true;
            }
            touchPos = default;
            return false;
        }

        private void Update()
        {
            if (!TryGetTouchPosition(out touchPosition))
            {
                return;
            }
            if (aRRaycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
            {
                Pose hitPose = hits[0].pose;
                if (spawnedObject == null)
                {
                    spawnedObject = Instantiate(gameObjectToInstantiate, hitPose.position, hitPose.rotation);
                }
                else
                {
                    spawnedObject.transform.position = hitPose.position;
                    spawnedObject.transform.rotation = hitPose.rotation;
                }
            }
        }
    }
}