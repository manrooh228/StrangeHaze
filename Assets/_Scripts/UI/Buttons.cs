using StrangeHaze.Bootstrap;
using UnityEngine;

public class Buttons : MonoBehaviour
{
    public void StartGameButton()
    {
        ServiceLocator.Get<IGameSceneManager>().GoTo(SceneNames.Level1);
    }

    public void ExitGameButton()
    {
        Application.Quit();
    }
}
