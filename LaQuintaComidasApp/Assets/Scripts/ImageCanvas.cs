using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ImageCanvas : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _dayText;
    [SerializeField] private List<SpecialObject> _specialObject;

    private void OnEnable()
    {
        AppMenuController.OnCreateImageAction += HandleCreateImage;
    }
    private void OnDisable()
    {
        AppMenuController.OnCreateImageAction -= HandleCreateImage;
    }

    private void HandleCreateImage()
    {
        _dayText.text = AppMenuController.SelectedDayString;

        for (int i = 0; i < _specialObject.Count; i++)
        {
            if (i < AppMenuController.FoodNames.Count)
            {
                string foodName = AppMenuController.FoodNames[i];
                if (FoodDictionaryLoader.Instance.imagesDictionary.TryGetValue(foodName, out Sprite foodImage))
                {
                    _specialObject[i].SetSpecialObject(foodName, foodImage);
                }
                else
                {
                    Debug.LogWarning($"Image not found for food: {foodName}");
                }
            }
            else
            {
                _specialObject[i].gameObject.SetActive(false);
            }
        }

        LoadScreenController.OnLoadingScreenEnable?.Invoke(false);
    }
}
