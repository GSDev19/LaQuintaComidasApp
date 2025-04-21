using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AppMenuController : Singleton<AppMenuController>
{
    public static Action OnCreateImageAction;
    public static Action OnDownloadMenuAction;

    public Button _createButton;
    public Button _downloadButton;
    public Button _backButton;

    public Button _optionButtonPrefab;
    public CanvasGroup _optionMenuCanvasGroup;

    [Header("Days")]
    public Days _selectedDay;
    public Button _daySelectionButton;
    public TextMeshProUGUI _daySelectionText;
    public CanvasGroup _daySelectionCanvasGroup;
    public Transform _dayOptionsParent;

    public static string SelectedDayString => Instance._selectedDay.ToString();

    [Header("Food")]
    public int _currentFoodIndex;
    public Button[] _foodButtons;
    public TextMeshProUGUI[] _foodTexts;
    public CanvasGroup _foodSelectionCanvasGroup;
    public Transform _foodOptionsParent;

    public static List<string> FoodNames = new List<string>(6);

    protected override void Awake()
    {
        base.Awake();

        _createButton.onClick.AddListener(() => OnCreateButtonClicked());
        _backButton.onClick.AddListener(() => OnBackButtonClicked());
        _downloadButton.onClick.AddListener(() => OnDownloadButtonClicked());

        _createButton.gameObject.SetActive(false);
        _downloadButton.gameObject.SetActive(false);
        _backButton.gameObject.SetActive(false);

        UIHelpers.SetCanvasGroup(_optionMenuCanvasGroup, true);
        UIHelpers.SetCanvasGroup(_daySelectionCanvasGroup, false);
        UIHelpers.SetCanvasGroup(_foodSelectionCanvasGroup, false);

        _selectedDay = Days.Lunes;
        _daySelectionButton.onClick.AddListener(() => UIHelpers.SetCanvasGroup(_daySelectionCanvasGroup, true));
        SetupDayButtons();

        SetFoodButtons();
        SetFoodsTexts();

        FoodNames = new List<string>(6);
        for (int i = 0; i < 6; i++)
        {
            FoodNames.Add(string.Empty);
        }
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

    private void OnBackButtonClicked()
    {
        UIHelpers.SetCanvasGroup(_optionMenuCanvasGroup, true);
        UIHelpers.SetCanvasGroup(_daySelectionCanvasGroup, false);
        UIHelpers.SetCanvasGroup(_foodSelectionCanvasGroup, false);

        _createButton.gameObject.SetActive(true);
        _downloadButton.gameObject.SetActive(false);
        _backButton.gameObject.SetActive(false);
    }
    private void OnCreateButtonClicked()
    {
        UIHelpers.SetCanvasGroup(_optionMenuCanvasGroup, false);

        LoadScreenController.OnLoadingScreenEnable?.Invoke(true);
        LoadScreenController.OnLoadingTextChanged?.Invoke("Creando imagen ...");

        OnCreateImageAction?.Invoke();
        _createButton.gameObject.SetActive(false);
        _downloadButton.gameObject.SetActive(true);
        _backButton.gameObject.SetActive(true);
    }
    private void OnDownloadButtonClicked()
    {
        LoadScreenController.OnLoadingScreenEnable?.Invoke(true);
        LoadScreenController.OnLoadingTextChanged?.Invoke("Descargando imagen ...");

        OnDownloadMenuAction?.Invoke();
    }

    #region DAYS
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
        UIHelpers.SetCanvasGroup(_foodSelectionCanvasGroup, false);
        UIHelpers.SetCanvasGroup(_optionMenuCanvasGroup, true);

        _foodTexts[_currentFoodIndex].text = foodName;
        FoodNames[_currentFoodIndex] = foodName;

        _createButton.gameObject.SetActive(CheckIfAllFoodNamesAreSet() && CheckIfAllFoodNamesAreDifferent());        
    }

    private bool CheckIfAllFoodNamesAreSet()
    {
        foreach (var item in FoodNames)
        {
            if (string.IsNullOrEmpty(item))
            {
                return false;
            }
        }
        return true;
    }
    private bool CheckIfAllFoodNamesAreDifferent()
    {
        HashSet<string> uniqueNames = new HashSet<string>();
        foreach (var item in FoodNames)
        {
            if (!string.IsNullOrEmpty(item))
            {
                if (!uniqueNames.Add(item))
                {
                    return false; // Duplicate found
                }
            }
        }
        return true; // All names are unique
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
