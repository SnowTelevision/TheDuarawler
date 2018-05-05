using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the basic movement of the player arms
/// </summary>
public class ControlArm : MonoBehaviour
{
    public bool isLeftArm; // Is this the left arm
    public Transform armTip; // The tip of the arm
    public Transform arm; // The actual arm that extends from the center of the body to the arm tip
    public float armMaxLength; // How long is the maximum arm length
    public Transform bodyRotatingCenter; // What is the center that the body is rotating around
    public Transform body; // The main body
    public DetectCollision bodyWallDetector; // The collider on the body that detects the walls
    public LayerMask armCollidingLayer; // The object layers that the arm can collide with
    public LayerMask bodyCollidingLayer; // The object layers that the body can collide with
    public ControlArm otherArm; // The other arm
    public float triggerThreshold; // How much the trigger has to be pressed down to register

    public bool isGrabbingFloor; // If the armTip is grabbing floor
    public float joyStickRotationAngle; // The rotation of the arm
    public float joyStickLength; // How much the joystick is pushed away

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        CalculateJoyStickRotation(isLeftArm);
        CalculateJoyStickLength(isLeftArm);

        if (!isGrabbingFloor)
        {
            RotateArm(isLeftArm);
            StretchArm(isLeftArm);
            DetectIfPickingUpItem();

            if (armTip.GetComponent<ArmUseItem>().currentlyHoldingItem != null)
            {
                DetectIfDropUnusableItem();
            }
        }
        //else
        //{
        //    armTip.position = bodyRotatingCenter.position;
        //}

        DetectGrabbingFloorInput();

        if (isGrabbingFloor)
        {
            armTip.position = bodyRotatingCenter.position;
            RotateBody();
            MoveBody();
        }

        UpdateArmTransform();

        // Test
        //TestControllerInput();
    }

    /// <summary>
    /// Detect if the player want to start grabbing the floor with the armTip, and see if can grab or not
    /// If the armTip is currently holding an usable item, then drop the item first. The player need to press
    /// the grab button again to start grabbing floor
    /// </summary>
    public void DetectGrabbingFloorInput()
    {
        if (isLeftArm)
        {
            // If the left armTip start grabbing the floor
            if (Input.GetKeyDown(KeyCode.JoystickButton4))
            {
                // If the armTip is currently empty and not holding any item, then start grabbing the ground
                if (armTip.GetComponent<ArmUseItem>().currentlyHoldingItem == null)
                {
                    StartGrabbing();
                }
                else // Drop the current holding item (no matter it can be used or not
                {
                    DropDownItem();
                }
            }
            if (Input.GetKeyUp(KeyCode.JoystickButton4))
            {
                // If the armTip is currently empty and not holding any item, then stop grabbing the ground
                if (armTip.GetComponent<ArmUseItem>().currentlyHoldingItem == null)
                {
                    StopGrabbing();
                }
            }
        }

        if (!isLeftArm)
        {
            // If the right armTip start grabbing the floor
            if (Input.GetKeyDown(KeyCode.JoystickButton5))
            {
                // If the armTip is currently empty and not holding any item, then start grabbing the ground
                if (armTip.GetComponent<ArmUseItem>().currentlyHoldingItem == null)
                {
                    StartGrabbing();
                }
                else // Drop the current holding item (no matter it can be used or not
                {
                    DropDownItem();
                }
            }
            if (Input.GetKeyUp(KeyCode.JoystickButton5))
            {
                // If the armTip is currently empty and not holding any item, then stop grabbing the ground
                if (armTip.GetComponent<ArmUseItem>().currentlyHoldingItem == null)
                {
                    StopGrabbing();
                }
            }
        }
    }

    /// <summary>
    /// Detect if the armTip is colliding with an item, and if the player want to pick up an item
    /// </summary>
    public void DetectIfPickingUpItem()
    {
        // If the arm tip is colliding with an item
        if (armTip.GetComponent<DetectCollision>().collidingObject != null &&
            armTip.GetComponent<DetectCollision>().collidingObject.GetComponent<ItemInfo>())
        {
            // Picking or dropping item
            // Usable item is click trigger to pick up, click shoulder to drop
            // Other item is hold down trigger to pick up, release to drop

            if (isLeftArm)
            {
                // If the left armTip is not holding an item and the left trigger is pressed down
                if (armTip.GetComponent<ArmUseItem>().currentlyHoldingItem == null && Input.GetAxis("LeftTrigger") >= triggerThreshold)
                {
                    StartCoroutine(PickUpItem());
                }
            }

            if (!isLeftArm)
            {
                // If the right armTip is not holding an item and the right trigger is pressed down
                if (armTip.GetComponent<ArmUseItem>().currentlyHoldingItem == null && Input.GetAxis("RightTrigger") >= triggerThreshold)
                {
                    StartCoroutine(PickUpItem());
                }
            }
        }
    }

    /// <summary>
    /// Detect if the player release the trigger to drop an unusable item
    /// </summary>
    public void DetectIfDropUnusableItem()
    {
        if (isLeftArm)
        {
            // If the left armTip is holding an unusable item and the trigger is lifting up
            if (!armTip.GetComponent<ArmUseItem>().currentlyHoldingItem.GetComponent<ItemInfo>().canUse &&
                Input.GetAxis("LeftTrigger") < triggerThreshold)
            {
                DropDownItem();
            }

        }

        if (!isLeftArm)
        {
            // If the right armTip is holding an unusable item and the trigger is lifting up
            if (!armTip.GetComponent<ArmUseItem>().currentlyHoldingItem.GetComponent<ItemInfo>().canUse &&
                Input.GetAxis("RightTrigger") < triggerThreshold)
            {
                DropDownItem();
            }
        }
    }

    /// <summary>
    /// Start picking up the item
    /// </summary>
    public IEnumerator PickUpItem()
    {
        // Assign the item that is currently colliding with the armTip to the armTip's current holding item
        armTip.GetComponent<ArmUseItem>().currentlyHoldingItem = armTip.GetComponent<DetectCollision>().collidingObject;

        // If the other armTip is currently holding the item which is going to be holding by this armTip, then let the other arm drop the item first
        if (otherArm.armTip.GetComponent<ArmUseItem>().currentlyHoldingItem == armTip.GetComponent<DetectCollision>().collidingObject)
        {
            otherArm.DropDownItem();
        }

        yield return new WaitForEndOfFrame();

        // Add a fixed joint to the holding item to attach it to the armTip
        armTip.GetComponent<ArmUseItem>().currentlyHoldingItem.gameObject.AddComponent<FixedJoint>();
        armTip.GetComponent<ArmUseItem>().currentlyHoldingItem.GetComponent<FixedJoint>().connectedBody = armTip.GetComponent<Rigidbody>();
        armTip.GetComponent<ArmUseItem>().currentlyHoldingItem.GetComponent<FixedJoint>().autoConfigureConnectedAnchor = true;
    }

    /// <summary>
    /// Start drop down the item
    /// </summary>
    public void DropDownItem()
    {
        // Destroy the fixed joint in the item that's currently holding by the armTip
        Destroy(armTip.GetComponent<ArmUseItem>().currentlyHoldingItem.GetComponent<FixedJoint>());
        // Remove the dropping item from armTip's currentHoldingItem
        armTip.GetComponent<ArmUseItem>().currentlyHoldingItem = null;
    }

    /// <summary>
    /// Test the controller's input
    /// </summary>
    public void TestControllerInput()
    {
        //print("HorizontalLeft: " + Input.GetAxis("HorizontalLeft"));
        //print("VerticalLeft: " + Input.GetAxis("VerticalLeft"));
        //print("HorizontalRight: " + Input.GetAxis("HorizontalRight"));
        //print("VerticalRight: " + Input.GetAxis("VerticalRight"));

        //float rotation = Mathf.Atan(Input.GetAxis("HorizontalLeft") / Mathf.Abs(Input.GetAxis("VerticalLeft"))) * Mathf.Rad2Deg;
        //if (Input.GetAxis("VerticalLeft") < 0)
        //{
        //    rotation = Mathf.Sign(Input.GetAxis("HorizontalLeft")) * (180 - Mathf.Abs(rotation));
        //}
        //print(rotation);
        //print(Mathf.Sqrt(Mathf.Pow(Input.GetAxis("HorizontalLeft"), 2) + Mathf.Pow(Input.GetAxis("VerticalLeft"), 2)));

        //print(Input.GetKey(KeyCode.JoystickButton4)); // LB
        //print(Input.GetKey(KeyCode.JoystickButton5)); // RB
        print("Left: " + Input.GetAxis("LeftTrigger"));
        print("Right: " + Input.GetAxis("RightTrigger"));
    }

    /// <summary>
    /// Calculate the euler angles of the arm relative to the world
    /// </summary>
    /// <param name="left"></param>
    public void CalculateJoyStickRotation(bool left)
    {
        // Calculate the euler angles of the arm relative to the world
        if (left)
        {
            joyStickRotationAngle = Mathf.Atan(Input.GetAxis("HorizontalLeft") / Mathf.Abs(Input.GetAxis("VerticalLeft"))) * Mathf.Rad2Deg;
            if (Input.GetAxis("VerticalLeft") < 0)
            {
                joyStickRotationAngle = Mathf.Sign(Input.GetAxis("HorizontalLeft")) * (180 - Mathf.Abs(joyStickRotationAngle));
            }
        }
        else
        {
            joyStickRotationAngle = Mathf.Atan(Input.GetAxis("HorizontalRight") / Mathf.Abs(Input.GetAxis("VerticalRight"))) * Mathf.Rad2Deg;
            if (Input.GetAxis("VerticalRight") < 0)
            {
                joyStickRotationAngle = Mathf.Sign(Input.GetAxis("HorizontalRight")) * (180 - Mathf.Abs(joyStickRotationAngle));
            }
        }
    }

    /// <summary>
    /// Calculate how far the joy stick is away from the center
    /// </summary>
    /// <param name="left"></param>
    public void CalculateJoyStickLength(bool left)
    {
        // Calculate how far the joy stick is away from the center
        if (left)
        {
            joyStickLength = Mathf.Clamp01(Mathf.Sqrt(Mathf.Pow(Input.GetAxis("HorizontalLeft"), 2) + Mathf.Pow(Input.GetAxis("VerticalLeft"), 2)));
        }
        else
        {
            joyStickLength = Mathf.Clamp01(Mathf.Sqrt(Mathf.Pow(Input.GetAxis("HorizontalRight"), 2) + Mathf.Pow(Input.GetAxis("VerticalRight"), 2)));
        }
    }

    public void UpdateArmTransform()
    {
        arm.localPosition = armTip.localPosition / 2f; // Put the center of the arm in the middle between the armTip and the body center
        //arm.localScale = new Vector3(1, 1, armTip.localPosition.z / 2f); // Extend the arm towards the armTip
        arm.localScale = new Vector3(0.1f, armTip.localPosition.z / 2f, 0.1f); // Extend the arm towards the armTip
    }

    /// <summary>
    /// Rotate the arm according to the joystick's rotation
    /// </summary>
    /// <param name="left"></param>
    public void RotateArm(bool left)
    {
        //Vector3 previousEuler = transform.eulerAngles;
        //Vector3 previousArmTipPosition = armTip.position;

        // Rotate the arm according to the calculated rotation
        transform.eulerAngles = new Vector3(0, joyStickRotationAngle, 0);

        //if (isColliding)
        //{
        //    // Don't rotate if the armTip will go into collider
        //    if (Vector3.SqrMagnitude(armTip.position - collidingPoint) <= Vector3.SqrMagnitude(previousArmTipPosition - collidingPoint))
        //    {
        //        transform.eulerAngles = previousEuler;
        //    }
        //}
    }

    /// <summary>
    /// Stretch the arm according to the joystick's tilt
    /// </summary>
    /// <param name="left"></param>
    public void StretchArm(bool left)
    {
        //Vector3 previousArmTipPosition = armTip.position;

        // Calculate how long the arm extends
        armTip.localPosition = new Vector3(0, 0, joyStickLength * armMaxLength);

        if (arm.GetComponentInChildren<DetectCollision>().isColliding)
        //if (armTip.GetComponent<DetectCollision>().isColliding)
        {
            RaycastHit hit;
            // Don't extend if the armTip will go into collider
            if (Physics.Raycast(transform.position, transform.forward, out hit, armMaxLength + armTip.localScale.x / 2f, armCollidingLayer))
            {
                // If the ray hits the object that is currently holding by the armTip, then ignore it, don't retract the arm
                if (armTip.GetComponent<ArmUseItem>().currentlyHoldingItem != null &&
                    hit.transform != armTip.GetComponent<ArmUseItem>().currentlyHoldingItem.transform &&
                    hit.transform != arm)
                {
                    armTip.localPosition =
                        //new Vector3(0, 0, hit.distance - armTip.localScale.x / Mathf.Cos(Vector3.Angle(hit.normal, transform.forward) * Mathf.Deg2Rad));
                        new Vector3(0, 0, hit.distance);
                }
            }
        }
    }

    /// <summary>
    /// When this armTip start grabbing floor
    /// </summary>
    public void StartGrabbing()
    {
        isGrabbingFloor = true;
        if (otherArm.isGrabbingFloor)
        {
            otherArm.isGrabbingFloor = false;
            body.SetParent(null, true);
        }

        bodyRotatingCenter.position = armTip.position;
        bodyRotatingCenter.eulerAngles = new Vector3(0, joyStickRotationAngle - (Mathf.Sign(joyStickRotationAngle) * 180), 0);
        body.SetParent(bodyRotatingCenter, true);
    }

    /// <summary>
    /// When this armTip stop grabbing floor
    /// </summary>
    public void StopGrabbing()
    {
        if (isGrabbingFloor)
        {
            isGrabbingFloor = false;
            //body.parent = null;
            body.SetParent(null, true);
        }
    }

    /// <summary>
    /// Rotate the body around the armTip
    /// </summary>
    public void RotateBody()
    {
        bodyRotatingCenter.eulerAngles = new Vector3(0, joyStickRotationAngle - (Mathf.Sign(joyStickRotationAngle) * 180), 0);
    }

    /// <summary>
    /// Move the body around the armTip
    /// </summary>
    public void MoveBody()
    {
        body.localPosition = new Vector3(0, 0, joyStickLength * armMaxLength);

        if (bodyWallDetector.isColliding)
        {
            RaycastHit hit;
            // Don't extend if the armTip will go into collider
            if (Physics.Raycast(bodyRotatingCenter.position, bodyRotatingCenter.forward, out hit, armMaxLength + body.localScale.x, bodyCollidingLayer))
            {
                body.localPosition =
                    new Vector3(0, 0, hit.distance - body.localScale.x / 2f / Mathf.Cos(Vector3.Angle(hit.normal, transform.forward) * Mathf.Deg2Rad));
            }
        }
    }

    /// <summary>
    /// sudo:
    /// On rotating arm
    ///     if armTip collide obstacle
    ///         if rotate left && body is on the right side of the normal of the colliding surface ||
    ///            rotate right && body is on the left side of the normal of the colliding surface
    ///         { don't keep rotate}
    ///         
    /// On extending arm
    ///     if armTip collide obstacle
    ///     { don't keep extending}
    /// 
    /// 2nd method:
    /// On rotating or extending arm:
    ///     predict the armTip's position in the next frame
    ///         if the armTip is currently colliding object & the predicted position after rotate or extend is closer to the colliding position
    ///         { don't apply rotation or extend }
    ///         
    /// 3rd method:
    /// raycast, if raycast hit distance is shorter than the controller pointed position, then only extend armTip to the raycast hit position
    /// </summary>
}
