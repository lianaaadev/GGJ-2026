using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CreditManager : MonoBehaviour
{
    private const string BGM = "Audios/introoutroambientwitchfadeout";

     void Start()
    {
        AudioManager.Instance.PlayBGM(BGM);
    }
}