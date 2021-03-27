using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadTutorialScene : MonoBehaviour
{
    public GameObject ScreenBlocker;

    public void PlayTutorial()
    {
        ScreenBlocker.SetActive(true);

        GameStatics.NewGame = true;
        SceneManager.LoadScene("Tutorial");
    }
}
