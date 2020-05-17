using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class FaceCamera : MonoBehaviour
{
    Transform cameraTranform;
    Vector3 desiredAngle;
    // Start is called before the first frame update
    void Start()
    {
        cameraTranform = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(cameraTranform);
        desiredAngle = transform.eulerAngles;
        desiredAngle.x = 0;
        desiredAngle.z = 0;
        transform.eulerAngles = desiredAngle;
    }
}
