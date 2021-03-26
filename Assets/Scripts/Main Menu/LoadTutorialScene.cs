using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadTutorialScene : MonoBehaviour
{
    public void PlayTutorial()
    {
        GameStatics.NewGame = true;
        SceneManager.LoadScene("Tutorial");
    }
}
