using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance { get; set; }

    public List<GameObject> weaponSlots;
    
    public GameObject activeWeaponSlot;

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
    }

    private void Update()
    {
        foreach (GameObject weaponSlot in weaponSlots)
        {
            if(weaponSlot == activeWeaponSlot)
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

    }
    public void PickUpWeapon(GameObject pickedupWeapon)
    {
        AddWeaponIntoActiveSlot(pickedupWeapon);
    }

    //private void AddWeaponIntoActiveSlot(GameObject pickedupWeapon)
    //{
    //    DropCurrentWeapon(pickedupWeapon);

    //    // Gán súng vào vị trí activeWeaponSlot
    //    pickedupWeapon.transform.SetParent(activeWeaponSlot.transform, false);

    //    // Lấy component Weapon để truy cập spawnPosition và spawnRotation
    //    Weapon weapon = pickedupWeapon.GetComponent<Weapon>();

    //    // Đặt position và rotation đúng mong muốn
    //    pickedupWeapon.transform.localPosition =new Vector3(weapon.spawnPosition.x, weapon.spawnPosition.y, weapon.spawnPosition.z);
    //    pickedupWeapon.transform.localRotation = Quaternion.Euler(weapon.spawnRotation.x, weapon.spawnRotation.y, weapon.spawnRotation.z);

    //    // Đảm bảo weapon hoạt động
    //    weapon.isActiveWeapon = true;
    //    weapon.animator.enabled = true;
    //}

    private void AddWeaponIntoActiveSlot(GameObject pickedupWeapon)
    {
        DropCurrentWeapon(pickedupWeapon);

        // Đặt parent của súng là activeWeaponSlot và giữ lại local position/rotation
        pickedupWeapon.transform.SetParent(activeWeaponSlot.transform, false);

        // Lấy component Weapon để truy cập spawnPosition và spawnRotation
        Weapon weapon = pickedupWeapon.GetComponent<Weapon>();

        // Đặt lại vị trí và xoay của súng trong local space của WeaponSlot_1
        pickedupWeapon.transform.localPosition = weapon.spawnPosition;
        pickedupWeapon.transform.localRotation = Quaternion.Euler(weapon.spawnRotation);

        // Kích hoạt súng và bật Animator (nếu cần)
        weapon.isActiveWeapon = true;
        weapon.animator.enabled = true;

        Debug.Log("Local Rotation của súng sau khi nhặt: " + pickedupWeapon.transform.localRotation.eulerAngles);
    }


    private void DropCurrentWeapon(GameObject pickedupWeapon)
    {
        if(activeWeaponSlot.transform.childCount > 0)
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
}
