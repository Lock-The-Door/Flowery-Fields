using UnityEngine;

public class Close : MonoBehaviour
{
    public AudioSource CloseSound;
    public GameObject ObjectToClose;
    public bool DestroyOnClose = false;

    void CloseObject()
    {
        CloseSound.Play();

        if (DestroyOnClose)
            Destroy(ObjectToClose);
        else
            ObjectToClose.SetActive(false);
    }
}
