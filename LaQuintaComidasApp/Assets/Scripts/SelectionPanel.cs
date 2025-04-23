using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectionPanel : MonoBehaviour
{
    public Image BGImage { get; private set; }
    public Button Button { get; private set; }
    public TextMeshProUGUI TextTMP { get; private set; }
    public int Index { get; set; } = -1;

    private void Awake()
    {
        BGImage = GetComponent<Image>();
        Button = GetComponentInChildren<Button>();
        TextTMP = GetComponentInChildren<TextMeshProUGUI>();
    }
    public void InitializeSelectionPanel(Color baseColor, string buttonText, Action onClick, int index = -1)
    {
        TextTMP.text = string.Empty;
        SetBGColor(baseColor);
        Button.GetComponentInChildren<TextMeshProUGUI>().text = buttonText;
        Button.onClick.AddListener(() => onClick?.Invoke());
        Index = index;
    }
    public void SetBGColor(Color color)
    {
        if (BGImage != null)
        {
            BGImage.color = color;
        }
    }
}
