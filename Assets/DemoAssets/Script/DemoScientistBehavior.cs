using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the dumb scientist in the magic tutorial demo
/// </summary>
public class DemoScientistBehavior : MonoBehaviour
{
    public GameObject key; // The door key

    // Use this for initialization
    void Start()
    {
        StartCoroutine(GetComponent<LinearObjectMovement>().Animate());
        GetComponentInChildren<Actions>().Walk();
    }

    // Update is called once per frame
    void Update()
    {
        // Drop the key if the scientist enters the room
        if (transform.position.z < 10.1f &&
            !GetComponent<LinearObjectMovement>().isAnimationRunning &&
            key.transform.parent != null)
        {
            GetComponentInChildren<Actions>().Stay();
            key.GetComponent<ItemInfo>().isBeingHeld = false;
            key.GetComponent<Rigidbody>().isKinematic = false;
            key.transform.parent = null;
        }

        //
        if (transform.position.z > 10 &&
            key.transform.parent == null &&
            GetComponent<LinearObjectMovement>().isAnimationRunning)
        {
            transform.eulerAngles += new Vector3(0, 180, 0);
            GetComponentInChildren<Actions>().Run();
            Destroy(gameObject, 10);
            this.enabled = false;
        }
    }
}
