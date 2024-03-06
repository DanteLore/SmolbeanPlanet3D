using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour//, IObjectGenerator
{
    public GameObject blackoutImage;
    public AnimationCurve fadeCurve;
    public float fadeDurationSeconds = 2f;
    public int fadeSteps = 100;

    public int Priority { get { return -100; } }
    public bool NewGameOnly { get { return false; } }
    public bool RunModeOnly { get { return true; } }

    public void Clear()
    {
    }

    public IEnumerator Generate(List<int> gameMap, int gameMapWidth, int gameMapHeight)
    {
        blackoutImage.SetActive(true);
        var image = blackoutImage.GetComponent<Image>();
        image.color = Color.black;

        for (int i = 0; i < fadeSteps; i++)
        {
            Color c = new(0, 0, 0, fadeCurve.Evaluate(i / fadeSteps));
            image.color = c;
            yield return new WaitForSeconds(fadeDurationSeconds / fadeSteps);
        }

        blackoutImage.SetActive(false);
        yield return null;
    }
}
