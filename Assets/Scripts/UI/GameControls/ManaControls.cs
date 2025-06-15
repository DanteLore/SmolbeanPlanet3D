using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

public class ManaControls : MonoBehaviour
{
    private VisualElement root;
    private Label manaLabel;
    private VisualElement container;
    private float previousMana;
    private readonly StringBuilder stringBuilder = new(128);

    private void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        manaLabel = root.Q<Label>("manaLabel");
        container = root.Q<VisualElement>("gameInfoControlsContainer");
        ManaController.Instance.OnManaChanged += ManaChanged;

        manaLabel.text = GetManaString(ManaController.Instance.Mana);
    }

    private void OnDisable()
    {
        ManaController.Instance.OnManaChanged -= ManaChanged;
    }

    private void ManaChanged(float mana)
    {
        manaLabel.text = GetManaString(mana);

        if (mana > previousMana)
            StartCoroutine(GlowEffectCoroutine());

        previousMana = mana;
    }

    private string GetManaString(float mana)
    {
        stringBuilder.Clear();
        if (mana < 1000)
        {
            stringBuilder.AppendFormat("{0:F0}", mana);
        }
        else if (mana < 1000 * 1000)
        {
            stringBuilder.AppendFormat("{0:F1}", mana / 1000);
            stringBuilder.Append("k");
        }
        else
        {
            stringBuilder.AppendFormat("{0:F1}", mana / 1000 * 1000);
            stringBuilder.Append("M");
        }

        return stringBuilder.ToString();
    }

    private IEnumerator GlowEffectCoroutine()
    {
        container.AddToClassList("glow");

        // Wait for transition duration to finish
        yield return new WaitForSeconds(2f);

        container.RemoveFromClassList("glow");
    }
}
