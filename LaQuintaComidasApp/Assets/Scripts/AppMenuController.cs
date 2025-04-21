using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AppMenuController : MonoBehaviour
{
    public static Action OnSetImagesAction;
    public static Action OnSaveMenuAction;

    public Button _createButton;
    public Button _saveButton;

    public Button _optionButtonPrefab;
    public CanvasGroup _optionMenuCanvasGroup;

    [Header("Days")]
    public Days _selectedDay;
    public Button _daySelectionButton;
    public TextMeshProUGUI _daySelectionText;
    public CanvasGroup _daySelectionCanvasGroup;
    public Transform _dayOptionsParent;

    [Header("Food")]
    public int _currentFoodIndex;
    public Button[] _foodButtons;
    public TextMeshProUGUI[] _foodTexts;
    public CanvasGroup _foodSelectionCanvasGroup;
    public Transform _foodOptionsParent;


    private void Awake()
    {
        _createButton.interactable = false;
        _saveButton.interactable = false;

        _createButton.onClick.AddListener(() => OnCreateButtonClicked());
        _saveButton.onClick.AddListener(() => OnSaveMenuButtonClicked());

        UIHelpers.SetCanvasGroup(_optionMenuCanvasGroup, true);
        UIHelpers.SetCanvasGroup(_daySelectionCanvasGroup, false);
        UIHelpers.SetCanvasGroup(_foodSelectionCanvasGroup, false);


        _daySelectionButton.onClick.AddListener(() => UIHelpers.SetCanvasGroup(_daySelectionCanvasGroup, true));
        SetupDayButtons();

        SetFoodButtons();
        SetFoodsTexts();
    }
    private void OnEnable()
    {
        FoodDictionaryLoader.OnImagesDictionaryLoaded += HandleImagesLoaded;
    }
    private void OnDisable()
    {
        FoodDictionaryLoader.OnImagesDictionaryLoaded -= HandleImagesLoaded;
    }

    private void HandleImagesLoaded()
    {
        _createButton.interactable = true;

        CreateFoodOptionButtons();
    }
    private void OnCreateButtonClicked()
    {
        OnSetImagesAction?.Invoke();
        _createButton.interactable = false;
        _saveButton.interactable = true;
    }
    private void OnSaveMenuButtonClicked()
    {
        OnSaveMenuAction?.Invoke();
    }

    #region
    private void SetupDayButtons()
    {
        // Corrected the foreach loop to iterate over the values of the Days enum
        foreach (Days day in Enum.GetValues(typeof(Days)))
        {
            Button button = Instantiate(_optionButtonPrefab, _dayOptionsParent);
            button.GetComponentInChildren<TextMeshProUGUI>().text = day.ToString();
            button.onClick.AddListener(() =>
            {
                OnDayOptionClicked(day);
            });
        }
    }
    private void OnDayOptionClicked(Days day)
    {
        _selectedDay = day;
        _daySelectionText.text = _selectedDay.ToString();
        UIHelpers.SetCanvasGroup(_optionMenuCanvasGroup, true);
        UIHelpers.SetCanvasGroup(_daySelectionCanvasGroup, false);
    }
    #endregion

    #region FOODS
    private void SetFoodButtons()
    {
        for (int i = 0; i < _foodButtons.Length; i++)
        {
            int index = i; // Prevent closure issue
            _foodButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = ((Foods)i).ToString();
            _foodButtons[i].onClick.AddListener(() =>
            {
                OnFoodOptionClicked(index);
            });
        }
    }
    private void SetFoodsTexts()
    {
        foreach (var item in _foodTexts)
        {
            item.text = string.Empty;
        }
    }
    private void OnFoodOptionClicked(int index)
    {
        _currentFoodIndex = index;
        UIHelpers.SetCanvasGroup(_optionMenuCanvasGroup, false);
        UIHelpers.SetCanvasGroup(_foodSelectionCanvasGroup, true);
    }

    private void CreateFoodOptionButtons()
    {
        foreach (var item in FoodDictionaryLoader.FoodDictionary)
        {
            Button button = Instantiate(_optionButtonPrefab, _foodOptionsParent);
            button.GetComponentInChildren<TextMeshProUGUI>().text = item.Key;
            button.onClick.AddListener(() =>
            {
                OnFoodOptionClicked(item.Key);
            });
        }
    }

    private void OnFoodOptionClicked(string foodName)
    {
        _foodTexts[_currentFoodIndex].text = foodName;
        UIHelpers.SetCanvasGroup(_foodSelectionCanvasGroup, false);
        UIHelpers.SetCanvasGroup(_optionMenuCanvasGroup, true);
    }

    #endregion
}
public enum Days
{
    Lunes,
    Martes,
    Miércoles,
    Jueves,
    Viernes,
    Sábado,
    Domingo
}
public enum Foods
{
    Especial1,
    Especial2, 
    Especial3, 
    Especial4, 
    Especial5,
    Especial6
}
