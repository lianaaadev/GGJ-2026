using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FullscreenImageAnimation : MonoBehaviour
{
    public Image image;
    public Sprite[] frames;
    public float frameDuration = 0.1f;
    public bool isIntro;
    public GameObject startButton; // drag your button here
    public GameObject skipButton; // drag your button here
    public AudioSource audio;
    bool skipping = false;

    void Start()
    {
        if (startButton != null)
            startButton.SetActive(false);
        if (audio != null)
            audio.Play();

        StartCoroutine(Play());
    }

    IEnumerator Play()
    {
        foreach (Sprite frame in frames)
        {
            image.sprite = frame;
            yield return new WaitUntil(() => skipping || Time.time >= frameDuration);
            skipping = false;
        }
        if (startButton != null)
        {
            startButton.SetActive(true);
            skipButton.SetActive(false);
        }
    }

    public void OnSkipBtn()
    {
        skipping = true;
    }
}