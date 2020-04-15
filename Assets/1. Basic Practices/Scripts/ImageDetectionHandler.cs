using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;


public class ImageDetectionHandler : MonoBehaviour
{
    private ARTrackedImageManager _arTrackedImageManager;
    // Start is called before the first frame update
    void Awake()
    {
        _arTrackedImageManager = FindObjectOfType<ARTrackedImageManager>();
    }

    private void OnEnable()
    {
        // Subscribes to the event with an evnet handler unity action delegate
        _arTrackedImageManager.trackedImagesChanged += HandleImageChange;
    }
    private void OnDisable()
    {
        // Unsubscribes to the event with an evnet handler unity action delegate
        _arTrackedImageManager.trackedImagesChanged -= HandleImageChange;
    }

    private void HandleImageChange(ARTrackedImagesChangedEventArgs args)
    {
        foreach (ARTrackedImage image in args.added)
        {
            Debug.Log("Image tracked " + image.name);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
