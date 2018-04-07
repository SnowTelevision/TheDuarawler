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

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        RotateArm(isLeftArm);
        StretchArm(isLeftArm);

        // Test
        TestControllerInput();
    }

    /// <summary>
    /// Test the controller's input
    /// </summary>
    public void TestControllerInput()
    {
        print("HorizontalLeft: " + Input.GetAxis("HorizontalLeft"));
        print("VerticalLeft: " + Input.GetAxis("VerticalLeft"));
        //print("HorizontalRight: " + Input.GetAxis("HorizontalRight"));
        //print("VerticalRight: " + Input.GetAxis("VerticalRight"));

        //float rotation = Mathf.Atan(Input.GetAxis("HorizontalLeft") / Mathf.Abs(Input.GetAxis("VerticalLeft"))) * Mathf.Rad2Deg;
        //if (Input.GetAxis("VerticalLeft") < 0)
        //{
        //    rotation = Mathf.Sign(Input.GetAxis("HorizontalLeft")) * (180 - Mathf.Abs(rotation));
        //}
        //print(rotation);
        //print(Mathf.Sqrt(Mathf.Pow(Input.GetAxis("HorizontalLeft"), 2) + Mathf.Pow(Input.GetAxis("VerticalLeft"), 2)));
    }

    /// <summary>
    /// Rotate the arm according to the joystick's rotation
    /// </summary>
    /// <param name="left"></param>
    public void RotateArm(bool left)
    {
        if (left)
        {
            float rotation = Mathf.Atan(Input.GetAxis("HorizontalLeft") / Mathf.Abs(Input.GetAxis("VerticalLeft"))) * Mathf.Rad2Deg;
            if (Input.GetAxis("VerticalLeft") < 0)
            {
                rotation = Mathf.Sign(Input.GetAxis("HorizontalLeft")) * (180 - Mathf.Abs(rotation));
            }

            transform.eulerAngles = new Vector3(0, rotation, 0);
        }
        else
        {
            float rotation = Mathf.Atan(Input.GetAxis("HorizontalRight") / Mathf.Abs(Input.GetAxis("VerticalRight"))) * Mathf.Rad2Deg;
            if (Input.GetAxis("VerticalRight") < 0)
            {
                rotation = Mathf.Sign(Input.GetAxis("HorizontalRight")) * (180 - Mathf.Abs(rotation));
            }

            transform.eulerAngles = new Vector3(0, rotation, 0);
        }
    }

    /// <summary>
    /// Stretch the arm according to the joystick's tilt
    /// </summary>
    /// <param name="left"></param>
    public void StretchArm(bool left)
    {
        if (left)
        {
            float length = Mathf.Sqrt(Mathf.Pow(Input.GetAxis("HorizontalLeft"), 2) + Mathf.Pow(Input.GetAxis("VerticalLeft"), 2));

            armTip.localPosition = new Vector3(0, 0, length);
        }
        else
        {
            float length = Mathf.Sqrt(Mathf.Pow(Input.GetAxis("HorizontalRight"), 2) + Mathf.Pow(Input.GetAxis("VerticalRight"), 2));

            armTip.localPosition = new Vector3(0, 0, length);
        }
    }
}
