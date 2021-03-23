using UnityEngine;

public class CloseOnClick : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
            gameObject.SetActive(false);
    }
}
