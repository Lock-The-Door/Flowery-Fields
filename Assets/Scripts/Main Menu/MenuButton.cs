using UnityEngine;

public class MenuButton : MonoBehaviour
{
    public AudioSource click;

    public void Clicked() => click.Play();
}
