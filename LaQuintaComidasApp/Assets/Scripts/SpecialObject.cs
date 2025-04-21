using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpecialObject : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _foodText;
    [SerializeField] private Image _foodImage;

    public void SetSpecialObject(string foodName, Sprite foodImage)
    {
        _foodText.text = foodName;
        _foodImage.sprite = foodImage;
    }
}
