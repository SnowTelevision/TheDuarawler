using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls a sliding door
/// </summary>
public class SlidingDoor : MonoBehaviour
{
    public bool requireKey; // Does this door needs an item that work as a key to open it
    public int keyCode; // The code of the key to check if the key matches this door
    public bool requirePass; // Does this door needs the player to put in a passcode to open it
    public int passcode; // The passcode this door is check againest when the player enters it
    public bool openLeft; // Does this door slides towards the negative(left) local x direction or the positive(right) local x direction

    public Coroutine controlDoorAnimationCoroutine; // The coroutine that runs the door's open&close animation
    public List<KeyInfo> touchingKeys; // The keys that are currently touching the door

    // Use this for initialization
    void Start()
    {
        if (openLeft)
        {
            transform.eulerAngles = transform.eulerAngles + new Vector3(0, 180, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (KeyInfo k in touchingKeys)
        {
            // Remove the key from the touching key list if the key is no longer holding by a character
            if (!k.GetComponent<ItemInfo>().isBeingHeld)
            {
                touchingKeys.Remove(k);
                // If the key is the last key that's being dropped then close the door
                if(touchingKeys.Count == 0)
                {
                    controlDoorAnimationCoroutine = StartCoroutine(GetComponentsInChildren<LinearObjectMovement>()[1].Animate());
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        bool openDoor = false;

        if (requireKey)
        {
            // If the item is a key and is currently holding by the player or some NPC
            if (other.GetComponent<KeyInfo>() && other.GetComponent<ItemInfo>().isBeingHeld)
            {
                if (other.GetComponent<KeyInfo>().keyCode == keyCode)
                {
                    openDoor = DetectIfUseKey();
                    touchingKeys.Add(other.GetComponent<KeyInfo>());
                }
            }
        }
        else if (requirePass)
        {

        }
        // If the door don't require any item or code
        else
        {
            // If the player is close to the door then open it
            if (other.gameObject.layer == LayerMask.NameToLayer("PlayerBody"))
            {
                openDoor = true;
            }
        }

        if (openDoor)
        {
            if (controlDoorAnimationCoroutine != null)
            {
                StopCoroutine(controlDoorAnimationCoroutine);
            }

            controlDoorAnimationCoroutine = StartCoroutine(GetComponentsInChildren<LinearObjectMovement>()[0].Animate());
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // If this door needs a key to open and a key touching it is just picked up by someone
        if (requireKey && other.GetComponent<KeyInfo>() &&
            other.GetComponent<KeyInfo>().keyCode == keyCode &&
            !touchingKeys.Contains(other.GetComponent<KeyInfo>()) &&
            other.GetComponent<ItemInfo>().isBeingHeld)
        {
            // If the key is the only key touching it
            if (DetectIfUseKey())
            {
                controlDoorAnimationCoroutine = StartCoroutine(GetComponentsInChildren<LinearObjectMovement>()[0].Animate());
            }
            // Add the key back to the touching key list
            touchingKeys.Add(other.GetComponent<KeyInfo>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        bool closeDoor = false;

        if (requireKey)
        {
            // If the item is a key and is currently holding by the player or some NPC
            if (other.GetComponent<KeyInfo>() && other.GetComponent<ItemInfo>().isBeingHeld)
            {
                // If the key code match with the door
                if (other.GetComponent<KeyInfo>().keyCode == keyCode)
                {
                    // Remove the key from the touching key list
                    touchingKeys.Remove(other.GetComponent<KeyInfo>());
                    closeDoor = DetectIfUseKey();
                }
            }
        }
        else if (requirePass)
        {

        }
        // If the door don't require any item or code
        else
        {
            // If the player is leaving the door then close it
            if (other.gameObject.layer == LayerMask.NameToLayer("PlayerBody"))
            {
                closeDoor = true;
            }
        }

        if (closeDoor)
        {
            if (controlDoorAnimationCoroutine != null)
            {
                StopCoroutine(controlDoorAnimationCoroutine);
            }

            controlDoorAnimationCoroutine = StartCoroutine(GetComponentsInChildren<LinearObjectMovement>()[1].Animate());
        }
    }

    /// <summary>
    /// When a key start/stop touching the door, detect if anything happenes to the door
    /// </summary>
    /// <returns></returns>
    public bool DetectIfUseKey()
    {
        if (touchingKeys.Count == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
