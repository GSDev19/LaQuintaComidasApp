using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class ImageCanvas : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _dayText;

    [SerializeField] private SpecialsContainer _specialsContainer5;
    [SerializeField] private SpecialsContainer _specialsContainer6;

    private void OnEnable()
    {
        AppMenuController.OnCreateImageAction += HandleCreateImage;
        AppMenuController.OnRemoveMenuAction += HandleRemoveMenu;
    }
    private void OnDisable()
    {
        AppMenuController.OnCreateImageAction -= HandleCreateImage;
        AppMenuController.OnRemoveMenuAction -= HandleRemoveMenu;
    }
    private void HandleCreateImage()
    {
        StartCoroutine(HandleCreateImageCoroutine());
    }  

    private IEnumerator HandleCreateImageCoroutine()
    {
        LoadScreenController.OnLoadingScreenEnable?.Invoke(true);
        LoadScreenController.OnLoadingTextChanged?.Invoke("Creando menu...");

        _dayText.text = AppMenuController.SelectedDayString.ToString();

        UIHelpers.SetCanvasGroup(_specialsContainer5.specialCanvasGroup, false);
        UIHelpers.SetCanvasGroup(_specialsContainer6.specialCanvasGroup, false);

        List<SpecialObject> activeSpecialObjects = null;

        if (AppMenuController.SelectedAmount == Amount.Cinco)
        {
            activeSpecialObjects = _specialsContainer5._specialObject;
            UIHelpers.SetCanvasGroup(_specialsContainer5.specialCanvasGroup, true);
        }
        else if (AppMenuController.SelectedAmount == Amount.Seis)
        {
            activeSpecialObjects = _specialsContainer6._specialObject;
            UIHelpers.SetCanvasGroup(_specialsContainer6.specialCanvasGroup, true);
        }

        int completedCount = 0;
        int expectedCount = Mathf.Min(AppMenuController.Instance.FoodNames.Count, activeSpecialObjects.Count);

        for (int i = 0; i < activeSpecialObjects.Count; i++)
        {
            if (i < AppMenuController.Instance.FoodNames.Count)
            {
                string foodName = AppMenuController.Instance.FoodNames[i];
                activeSpecialObjects[i].foodText.text = foodName;
                activeSpecialObjects[i].gameObject.SetActive(true);

                FoodDictionaryLoader.RequestSprite(foodName, activeSpecialObjects[i].foodImage, () =>
                {
                    completedCount++;
                });
            }
            else
            {
                activeSpecialObjects[i].gameObject.SetActive(false);
            }
        }

        // Wait until all images are done
        while (completedCount < expectedCount)
        {
            yield return null;
        }

        LoadScreenController.OnLoadingScreenEnable?.Invoke(false);
    }

    private void HandleRemoveMenu()
    {
        foreach (var specialObject in _specialsContainer5._specialObject)
        {
            specialObject.foodImage.sprite = null;
        }
        foreach (var specialObject in _specialsContainer6._specialObject)
        {
            specialObject.foodImage.sprite = null;
        }

        FoodDictionaryLoader.ClearLoadedSprites();
    }
}
[System.Serializable]
public struct SpecialsContainer
{
    public CanvasGroup specialCanvasGroup;
    public List<SpecialObject> _specialObject;
}
