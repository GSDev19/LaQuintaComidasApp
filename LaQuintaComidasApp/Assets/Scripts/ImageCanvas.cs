using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ImageCanvas : MonoBehaviour
{
    private const string EmpanadasDisplayName = "¡Empanadas Especiales!";

    [SerializeField] private TextMeshProUGUI _dayText;

    //[SerializeField] private SpecialsContainer _specialsContainerSaturday;
    [SerializeField] private SpecialsContainer _specialsContainer5;
    [SerializeField] private SpecialsContainer _specialsContainer6;

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
        _dayText.text = AppMenuController.SelectedDayString.ToString();

        //UIHelpers.SetCanvasGroup(_specialsContainerSaturday.specialCanvasGroup, false);
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

        for (int i = 0; i < activeSpecialObjects.Count; i++)
        {
            if (i < AppMenuController.Instance.FoodNames.Count)
            {
                string foodName = AppMenuController.Instance.FoodNames[i];
                if (FoodDictionaryLoader.Instance.imagesDictionary.TryGetValue(foodName, out Sprite foodImage))
                {
                    activeSpecialObjects[i].SetSpecialObject(foodName, foodImage);
                    activeSpecialObjects[i].gameObject.SetActive(true);
                }
                else
                {
                    Debug.LogWarning($"Image not found for food: {foodName}");
                    activeSpecialObjects[i].gameObject.SetActive(false);
                }
            }
            else
            {
                activeSpecialObjects[i].gameObject.SetActive(false);
            }
        }
        //if(AppMenuController.SelectedDayString == Days.Sábado)
        //{
        //    activeSpecialObjects = _specialsContainerSaturday._specialObject;
        //    UIHelpers.SetCanvasGroup(_specialsContainer5.specialCanvasGroup, true);

        //    for (int i = 0; i < activeSpecialObjects.Count - 1; i++)
        //    {
        //        if (i < AppMenuController.Instance.FoodNames.Count)
        //        {
        //            string foodName = AppMenuController.Instance.FoodNames[i];

        //            //if (foodName.ToLower().Contains("Empanadas"))
        //            //{
        //            //    activeSpecialObjects[i].SetSpecialObject(EmpanadasDisplayName, FoodDictionaryLoader.Instance.imagesDictionary[foodName]);
        //            //    activeSpecialObjects[i].gameObject.SetActive(true);
        //            //}
        //            if (FoodDictionaryLoader.Instance.imagesDictionary.TryGetValue(foodName, out Sprite foodImage))
        //            {
        //                activeSpecialObjects[i].SetSpecialObject(foodName, foodImage);
        //                activeSpecialObjects[i].gameObject.SetActive(true);
        //            }
        //            else
        //            {
        //                Debug.LogWarning($"Image not found for food: {foodName}");
        //                activeSpecialObjects[i].gameObject.SetActive(false);
        //            }
        //        }
        //        else
        //        {
        //            activeSpecialObjects[i].gameObject.SetActive(false);
        //        }
        //    }

        //}        

        LoadScreenController.OnLoadingScreenEnable?.Invoke(false);
    }
}
[System.Serializable]
public struct SpecialsContainer
{
    public CanvasGroup specialCanvasGroup;
    public List<SpecialObject> _specialObject;
}
