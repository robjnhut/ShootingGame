using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Weapon;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; set; }

    public AudioSource shootingChannel;

    public AudioClip MP40Shot;
    public AudioClip PistolShot;

    public AudioSource reloadingSoundMP40;
    public AudioSource reloadingSoundPistol;



    public AudioSource emptyMagazineSoundPistol;
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

    public void playShootingSound(WeaponModel weapon)
    {
        switch (weapon)
        {
            case WeaponModel.Pistol:
                shootingChannel.PlayOneShot(PistolShot);
                break;

            case WeaponModel.MP40:
                shootingChannel.PlayOneShot(MP40Shot);
                break;

        }
    }

    public void playReloadSound(WeaponModel weapon)
    {
        switch (weapon)
        {
            case WeaponModel.Pistol:
                reloadingSoundPistol.Play();
                break;

            case WeaponModel.MP40:
                reloadingSoundMP40.Play();
                break;

        }
    }
}
