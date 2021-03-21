using UnityEngine;

public class MenuButton : MonoBehaviour
{
    public AudioSource normalClick;
    public AudioSource majorClick;

    public bool isMajorClick = false;
    public void Clicked()
    {
        if (isMajorClick)
            majorClick.Play();
        else
            normalClick.Play();
    }
}
