using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Logics for the simple pistol bullet
/// </summary>
public class SimplePistolBullet : MonoBehaviour
{


    public SimplePistol owner; // The pistol that shot it

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<SimplePistol>())
        {
            GetComponent<Collider>().enabled = true;
        }
    }
}
