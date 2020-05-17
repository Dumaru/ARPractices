using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Gaze : MonoBehaviour
{
    List<InfoBehaviour> infoBehaviours = new List<InfoBehaviour>();
    Transform cameraTransform;
    // Start is called before the first frame update
    void Start()
    {
        cameraTransform = Camera.main.transform;
        //  TODO: ADD DYNAMICALLY
        infoBehaviours = FindObjectsOfType<InfoBehaviour>().ToList();
    }

    // Update is called once per frame
    void Update()
    {
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out RaycastHit hit))
        {
            if (hit.collider.gameObject.CompareTag("HasInfo"))
            {
                OpenInfo(hit.collider.gameObject.GetComponent<InfoBehaviour>());
            }
        }
        else
        {
            // No info behaviour was hitted
            CloseAllInfos();
        }
    }

    private void CloseAllInfos()
    {
        foreach (InfoBehaviour infoBehaviour in infoBehaviours)
        {
            infoBehaviour.CloseInfoSection();
        }
    }

    void OpenInfo(InfoBehaviour desiredInfo)
    {
        foreach (InfoBehaviour infoBehaviour in infoBehaviours)
        {
            if (infoBehaviour == desiredInfo)
            {
                infoBehaviour.OpenInfoSection();
            }
            else
            {
                infoBehaviour.CloseInfoSection();
            }
        }
    }
}
