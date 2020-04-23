using System.Collections.Generic;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine;
using UnityEngine.UI;

namespace BasicPractices
{
    public class ARTapToPlaceObjectOnIndicator : MonoBehaviour
    {
        public GameObject gameObjectToInstantiate;
        public Text logText;
        public GameObject placementIndicator;
        ARRaycastManager aRRaycastManager;
        Pose placementPose;
        bool placementPoseIsValid = false;
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        // Start is called before the first frame update
        void Start()
        {
            aRRaycastManager = FindObjectOfType<ARRaycastManager>();
            UpdatePlacementPose();
            UpdatePlacementIndicator();
        }

        private void UpdatePlacementPose()
        {
            Ray screenCenter = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            aRRaycastManager.Raycast(screenCenter, hits, TrackableType.PlaneWithinPolygon);
            placementPoseIsValid = hits.Count > 0;
            if (placementPoseIsValid)
            {
                placementPose = hits[0].pose;
                Vector3 cameraForward = Camera.main.transform.forward;
                Vector3 cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
                placementPose.rotation = Quaternion.LookRotation(cameraBearing);
            }
        }
        private void UpdatePlacementIndicator()
        {
            if (placementPoseIsValid)
            {
                logText.text = "Placement pose Valid, hits = " + hits.Count;
                placementIndicator.SetActive(true);
                placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
            }
            else
            {
                logText.text = "Placement pose Invalid, hits = " + hits.Count;
                placementIndicator.SetActive(false);
            }
        }


        // Update is called once per frame
        void Update()
        {
            UpdatePlacementPose();
            UpdatePlacementIndicator();
            if (placementPoseIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                PlaceObject();
            }

        }

        private void PlaceObject()
        {
            logText.text = "Object placed";
            Instantiate(gameObjectToInstantiate, placementPose.position, placementPose.rotation);
        }
    }

}