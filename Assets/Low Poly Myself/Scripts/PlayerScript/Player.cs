using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Player : MonoBehaviour
{
    public int HP = 100;
    public GameObject bloodyScreen;

    public TextMeshProUGUI playerHealthUI;
    public GameObject gameOverUI;

    public bool isDead;


    private void Start()
    {
        playerHealthUI.text = $"Health: {HP}";

    }

    public void TakeDamage(int damageAmount)
    {
        HP -= damageAmount;

        if (HP <= 0)
        {
            print("Player Dead");
            PlayerDead();
            isDead = true;

        }
        else 
        {
            print("Player Hit");
            StartCoroutine(BloodyScreenEffect());
            playerHealthUI.text = $"Health: {HP}";
            SoundManager.Instance.playerChannel.PlayOneShot(SoundManager.Instance.playerHurt);

        }
    }

    private void PlayerDead()
    {
        SoundManager.Instance.playerChannel.PlayOneShot(SoundManager.Instance.playerDie);


        GetComponent<MouseMovementScript>().enabled = false;
        GetComponent<PlayerMovementScript>().enabled = false;

        //Dying Animation
        GetComponentInChildren<Animator>().enabled = true;
        playerHealthUI.gameObject.SetActive(false);

        GetComponent<ScreenFader>().StartFade();
        StartCoroutine(ShowGameOverUI());

    }

    private IEnumerator ShowGameOverUI()
    {
        yield return new WaitForSeconds(1f);
        gameOverUI.gameObject.SetActive(true);

        int waveSurvived = GlobalReferences.Instance.waveNumber - 1; // Decrease 1 because waveNumber is incremented at the start of the wave
        int currentHighScore = SaveLoadManager.Instance.LoadHighScore();

        if (waveSurvived > currentHighScore)
        {
            SaveLoadManager.Instance.SaveHighScore(waveSurvived);
        }

        // Optionally, you could update the high score UI in the game over screen here

        StartCoroutine(ReturnToMainMenu());
    }



    private IEnumerator ReturnToMainMenu()
    {
        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene("MainMenu");
    }

    private IEnumerator BloodyScreenEffect()
    {
        if(bloodyScreen.activeInHierarchy == false)
        {
            bloodyScreen.SetActive(true);
        }

        var image = bloodyScreen.GetComponent<Image>();

        //Set the initial alpha value to 1 (fully visible)
        Color startColor = image.color;
        startColor.a = 1f;
        image.color = startColor;

        float duration = 3f;
        float elapsedTime = 0f;

        while(elapsedTime < duration)
        {
            // calculate the new alpha value using Lerp
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);

            // update the color with the new alpha value
            Color newColor = image.color;
            newColor.a = alpha;
            image.color = newColor;

            // Increment the alapsed time.
            elapsedTime += Time.deltaTime;

            yield return null; // Wait for the next frame.
        }

        if (bloodyScreen.activeInHierarchy)
        {
            bloodyScreen.SetActive(false);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("ZombieHand"))
        {
            if (isDead == false)
            {
                TakeDamage(other.gameObject.GetComponent<ZombieHand>().damage);

            }        
        }
    }
}