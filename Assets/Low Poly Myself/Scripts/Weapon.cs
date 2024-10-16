using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
   

    //shooting
    public bool isShooting, readyToshoot;
    bool allowReset = true;
    public float shootingDelay = 2f;

    //Burst
    public int bulletsPerBurst = 3;
    public int burstBulletLeft;

    // spread
    public float spreadIntensity;

    //bullet
    public GameObject bulletPrefabs;
    public Transform bulletSpawn;
    public float bulletVelocity = 30;
    public float bulletPrefabsLifeTime = 3f;
    // Update is called once per frame

    public GameObject muzzleEffect;

    private Animator animator;
    public enum ShootingMode
    {
        Single,
        Burst,
        Auto
    }

    public ShootingMode currentShootingMode;

    private void Awake()
    {
        readyToshoot = true;
        burstBulletLeft = bulletsPerBurst;
        animator = GetComponent<Animator>(); 

    }

    void Update()
    {
       if(currentShootingMode == ShootingMode.Auto)
        {
            //Holding Down left Mouse button
            isShooting = Input.GetKey(KeyCode.Mouse0);
        }
       else if(currentShootingMode == ShootingMode.Single || currentShootingMode == ShootingMode.Burst)
        {
            // clicik chuột trái 1 lần
            isShooting = Input.GetKeyDown(KeyCode.Mouse0);
        }
       if(readyToshoot && isShooting)
        {
            burstBulletLeft = bulletsPerBurst;
            FireWeapon();
        }
        
    }

    private void FireWeapon()
    {
        muzzleEffect.GetComponent<ParticleSystem>().Play();
        animator.SetTrigger("RECOIL");

        readyToshoot = false;
        Vector3 shootingDirection = CalculateDirectionAndSpread().normalized;


        // instantiate the bullet
        GameObject bullet =  Instantiate(bulletPrefabs, bulletSpawn.position, Quaternion.identity);
        
        // point the bullet to face shooting direction
        bullet.transform.forward = shootingDirection;

        //shoot the bullet
        bullet.GetComponent<Rigidbody>().AddForce(bulletSpawn.forward.normalized *  bulletVelocity, ForceMode.Impulse);
        
        //destroy the bullet after a few seconds
        StartCoroutine(DestroyBulletAfterTime(bullet,bulletPrefabsLifeTime));


        //checking if we are done shooting
        if(allowReset)
        {
            Invoke("ResetShot", shootingDelay);
            allowReset = false;
        }

        //burst mode
        if (currentShootingMode == ShootingMode.Burst && burstBulletLeft > 1)
        {
            burstBulletLeft--;
            Invoke("FireWeapon", shootingDelay);
        }
    }
    
    private void ResetShot()
    {
        readyToshoot = true;
        allowReset = true;
    }

    public Vector3 CalculateDirectionAndSpread()
    { 
        //shooting from the middle of the screen to check where we point at
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        Vector3 targetPoint;
        if(Physics.Raycast(ray, out hit))
        {
            // hit something
            targetPoint = hit.point;
        }
        else 
        {
            //shooting at the air
            targetPoint = ray.GetPoint(100);
        }
        

        Vector3 direction = targetPoint - bulletSpawn.position;

        float x = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);
        float y = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);

        //returning the shooting direction and spread
        return direction + new Vector3(x, y, 0);


    }

    private IEnumerator DestroyBulletAfterTime(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(bullet);

    }
}
