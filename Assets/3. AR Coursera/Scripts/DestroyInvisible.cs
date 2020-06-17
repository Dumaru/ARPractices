using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyInvisible : MonoBehaviour
{
    // Start is called before the first frame update
    float timeToDestroy = 5;
    float current;
    bool destroyGo;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (destroyGo)
        {
            current -= Time.deltaTime;
            if (current < 0)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnBecameInvisible()
    {
        destroyGo = true;
        current = timeToDestroy;
    }

    private void OnBecameVisible()
    {
        destroyGo = false;
    }

    private void OnDestroy()
    {
        Debug.Log("Me destruyeron =(");
    }
}
