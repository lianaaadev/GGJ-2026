using UnityEngine;

public class FinishFlag : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Level Complete!");
            AudioManager.Instance.PlaySE(AudioManager.SE_SUCCESS);
            GameManager.Instance.OnLevelComplete();
        }
    }
}
