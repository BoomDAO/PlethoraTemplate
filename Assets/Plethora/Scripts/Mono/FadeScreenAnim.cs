using System.Collections;
using UnityEngine;

public class FadeScreenAnim : MonoBehaviour
{
    [SerializeField] bool animOnStart;
    [SerializeField] float fadeDuration = 2;
    [SerializeField] bool fadeIn;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField, ShowOnly] bool playing;

    // Start is called before the first frame update
    void Start()
    {
        if (animOnStart) Fade();
    }

    public void Fade()
    {
        if (playing) return;

        StartCoroutine(FadeScreen());
    }

    IEnumerator FadeScreen()
    {
        playing = true;

        if (fadeIn) canvasGroup.alpha = 0;
        else canvasGroup.alpha = 1;

        var duration = fadeDuration;

        while (duration > 0) { 
            duration -= Time.deltaTime;
             
            float perc = duration / fadeDuration;

            if (fadeIn) canvasGroup.alpha = 1 - perc;
            else canvasGroup.alpha = perc;

            yield return null;
        }

        if (fadeIn) canvasGroup.alpha = 1;
        else canvasGroup.alpha = 0;

        playing = false;
    }
}
