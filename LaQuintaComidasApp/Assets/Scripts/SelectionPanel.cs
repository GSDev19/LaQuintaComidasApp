using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectionPanel : MonoBehaviour
{
    public Image BGImage { get; private set; }
    public Button Button { get; private set; }
    public TextMeshProUGUI TextTMP { get; private set; }

    private void Awake()
    {
        BGImage = GetComponent<Image>();
        Button = GetComponentInChildren<Button>();
        TextTMP = GetComponentInChildren<TextMeshProUGUI>();
    }
    public void SetBGColor(Color color)
    {
        if (BGImage != null)
        {
            BGImage.color = color;
        }
    }
}
