using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NextDayScreen : MonoBehaviour
{
    public Image Image;
    TextMeshProUGUI TextMeshPro;
    public float time = 0;

    private void Start()
    {
        TextMeshPro = GetComponentInChildren<TextMeshProUGUI>();

        Image.color = new Color(0, 0, 0, 1);
        TextMeshPro.color = new Color(1, 1, 1, 1);

        if (GameStatics.NewGame)
            StartCoroutine(ShowScreen(1));
    }

    private void Update()
    {
        transform.SetAsLastSibling();
    }

    public IEnumerator ShowScreen(int day)
    {
        TextMeshPro.text = $"Day {day}";

        float startTransparency = 0;
        time = 0;
        float endTransparency = 1;

        if (Image.color.a != endTransparency)
        {
            while (time < 1)
            {
                time += Time.deltaTime;

                Image.color = new Color(0, 0, 0, Mathf.Lerp(startTransparency, endTransparency, time));
                TextMeshPro.color = new Color(1, 1, 1, Mathf.Lerp(startTransparency, endTransparency, time));

                yield return new WaitForFixedUpdate();
            }
        }

        yield return new WaitForSeconds(3);
        if (GameStatics.Loading)
            yield return new WaitUntil(() => !GameStatics.Loading);
        yield return new WaitForEndOfFrame();

        time = 0;
        while (time < 1)
        {
            time += Time.deltaTime;

            Image.color = new Color(0, 0, 0, Mathf.Lerp(endTransparency, startTransparency, time));
            TextMeshPro.color = new Color(1, 1, 1, Mathf.Lerp(endTransparency, startTransparency, time));

            yield return new WaitForFixedUpdate();
        }

        gameObject.SetActive(false);
    }
}
