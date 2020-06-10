using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;
using UnityEngine;
using System;

public class ARLessons : MonoBehaviour
{
    [SerializeField] TMPro.TMP_Text stateText;
    [SerializeField] TMPro.TMP_Text planesCounterText;
    [SerializeField] ARPlaneManager arPlaneMnager;
    [SerializeField] ARPointCloudManager arPointCloudManager;
    [SerializeField]
    ARRaycastManager arRaycastManager;
    [SerializeField]
    ARCameraManager arCameraManager;
    [SerializeField]
    Light light;
    [SerializeField]
    Image refImage;
    HashSet<ARPlane> arPlanes = new HashSet<ARPlane>();
    HashSet<ARPointCloud> arPointClouds = new HashSet<ARPointCloud>();
    int arPlanesCounter = 0;
    int arPointCloudsCounter = 0;
    [SerializeField]
    GameObject robotPrefab;
    GameObject robot;
    float avgBrightness;
    float avgColorTemp;
    Color colorCorrection;
    private void Awake()
    {
        ARSession.stateChanged += HandleARSessionStateChanged;
    }
    void Start()
    {
        arPlaneMnager.planesChanged += HandleARPlanesChangedEvent;
        arPointCloudManager.pointCloudsChanged += HandleARPointCloudsChangedEvent;
        arCameraManager.frameReceived += HandleFrameReceivedCallback;
    }

    private void HandleFrameReceivedCallback(ARCameraFrameEventArgs obj)
    {
        ARLightEstimationData lightEstimation = obj.lightEstimation;
        Debug.Log("Camera Frame received");
        avgBrightness = lightEstimation.averageBrightness != null ? (float)lightEstimation.averageBrightness : -1;
        avgColorTemp = lightEstimation.averageColorTemperature != null ? (float)lightEstimation.averageColorTemperature : -1;
        colorCorrection = lightEstimation.colorCorrection != null ? (Color)lightEstimation.colorCorrection : Color.black;
    }

    private void HandleARSessionStateChanged(ARSessionStateChangedEventArgs obj)
    {
        Debug.Log("AR Session state change: " + obj.state.ToString());
        stateText.text = "State: " + obj.state.ToString();
    }

    private void HandleARPointCloudsChangedEvent(ARPointCloudChangedEventArgs obj)
    {
        // arPointCloudsCounter += obj.added.Count;
        // arPointCloudsCounter -= obj.removed.Count;

        arPointClouds.UnionWith(obj.added);
        arPointClouds.UnionWith(obj.updated);
        arPointClouds.ExceptWith(obj.removed);
        arPointCloudsCounter = 0;
        foreach (ARPointCloud aRPointCloud in arPointClouds)
        {
            arPointCloudsCounter += aRPointCloud.positions.Length;
        }
        UpdatePlaneCounterText();
    }

    private void HandleARPlanesChangedEvent(ARPlanesChangedEventArgs obj)
    {
        // arPlanesCounter += obj.addeid.Count;
        // arPlanesCounter -= obj.remoived.Count;
        arPlanes.UnionWith(obj.added);
        arPlanes.UnionWith(obj.updated);
        arPlanes.ExceptWith(obj.removed);

        arPlanesCounter = arPlanes.Count;
        UpdatePlaneCounterText();

    }

    void UpdatePlaneCounterText()
    {
        String msg = "N planes: " + arPlanesCounter.ToString() + "\n" +
                    "N point clouds: " + arPointCloudsCounter.ToString();
        planesCounterText.text = msg;
    }

    // Update is called once per frame. 
    void Update()
    {
        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                Vector2 touchPos = Input.GetTouch(0).position;
                List<ARRaycastHit> hitResults = new List<ARRaycastHit>();
                if (arRaycastManager.Raycast(touchPos, hitResults, TrackableType.PlaneWithinBounds))
                {
                    Pose hitPose = hitResults[0].pose;
                    if (robot == null)
                    {
                        robot = Instantiate(robotPrefab, hitPose.position, hitPose.rotation);
                    }
                    else
                    {
                        robot.transform.position = hitPose.position;
                        robot.transform.rotation = hitPose.rotation;
                    }
                }

            }
        }

        CorrectLight();
    }

    private void CorrectLight()
    {
        if (avgBrightness != -1)
        {
            light.intensity = avgBrightness;
        }
        if (avgColorTemp != -1)
        {
            light.colorTemperature = avgColorTemp;
        }
        if (!colorCorrection.Equals(Color.black))
        {
            refImage.color = colorCorrection;
        }
    }
}
