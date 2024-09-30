using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameObject bulletPrefabs;
    public Transform bulletSpawn;
    public float bulletVelocity = 30;
    public float bulletPrefabsLifeTime = 3f;
    // Update is called once per frame
    void Update()
    {
        //left mouse click
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            FireWeapon();
        }
        
    }

    private void FireWeapon()
    {
        // Kiểm tra vị trí bulletSpawn
        Debug.Log("Bullet Spawn Position: " + bulletSpawn.position);
        Debug.Log(bulletSpawn.forward);


        // instantiate the bullet
        GameObject bullet =  Instantiate(bulletPrefabs, bulletSpawn.position, Quaternion.identity);
        //shoot the bullet
        
        bullet.GetComponent<Rigidbody>().AddForce(bulletSpawn.forward.normalized *  bulletVelocity, ForceMode.Impulse);
        //destroy the bullet after a few seconds
        StartCoroutine(DestroyBulletAfterTime(bullet,bulletPrefabsLifeTime));


    }

    private IEnumerator DestroyBulletAfterTime(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(bullet);

    }
}
