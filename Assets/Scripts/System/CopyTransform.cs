using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Copy an object's transform
/// </summary>
public class CopyTransform : MonoBehaviour
{
    public bool copyRotation;
    public bool copyPosition;
    public Transform copyFrom;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(copyRotation)
        {
            transform.rotation = copyFrom.rotation;
        }
        if(copyPosition)
        {
            transform.position = copyFrom.position;
        }
    }
}
