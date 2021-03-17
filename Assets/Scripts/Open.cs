using UnityEngine;

public class Open : MonoBehaviour
{
    public AudioSource OpenSound;
    public GameObject ObjectToOpen;
    public bool InstantiateOnOpen = false;

    void OpenObject()
    {
        OpenSound.Play();

        if (InstantiateOnOpen)
            Instantiate(ObjectToOpen);
        else
            ObjectToOpen.SetActive(true);
    }
}
