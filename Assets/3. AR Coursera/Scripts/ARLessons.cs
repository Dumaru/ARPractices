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
    GameObject ballPrefab;
    [SerializeField]
    Image refImage;
    HashSet<ARPlane> arPlanes = new HashSet<ARPlane>();
    HashSet<ARPointCloud> arPointClouds = new HashSet<ARPointCloud>();
    [SerializeField]
    GameObject robotPrefab;
    [SerializeField]
    Button fireButton;
    RectTransform fireButtonRectTransform;
    float halfButtonHeight;
    float halfButtonWidth;
    int arPlanesCounter = 0;
    int arPointCloudsCounter = 0;
    GameObject robot;
    private Rigidbody robotRb;
    float avgBrightness;
    float avgColorTemp;
    Color colorCorrection;
    private bool overButton;

    private void Awake()
    {
        ARSession.stateChanged += HandleARSessionStateChanged;
    }
    void Start()
    {
        arPlaneMnager.planesChanged += HandleARPlanesChangedEvent;
        arPointCloudManager.pointCloudsChanged += HandleARPointCloudsChangedEvent;
        arCameraManager.frameReceived += HandleFrameReceivedCallback;

        fireButtonRectTransform = fireButton.GetComponent<RectTransform>();
        halfButtonWidth = fireButtonRectTransform.rect.width / 2;
        halfButtonHeight = fireButtonRectTransform.rect.height / 2;
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
            if (Input.GetTouch(0).position.x < fireButtonRectTransform.position.x + halfButtonWidth &&
                Input.GetTouch(0).position.x > fireButtonRectTransform.position.x - halfButtonWidth &&
                Input.GetTouch(0).position.y < fireButtonRectTransform.position.y + halfButtonHeight &&
                Input.GetTouch(0).position.y > fireButtonRectTransform.position.y - halfButtonHeight
            ) overButton = true;
            else overButton = false;

            Debug.Log("Rayo " + fireButtonRectTransform.rect.position.ToString() + " over button " + overButton);
            if (Input.GetTouch(0).phase == TouchPhase.Ended && !overButton)
            {
                Debug.Log("Rayo FIN >>>>>>>>>>>>>>>>>>>>>>");
                Vector2 touchPos = Input.GetTouch(0).position;
                List<ARRaycastHit> hitResults = new List<ARRaycastHit>();
                if (arRaycastManager.Raycast(touchPos, hitResults, TrackableType.PlaneWithinBounds))
                {
                    Pose hitPose = hitResults[0].pose;
                    if (robot == null)
                    {
                        robot = Instantiate(robotPrefab, hitPose.position, hitPose.rotation);
                        robotRb = robot.GetComponent<Rigidbody>();
                        robotRb.isKinematic = true;
                    }
                    else
                    {
                        robot.transform.position = hitPose.position;
                        robot.transform.rotation = hitPose.rotation;
                        robotRb.isKinematic = false;
                        robotRb.velocity = Vector3.zero;
                        robotRb.angularVelocity = Vector3.zero;
                        Rigidbody[] rbChildren = robot.GetComponentsInChildren<Rigidbody>();
                        foreach (Rigidbody rb in rbChildren)
                        {
                            rb.isKinematic = false;
                            rb.velocity = Vector3.zero;
                            rb.angularVelocity = Vector3.zero;
                        }
                    }
                }

            }
        }

        CorrectLight();
    }

    public void ShootBall()
    {
        overButton = true;
        GameObject tempBall = Instantiate(ballPrefab, Camera.main.transform.position, Camera.main.transform.rotation);
        tempBall.GetComponent<Rigidbody>().AddForce(Camera.main.transform.forward * 5000, ForceMode.Force);
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


    public void HandlePointerEnter()
    {
        this.overButton = true;
    }
    public void HandlePointerExit()
    {
        this.overButton = false;
    }
}
