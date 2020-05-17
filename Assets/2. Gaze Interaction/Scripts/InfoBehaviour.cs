using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoBehaviour : MonoBehaviour
{
    public const float SPEED = 6;
    [SerializeField]
    Transform sectionInfo;
    Vector3 desiredScale = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        sectionInfo.localScale = Vector3.Lerp(sectionInfo.localScale, desiredScale, Time.deltaTime * SPEED);
    }

    public void CloseInfoSection()
    {
        desiredScale = Vector3.zero;
    }
    public void OpenInfoSection()
    {
        desiredScale = Vector3.one;
    }
}
