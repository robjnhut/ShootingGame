using System.Collections;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager Instance { get; set; }

    public Weapon hoveredWeapon = null;
    public AmmoBox hoveredAmmoBox = null;
    public Throwable hoveredThrowable = null;  // New addition for Throwable objects

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Update()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject objectHitByRaycast = hit.transform.gameObject;

            // Handle Weapon
            if (objectHitByRaycast.GetComponent<Weapon>() && !objectHitByRaycast.GetComponent<Weapon>().isActiveWeapon)
            {
                if (hoveredWeapon != objectHitByRaycast.GetComponent<Weapon>())
                {
                    ResetHoveredObjects();
                    hoveredWeapon = objectHitByRaycast.GetComponent<Weapon>();
                    hoveredWeapon.GetComponent<Outline>().enabled = true;
                }

                if (Input.GetKeyDown(KeyCode.F))
                {
                    WeaponManager.Instance.PickUpWeapon(objectHitByRaycast);
                }
            }
            // Handle AmmoBox
            else if (objectHitByRaycast.GetComponent<AmmoBox>())
            {
                if (hoveredAmmoBox != objectHitByRaycast.GetComponent<AmmoBox>())
                {
                    ResetHoveredObjects();
                    hoveredAmmoBox = objectHitByRaycast.GetComponent<AmmoBox>();
                    hoveredAmmoBox.GetComponent<Outline>().enabled = true;
                }

                if (Input.GetKeyDown(KeyCode.F))
                {
                    WeaponManager.Instance.PickUpAmmo(hoveredAmmoBox);
                    Destroy(objectHitByRaycast.gameObject);
                }
            }
            // Handle Throwable
            else if (objectHitByRaycast.GetComponent<Throwable>())
            {
                if (hoveredThrowable != objectHitByRaycast.GetComponent<Throwable>())
                {
                    ResetHoveredObjects();
                    hoveredThrowable = objectHitByRaycast.GetComponent<Throwable>();
                    hoveredThrowable.GetComponent<Outline>().enabled = true;
                }

                if (Input.GetKeyDown(KeyCode.F))
                {
                    WeaponManager.Instance.PickUpThrowable(hoveredThrowable);
                }
            }
            else
            {
                // If not hitting any interactive object, reset hovered objects
                ResetHoveredObjects();
            }
        }
        else
        {
            // If no object is hit, reset hovered objects
            ResetHoveredObjects();
        }
    }

    // Method to reset the hovered objects and disable their outlines
    private void ResetHoveredObjects()
    {
        if (hoveredWeapon)
        {
            hoveredWeapon.GetComponent<Outline>().enabled = false;
            hoveredWeapon = null;
        }

        if (hoveredAmmoBox)
        {
            hoveredAmmoBox.GetComponent<Outline>().enabled = false;
            hoveredAmmoBox = null;
        }

        if (hoveredThrowable)
        {
            hoveredThrowable.GetComponent<Outline>().enabled = false;
            hoveredThrowable = null;
        }
    }
}
