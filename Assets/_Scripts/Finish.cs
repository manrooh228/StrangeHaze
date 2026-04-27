using UnityEngine;

public class Finish : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        { 
            EndGame();
        }
    }

    private void EndGame()
    {
        Application.Quit();

        // ¬ редакторе Unity используем это, чтобы остановить игру:
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
