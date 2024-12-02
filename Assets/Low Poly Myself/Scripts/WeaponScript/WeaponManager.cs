using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Weapon;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance { get; private set; }

    public List<GameObject> weaponSlots;

    public GameObject activeWeaponSlot;

    [Header("Ammo")]
    public int totalRifleAmmo = 0;
    public int totalPistolAmmo = 0;

    [Header("Throwables General")]
    public float throwForce = 10f;



    public GameObject throwablSpawn;
    public float forceMultiplier = 0;
    public float forceMultiplierLimit = 2f;

    [Header("Lethals")]
    public int maxLethals = 2;
    public int lethalsCount = 0;
    public Throwable.ThrowableType equippedLethalType;
    public GameObject grenadePrefab;

    [Header("Tacticals")]
    public int maxTacticals = 2;
    public int TacticalCount = 0;
    public Throwable.ThrowableType equippedTacticalType;
    public GameObject smokeGrenadePrefab;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        activeWeaponSlot = weaponSlots[0];

        equippedLethalType = Throwable.ThrowableType.None;
        equippedTacticalType = Throwable.ThrowableType.None;

    }

    private void Update()
    {
        foreach (GameObject weaponSlot in weaponSlots)
        {
            if (weaponSlot == activeWeaponSlot)
            {
                weaponSlot.SetActive(true);
            }
            else
            {
                weaponSlot.SetActive(false);
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchActiveSlot(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchActiveSlot(1);
        }

        if (Input.GetKey(KeyCode.G) || Input.GetKeyUp(KeyCode.T))
        {
            forceMultiplier += Time.deltaTime;

            if (forceMultiplier > forceMultiplierLimit)
            {
                forceMultiplier = forceMultiplierLimit;
            }
        }

        if (Input.GetKeyUp(KeyCode.G))
        {
            if (lethalsCount > 0)
            {
                ThrowLethal();
            }

            forceMultiplier = 0;

        }

        if (Input.GetKeyUp(KeyCode.T))
        {
            if (TacticalCount > 0)
            {
                ThrowTactical();
            }

            forceMultiplier = 0;

        }


    }



    public void PickUpWeapon(GameObject pickedupWeapon)
    {

        AddWeaponIntoActiveSlot(pickedupWeapon);
        HUDManager.Instance.UpdateHUD(); // Cập nhật HUD sau khi nhặt vũ khí
    }

    private void AddWeaponIntoActiveSlot(GameObject pickedupWeapon)
    {
        DropCurrentWeapon(pickedupWeapon);

        // Gán súng vào vị trí activeWeaponSlot
        pickedupWeapon.transform.SetParent(activeWeaponSlot.transform, false);

        // Lấy component Weapon để truy cập spawnPosition và spawnRotation
        Weapon weapon = pickedupWeapon.GetComponent<Weapon>();

        // Đặt position và rotation đúng mong muốn
        pickedupWeapon.transform.localPosition = new Vector3(weapon.spawnPosition.x, weapon.spawnPosition.y, weapon.spawnPosition.z);
        pickedupWeapon.transform.localRotation = Quaternion.Euler(weapon.spawnRotation.x, weapon.spawnRotation.y, weapon.spawnRotation.z);

        // Đảm bảo weapon hoạt động
        weapon.isActiveWeapon = true;
        weapon.animator.enabled = true;
    }

    //private void AddWeaponIntoActiveSlot(GameObject pickedupWeapon)
    //{
    //    Debug.Log("Adding weapon to active slot: " + activeWeaponSlot.name);
    //    DropCurrentWeapon(pickedupWeapon);

    //    // Đặt parent của súng là activeWeaponSlot và giữ lại local position/rotation
    //    pickedupWeapon.transform.SetParent(activeWeaponSlot.transform, false);

    //    // Lấy component Weapon để truy cập spawnPosition và spawnRotation
    //    Weapon weapon = pickedupWeapon.GetComponent<Weapon>();

    //    // Đặt lại vị trí và xoay của súng trong local space của WeaponSlot_1
    //    pickedupWeapon.transform.localPosition = weapon.spawnPosition;
    //    pickedupWeapon.transform.localRotation = Quaternion.Euler(weapon.spawnRotation);

    //    // Kích hoạt súng và bật Animator (nếu cần)
    //    weapon.isActiveWeapon = true;
    //    weapon.animator.enabled = true;

    //    Debug.Log("Local Rotation của súng sau khi nhặt: " + pickedupWeapon.transform.localRotation.eulerAngles);
    //}


    private void DropCurrentWeapon(GameObject pickedupWeapon)
    {
        if (activeWeaponSlot.transform.childCount > 0)
        {
            var weaponToDrop = activeWeaponSlot.transform.GetChild(0).gameObject;

            weaponToDrop.GetComponent<Weapon>().isActiveWeapon = false;
            weaponToDrop.GetComponent<Weapon>().animator.enabled = false;

            weaponToDrop.transform.SetParent(pickedupWeapon.transform.parent);
            weaponToDrop.transform.localPosition = pickedupWeapon.transform.localPosition;
            weaponToDrop.transform.localRotation = pickedupWeapon.transform.localRotation;

        }
    }

    public void SwitchActiveSlot(int slotNumber)
    {
        if (activeWeaponSlot.transform.childCount > 0)
        {
            Weapon currentWeapon = activeWeaponSlot.transform.GetChild(0).GetComponent<Weapon>();
            currentWeapon.isActiveWeapon = false;
        }

        activeWeaponSlot = weaponSlots[slotNumber];

        if (activeWeaponSlot.transform.childCount > 0)
        {
            Weapon newWeapon = activeWeaponSlot.transform.GetChild(0).GetComponent<Weapon>();
            newWeapon.isActiveWeapon = true;
        }


    }

    internal void PickUpAmmo(AmmoBox ammo)
    {
        switch (ammo.ammoType)
        {
            case AmmoBox.AmmoType.PistolAmmo:
                totalPistolAmmo += ammo.ammoAmount;
                break;

            case AmmoBox.AmmoType.RifleAmmo:
                totalRifleAmmo += ammo.ammoAmount;
                break;
        }
        HUDManager.Instance.UpdateHUD(); // Cập nhật HUD sau khi nhặt đạn
    }

    internal void DecreaseTotalAmmo(int bulletsToDecrease, Weapon.WeaponModel thisWeaponModel)
    {
        switch (thisWeaponModel)
        {
            case Weapon.WeaponModel.MP40:
                totalRifleAmmo -= bulletsToDecrease;
                break;
            case Weapon.WeaponModel.Pistol:
                totalPistolAmmo -= bulletsToDecrease;
                break;

        }
    }

    public int CheckAmmoLeftFor(WeaponModel thisWeaponModel)
    {
        switch (thisWeaponModel)
        {
            case WeaponModel.MP40:
                return totalRifleAmmo;

            case WeaponModel.Pistol:
                return totalPistolAmmo;

            default:
                return 0;
        }
    }

    public void PickUpThrowable(Throwable throwable)
    {
        switch (throwable.throwableType)
        {
            case Throwable.ThrowableType.Grenade:
                PickupThrowableAsLethal(Throwable.ThrowableType.Grenade);
                break;
            case Throwable.ThrowableType.Smoke_Grenade:
                PickupThrowableAsTactical(Throwable.ThrowableType.Smoke_Grenade);
                break;
        }
    }

    private void PickupThrowableAsTactical(Throwable.ThrowableType tactical)
    {
        if (equippedTacticalType == tactical || equippedTacticalType == Throwable.ThrowableType.None)
        {
            equippedTacticalType = tactical;

            if (TacticalCount < maxTacticals)
            {
                TacticalCount += 1;
                Destroy(InteractionManager.Instance.hoveredThrowable.gameObject);
                HUDManager.Instance.UpdateThrowablesUI();

            }
            else
            {
                print("tactical limit reached");
            }
        }
        else
        {
            // cannot pickup different tactical
            //option to swap tactical
        }
    }

    private void PickupThrowableAsLethal(Throwable.ThrowableType lethal)
    {
        if (equippedLethalType == lethal || equippedLethalType == Throwable.ThrowableType.None)
        {
            equippedLethalType = lethal;

            if (lethalsCount < maxLethals)
            {
                lethalsCount += 1;
                Destroy(InteractionManager.Instance.hoveredThrowable.gameObject);
                HUDManager.Instance.UpdateThrowablesUI();

            }
            else
            {
                print("Lethal limit reached");
            }
        }
        else
        {
            // cannot pickup different lethal
            //option to swap weapon
        }
    }


    private void ThrowLethal()
    {
        GameObject lethalPrefab = GetThrowablePrefabs(equippedLethalType);

        //GameObject throwable = Instantiate(lethalPrefab, throwablSpawn.transform.position, Camera.main.transform.rotation);
        //Rigidbody rb = throwable.GetComponent<Rigidbody>();

        //rb.AddForce(Camera.main.transform.forward * (throwForce * forceMultiplier), ForceMode.Impulse);

        //grenades -= 1;

        //HUDManager.Instance.UpdateThrowables(Throwable.ThrowableType.Grenade);

        // Tạo lựu đạn từ prefab và đặt vị trí ném
        GameObject throwable = Instantiate(grenadePrefab, throwablSpawn.transform.position, Camera.main.transform.rotation);
        Throwable throwableComponent = throwable.GetComponent<Throwable>();

        // Đặt hasBeenThrown = true để bắt đầu đếm ngược cho phát nổ
        if (throwableComponent != null)
        {
            throwableComponent.hasBeenThrown = true;
        }

        // Thêm lực ném
        Rigidbody rb = throwable.GetComponent<Rigidbody>();
        rb.AddForce(Camera.main.transform.forward * (throwForce * forceMultiplier), ForceMode.Impulse);

        // Giảm số lượng lựu đạn và cập nhật HUD
        lethalsCount -= 1;

        if (lethalsCount <= 0)
        {
            equippedLethalType = Throwable.ThrowableType.None;
        }



        HUDManager.Instance.UpdateThrowablesUI();

    }

    //private void ThrowTactical()
    //{
    //    GameObject tacticalPrefab = GetThrowablePrefabs(equippedTacticalType);

    //    GameObject throwable = Instantiate(tacticalPrefab, throwablSpawn.transform.position, Camera.main.transform.rotation);
    //    Throwable throwableComponent = throwable.GetComponent<Throwable>();

    //    // Đặt hasBeenThrown = true để bắt đầu đếm ngược cho phát nổ
    //    if (throwableComponent != null)
    //    {
    //        throwableComponent.hasBeenThrown = true;
    //    }

    //    // Thêm lực ném
    //    Rigidbody rb = throwable.GetComponent<Rigidbody>();
    //    rb.AddForce(Camera.main.transform.forward * (throwForce * forceMultiplier), ForceMode.Impulse);

    //    // Giảm số lượng lựu đạn và cập nhật HUD
    //    TacticalCount -= 1;

    //    if (TacticalCount <= 0)
    //    {
    //        equippedTacticalType = Throwable.ThrowableType.None;
    //    }

    //    HUDManager.Instance.UpdateThrowablesUI();
    //}

    private void ThrowTactical()
    {
        GameObject tacticalPrefab = GetThrowablePrefabs(equippedTacticalType);

        GameObject throwable = Instantiate(tacticalPrefab, throwablSpawn.transform.position, Camera.main.transform.rotation);
        Throwable throwableComponent = throwable.GetComponent<Throwable>();

        if (throwableComponent != null)
        {
            throwableComponent.hasBeenThrown = true;
        }

        Rigidbody rb = throwable.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Debug.Log("Rigidbody is correctly attached to the smoke grenade.");
            rb.AddForce(Camera.main.transform.forward * (throwForce * forceMultiplier), ForceMode.Impulse);
        }
        else
        {
            Debug.LogError("Rigidbody is missing on smoke grenade!");
        }

        TacticalCount -= 1;

        if (TacticalCount <= 0)
        {
            equippedTacticalType = Throwable.ThrowableType.None;
        }

        HUDManager.Instance.UpdateThrowablesUI();
    }


    private GameObject GetThrowablePrefabs(Throwable.ThrowableType throwableType)
    {
        switch (throwableType)
        {
            case Throwable.ThrowableType.Grenade:
                return grenadePrefab;
            case Throwable.ThrowableType.Smoke_Grenade:
                return smokeGrenadePrefab;
        }

        return new();
    }
}

