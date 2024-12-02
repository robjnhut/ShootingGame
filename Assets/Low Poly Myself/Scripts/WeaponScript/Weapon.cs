using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Weapon : MonoBehaviour
{

    public bool isActiveWeapon;
    public int weaponDamage;


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
    internal Animator animator;

    //loading
    public float reloadTime;
    public int magazineSize, bulletsLeft;
    public bool isReloading;

    public Vector3 spawnPosition;
    public Vector3 spawnRotation;


    //UI
    public TextMeshProUGUI ammoDisplay;

    public enum WeaponModel
    {
        Pistol,
        MP40

    }

    public WeaponModel thisWeaponModel;
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

        bulletsLeft = magazineSize;

    }

    void Update()
    {

        if (isActiveWeapon)
        {

            foreach(Transform child in transform)
            {
                child.gameObject.layer = LayerMask.NameToLayer("WeaponRender");
            }


            GetComponent<Outline>().enabled = false;

            // Phát âm thanh hết đạn nếu đạn đã hết và người chơi nhấn chuột để bắn
            if (bulletsLeft == 0 && isShooting && !SoundManager.Instance.emptyMagazineSoundPistol.isPlaying)
            {
                SoundManager.Instance.emptyMagazineSoundPistol.Play();
            }

            //if (bulletsLeft == 0 && isShooting)
            //{
            //    SoundManager.Instance.emptyMagazineSoundPistol.Play();
            //}

            if (currentShootingMode == ShootingMode.Auto)
            {
                //Holding Down left Mouse button
                isShooting = Input.GetKey(KeyCode.Mouse0);
            }
            else if (currentShootingMode == ShootingMode.Single || currentShootingMode == ShootingMode.Burst)
            {
                // clicik chuột trái 1 lần
                isShooting = Input.GetKeyDown(KeyCode.Mouse0);
            }

            if (Input.GetKeyUp(KeyCode.R) && bulletsLeft < magazineSize && isReloading == false && WeaponManager.Instance.CheckAmmoLeftFor(thisWeaponModel) > 0)
            {
                Reload();
            }

            // if you want to automatically reload when magazine is empty
            if (readyToshoot && isShooting == false && isReloading == false && bulletsLeft <= 0)
            {
                // Reload();
            }

            if (readyToshoot && isShooting)
            {
                burstBulletLeft = bulletsPerBurst;
                FireWeapon();
            }

        }
        else
        {
            foreach (Transform child in transform)
            {
                child.gameObject.layer = LayerMask.NameToLayer("Default");
            }
        }
    }



    private void FireWeapon()
    {
        if (bulletsLeft <= 0)
        {
            SoundManager.Instance.emptyMagazineSoundPistol.Play();
            return; // Ngừng hàm để không bắn khi không còn đạn
        }

        bulletsLeft--;

        muzzleEffect.GetComponent<ParticleSystem>().Play();
        animator.SetTrigger("RECOIL");
        HUDManager.Instance.UpdateHUD();



        //SoundManager.Instance.shootingPistol.Play();
        SoundManager.Instance.playShootingSound(thisWeaponModel);

        readyToshoot = false;
        Vector3 shootingDirection = CalculateDirectionAndSpread().normalized;


        // instantiate the bullet
        GameObject bullet = Instantiate(bulletPrefabs, bulletSpawn.position, Quaternion.identity);


        Bullet bul = bullet.GetComponent<Bullet>();
        bul.bulletDamage = weaponDamage;

        // point the bullet to face shooting direction
        bullet.transform.forward = shootingDirection;

        //shoot the bullet
        bullet.GetComponent<Rigidbody>().AddForce(bulletSpawn.forward.normalized * bulletVelocity, ForceMode.Impulse);

        //destroy the bullet after a few seconds
        StartCoroutine(DestroyBulletAfterTime(bullet, bulletPrefabsLifeTime));


        //checking if we are done shooting
        if (allowReset)
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

    private void Reload()
    {
        //SoundManager.Instance.reloadingSoundPistol.Play();
        SoundManager.Instance.playReloadSound(thisWeaponModel);


        animator.SetTrigger("RELOAD");


        isReloading = true;
        Invoke("ReloadCompleted", reloadTime);

    }

    private void ReloadCompleted()
    {
        if (WeaponManager.Instance.CheckAmmoLeftFor(thisWeaponModel) > magazineSize)
        {
            bulletsLeft = magazineSize;
            WeaponManager.Instance.DecreaseTotalAmmo(bulletsLeft, thisWeaponModel);
        }
        else
        {
            bulletsLeft = WeaponManager.Instance.CheckAmmoLeftFor(thisWeaponModel);
            WeaponManager.Instance.DecreaseTotalAmmo(bulletsLeft, thisWeaponModel);
        }

        isReloading = false;
        HUDManager.Instance.UpdateHUD();

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
        if (Physics.Raycast(ray, out hit))
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


