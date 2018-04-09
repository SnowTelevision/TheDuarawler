using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Detects collision
/// </summary>
public class DetectCollision : MonoBehaviour
{
    public bool isColliding; // If the arm is colliding something
    public Vector3 collidingPoint; // The colliding position

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionStay(Collision collision)
    {
        isColliding = true;
        collidingPoint = collision.contacts[0].point;
    }

    private void OnCollisionExit(Collision collision)
    {
        isColliding = false;
    }
}
