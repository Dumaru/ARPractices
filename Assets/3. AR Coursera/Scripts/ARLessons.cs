using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine;
using System;

public class ARLessons : MonoBehaviour
{
    [SerializeField] TMPro.TMP_Text stateText;
    [SerializeField] TMPro.TMP_Text planesCounterText;
    [SerializeField] ARPlaneManager arPlaneMnager;
    [SerializeField] ARPointCloudManager arPointCloudManager;
    HashSet<ARPlane> arPlanes = new HashSet<ARPlane>();
    HashSet<ARPointCloud> arPointClouds = new HashSet<ARPointCloud>();
    int arPlanesCounter = 0;
    int arPointCloudsCounter = 0;
    private void Awake()
    {
        ARSession.stateChanged += HandleARSessionStateChanged;
    }
    void Start()
    {
        arPlaneMnager.planesChanged += HandleARPlanesChangedEvent;
        arPointCloudManager.pointCloudsChanged += HandleARPointCloudsChangedEvent;
    }



    private void HandleARSessionStateChanged(ARSessionStateChangedEventArgs obj)
    {
        Debug.Log("AR Session state change: " + obj.state.ToString());
        stateText.text = "State: " + obj.state.ToString();
    }

    private void HandleARPointCloudsChangedEvent(ARPointCloudChangedEventArgs obj)
    {
        arPointCloudsCounter += obj.added.Count;
        arPointCloudsCounter -= obj.removed.Count;

        arPointClouds.UnionWith(obj.added);
        arPointClouds.UnionWith(obj.updated);
        arPointClouds.ExceptWith(obj.removed);
        UpdatePlaneCounterText();
    }

    private void HandleARPlanesChangedEvent(ARPlanesChangedEventArgs obj)
    {
        arPlanesCounter += obj.added.Count;
        arPlanesCounter -= obj.removed.Count;
        arPlanes.UnionWith(obj.added);
        arPlanes.UnionWith(obj.updated);
        arPlanes.ExceptWith(obj.removed);
        UpdatePlaneCounterText();

    }

    void UpdatePlaneCounterText()
    {
        String msg = "N planes: " + arPlanesCounter.ToString() + "\n" +
                    "N point clouds: " + arPointCloudsCounter.ToString();
        planesCounterText.text = msg;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
