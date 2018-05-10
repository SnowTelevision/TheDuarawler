﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInfo : MonoBehaviour
{
    public bool canUse; // Is this an usable item? If it is not then the player has to keep holding down the grab item button to hold it
    public bool smallItem; // Is this an small item? If it is then it should be able to be lifted by the arm and not touching the ground

    public float normalDrag; // The item's normal drag
    public float normalAngularDrag; // The item's normal angular drag
    public float normalMass; // The item's normal mass
    public bool isBeingHeld; // If this item is currently being held by an arm
    public Transform holdingArm; // The arm that is holding this item

    // Use this for initialization
    void Start()
    {
        if (GetComponent<Rigidbody>())
        {
            normalDrag = GetComponent<Rigidbody>().drag;
            normalAngularDrag = GetComponent<Rigidbody>().angularDrag;
            normalMass = GetComponent<Rigidbody>().mass;
        }
    }

    // Update is called once per frame
    void Update()
    {
        CalculatingItemTransform();
    }

    public void CalculatingItemTransform()
    {
        if (isBeingHeld)
        {
            if (smallItem && canUse)
            {
                GetComponent<Rigidbody>().velocity = Vector3.zero;
                GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                transform.rotation = holdingArm.rotation;
                transform.position = holdingArm.position;
                //print(holdingArm.position);
            }
        }
    }
}
