using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

public class AppMenuController : Singleton<AppMenuController>
{
    public static Action OnCreateImageAction;
    public static Action OnDownloadMenuAction;
    public static Action OnRemoveMenuAction;

    public Button _createButton;
    public Button _downloadButton;
    public Button _backButton;

    public SelectionPanel selectionPanelPrefab;

    public UDictionary<Panels, CanvasGroup> _canvasGroups = new UDictionary<Panels, CanvasGroup>();

    public CanvasGroup _buttonsCanvasGroup;

    [Header("Days")]
    public static Days SelectedDayString => Instance._selectedDay;
    public Days _selectedDay;
    public Button _daySelectionButton;
    public TextMeshProUGUI _daySelectionText;

    [Header("Amount")]
    public static Amount SelectedAmount => Instance._selectedAmount;
    public Amount _selectedAmount;
    public Button _amountSelectionButton;
    public TextMeshProUGUI _amountSelectionText;

    [Header("Food")]
    public int _currentFoodIndex;
    public Transform _foodSelectionParent;
    public List<Button> _foodButtons;
    public List<TextMeshProUGUI> _foodTexts;

    public List<string> FoodNames = new List<string>(6);

    protected override void Awake()
    {
        base.Awake();

        _createButton.onClick.AddListener(() => OnCreateButtonClicked());
        _backButton.onClick.AddListener(() => OnBackButtonClicked());
        _downloadButton.onClick.AddListener(() => OnDownloadButtonClicked());

        _createButton.gameObject.SetActive(false);
        _downloadButton.gameObject.SetActive(false);
        _backButton.gameObject.SetActive(false);

        EnableMenuCanvas(Panels.MainMenu);
        UIHelpers.SetCanvasGroup(_buttonsCanvasGroup, true);

        _selectedDay = Days.Lunes;
        _daySelectionButton.onClick.AddListener(() => OnDayButtonClicked());

        _selectedAmount = Amount.Seis;
        _amountSelectionText.text = string.Empty;
        _amountSelectionButton.onClick.AddListener(() => OnAmountButtonClicked());
    }

    private void ShowOptionsPanel(List<string> optionNames, Action<string> onOptionSelected)
    {
        EnableMenuCanvas(Panels.OptionsPanel);
        UIHelpers.SetCanvasGroup(_buttonsCanvasGroup, false);

        FoodDictionaryLoader.TurnOffOptionButtonsPool();

        for (int i = 0; i < optionNames.Count; i++)
        {
            var button = FoodDictionaryLoader.OptionButtonsPool[i];
            button.gameObject.SetActive(true);
            button.GetComponentInChildren<TextMeshProUGUI>().text = optionNames[i];

            button.onClick.RemoveAllListeners();
            string optionValue = optionNames[i];
            button.onClick.AddListener(() =>
            {
                onOptionSelected(optionValue);
            });
        }
    }

    #region BUTTONS
    private void OnBackButtonClicked()
    {
        EnableMenuCanvas(Panels.MainMenu);
        UIHelpers.SetCanvasGroup(_buttonsCanvasGroup, true);

        OnRemoveMenuAction?.Invoke();

        _createButton.gameObject.SetActive(true);
        _downloadButton.gameObject.SetActive(false);
        _backButton.gameObject.SetActive(false);
    }
    private void OnCreateButtonClicked()
    {
        LoadScreenController.OnLoadingScreenEnable?.Invoke(true);
        LoadScreenController.OnLoadingTextChanged?.Invoke("Creando imagen ...");

        OnCreateImageAction?.Invoke();

        EnableMenuCanvas(Panels.MenuDisplay);
        UIHelpers.SetCanvasGroup(_buttonsCanvasGroup, true);

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

    #endregion

    #region DAYS
    private void OnDayButtonClicked()
    {
        var days = Enum.GetNames(typeof(Days)).ToList();
        ShowOptionsPanel(days, OnDayOptionSelected);
    }
    private void OnDayOptionSelected(string day)
    {
        _selectedDay = Enum.Parse<Days>(day);
        _daySelectionText.text = day;

        EnableMenuCanvas(Panels.MainMenu);
        UIHelpers.SetCanvasGroup(_buttonsCanvasGroup, true);
    }
    #endregion

    #region AMOUNT
    private void OnAmountButtonClicked()
    {
        var amounts = Enum.GetNames(typeof(Amount)).ToList();
        ShowOptionsPanel(amounts, OnAmountOptionSelected);
    }

    private void OnAmountOptionSelected(string amount)
    {
        _selectedAmount = Enum.Parse<Amount>(amount);
        _amountSelectionText.text = amount;

        TransformHelper.DeleteAllChildren(_foodSelectionParent);
        _foodButtons.Clear();
        _foodTexts.Clear();

        int foodCount = (int)_selectedAmount;

        for (int i = 0; i < foodCount; i++)
        {
            SelectionPanel foodSelection = Instantiate(selectionPanelPrefab, _foodSelectionParent);
            foodSelection.foodNameText.text = string.Empty;
            _foodButtons.Add(foodSelection.foodButon);
            _foodTexts.Add(foodSelection.foodNameText);
        }

        SetFoodButtons();

        FoodNames = new List<string>(foodCount);
        for (int i = 0; i < foodCount; i++)
        {
            FoodNames.Add(string.Empty);
        }

        _createButton.gameObject.SetActive(false);
        _downloadButton.gameObject.SetActive(false);
        _backButton.gameObject.SetActive(false);

        EnableMenuCanvas(Panels.MainMenu);
        UIHelpers.SetCanvasGroup(_buttonsCanvasGroup, true);
    }

    #endregion

    #region FOODS

    private void OnFoodButtonClicked(int index)
    {
        _currentFoodIndex = index;
        var foodOptions = FoodDictionaryLoader.FoodDictionary.Keys.ToList();
        ShowOptionsPanel(foodOptions, OnFoodOptionSelected);
    }

    private void OnFoodOptionSelected(string foodName)
    {
        _foodTexts[_currentFoodIndex].text = foodName;
        FoodNames[_currentFoodIndex] = foodName;

        EnableMenuCanvas(Panels.MainMenu);
        UIHelpers.SetCanvasGroup(_buttonsCanvasGroup, true);

        _createButton.gameObject.SetActive(CheckIfAllFoodNamesAreSet() && CheckIfAllFoodNamesAreDifferent());
    }

    private void SetFoodButtons()
    {
        for (int i = 0; i < _foodButtons.Count; i++)
        {
            int index = i; // Prevent closure issue
            _foodButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = ((Foods)i).ToString();
            _foodButtons[i].onClick.AddListener(() =>
            {
                OnFoodButtonClicked(index);
            });
        }
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

    private void EnableMenuCanvas(Panels panel)
    {
        foreach (var item in _canvasGroups)
        {
            bool active = item.Key == panel;
            UIHelpers.SetCanvasGroup(item.Value, active);
        }
    }
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
public enum Amount
{
    //Cuatro,
    Cinco = 5,
    Seis = 6
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
public enum Panels
{
    MainMenu,
    OptionsPanel,
    MenuDisplay
}
