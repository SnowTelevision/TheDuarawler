using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Functions for a simple pistol (shoot, probably reload)
/// </summary>
public class SimplePistol : MonoBehaviour
{
    public GameObject pistolBullet; // The bullet that should be shot out from the pistol
    public float pistolInitialVelocity; // The initial velocity of the pistol
    public float pistolBulletLifeSpawn; // How long the pistol can exist before disappear

    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Shoot the pistol
    /// </summary>
    public void Shoot()
    {
        GameObject newPistolBullet = Instantiate(pistolBullet, transform.position, transform.rotation);
        //newPistolBullet.transform.LookAt(transform.position + transform.forward);
        newPistolBullet.GetComponent<Rigidbody>().AddForce(pistolInitialVelocity * newPistolBullet.transform.forward, ForceMode.VelocityChange);
        newPistolBullet.GetComponent<SimplePistolBullet>().owner = this;
        Destroy(newPistolBullet, pistolBulletLifeSpawn);
    }
}
