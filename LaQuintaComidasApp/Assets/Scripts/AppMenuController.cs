using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

public class AppMenuController : Singleton<AppMenuController>
{
    private const string DAY_BUTTONTEXT = "Día";
    private const string AMOUNT_BUTTONTEXT = "Cantidad";

    public static Action OnCreateImageAction;
    public static Action OnDownloadMenuAction;
    public static Action OnRemoveMenuAction;

    public Button _createButton;
    public Button _downloadButton;
    public Button _backButton;

    public UDictionary<Panels, CanvasGroup> _canvasGroups = new UDictionary<Panels, CanvasGroup>();
    public CanvasGroup _appButtonsCanvasGroup;

    [SerializeField] private Transform _selectionAreaTransform;
    [SerializeField] private GameObject _foodSelectionsContainerPrefab;

    public SelectionPanel selectionPanelPrefab;

    public AppColors AppColors => _appColors;
    [SerializeField] private AppColors _appColors = new AppColors();

    [Header("Days")]
    public static Days SelectedDayString => Instance._selectedDay;
    private Days _selectedDay;
    private SelectionPanel _daySelectionPanel;
    private bool _hasSelectedDay = false;

    [Header("Amount")]
    public static Amount SelectedAmount => Instance._selectedAmount;
    private Amount _selectedAmount;
    private SelectionPanel _amountSelectionPanel;

    [Header("Food")]
    private int _currentFoodIndex;
    private Transform _foodSelectionParent;
    private List<SelectionPanel> _foodSelectionPanels;

    public List<string> FoodNames = new List<string>(6);

    protected override void Awake()
    {
        base.Awake();

        TransformHelper.DeleteAllChildren(_selectionAreaTransform);

        _daySelectionPanel = Instantiate(selectionPanelPrefab, _selectionAreaTransform);
        _daySelectionPanel.InitializeSelectionPanel(_appColors.selectionPanel_Base, DAY_BUTTONTEXT, OnDayButtonClicked);
        _selectedDay = Days.Lunes;
        _hasSelectedDay = false;

        _amountSelectionPanel = Instantiate(selectionPanelPrefab, _selectionAreaTransform);
        _amountSelectionPanel.InitializeSelectionPanel(_appColors.selectionPanel_Base, AMOUNT_BUTTONTEXT, OnAmountButtonClicked);
        _selectedAmount = Amount.Cinco;

        _foodSelectionParent = Instantiate(_foodSelectionsContainerPrefab, _selectionAreaTransform).transform;

        _foodSelectionPanels = new List<SelectionPanel>();

        _createButton.onClick.AddListener(() => OnCreateButtonClicked());
        _backButton.onClick.AddListener(() => OnBackButtonClicked());
        _downloadButton.onClick.AddListener(() => OnDownloadButtonClicked());

        _createButton.gameObject.SetActive(false);
        _downloadButton.gameObject.SetActive(false);
        _backButton.gameObject.SetActive(false);

        EnableMenuCanvas(Panels.MainMenu);
        UIHelpers.SetCanvasGroup(_appButtonsCanvasGroup, true);
    }

    private void ShowOptionsPanel(List<string> optionNames, Action<string> onOptionSelected)
    {
        EnableMenuCanvas(Panels.OptionsPanel);
        UIHelpers.SetCanvasGroup(_appButtonsCanvasGroup, false);

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
        UIHelpers.SetCanvasGroup(_appButtonsCanvasGroup, true);

        _createButton.gameObject.SetActive(true);
        _downloadButton.gameObject.SetActive(false);
        _backButton.gameObject.SetActive(false);

        OnRemoveMenuAction?.Invoke();
    }
    private void OnCreateButtonClicked()
    {
        LoadScreenController.OnLoadingScreenEnable?.Invoke(true);
        LoadScreenController.OnLoadingTextChanged?.Invoke("Creando imagen ...");

        EnableMenuCanvas(Panels.MenuDisplay);
        UIHelpers.SetCanvasGroup(_appButtonsCanvasGroup, true);

        _createButton.gameObject.SetActive(false);
        _downloadButton.gameObject.SetActive(true);
        _backButton.gameObject.SetActive(true);

        OnCreateImageAction?.Invoke();
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
        _daySelectionPanel.TextTMP.text = day;

        EnableMenuCanvas(Panels.MainMenu);
        UIHelpers.SetCanvasGroup(_appButtonsCanvasGroup, true);

        _daySelectionPanel.SetBGColor(_appColors.selectionPanel_CompletedCorrect);
        _hasSelectedDay = true;

        _createButton.gameObject.SetActive(CheckIfAllFoodNamesAreSet() && CheckIfAllFoodNamesAreDifferent() && _hasSelectedDay);
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
        _amountSelectionPanel.TextTMP.text = _selectedAmount.ToString();

        TransformHelper.DeleteAllChildren(_foodSelectionParent);

        _foodSelectionPanels.Clear();

        int foodCount = (int)_selectedAmount;

        for (int i = 0; i < foodCount; i++)
        {
            SelectionPanel foodSelection = Instantiate(selectionPanelPrefab, _foodSelectionParent);
            _foodSelectionPanels.Add(foodSelection);
            foodSelection.InitializeSelectionPanel(_appColors.selectionPanel_Base, ((Foods)i).ToString(), () => OnFoodButtonClicked(foodSelection), i);
            foodSelection.TextTMP.text = string.Empty;
        }

        FoodNames = new List<string>(foodCount);
        for (int i = 0; i < foodCount; i++)
        {
            FoodNames.Add(string.Empty);
        }

        _createButton.gameObject.SetActive(false);
        _downloadButton.gameObject.SetActive(false);
        _backButton.gameObject.SetActive(false);

        EnableMenuCanvas(Panels.MainMenu);
        UIHelpers.SetCanvasGroup(_appButtonsCanvasGroup, true);

        _amountSelectionPanel.SetBGColor(_appColors.selectionPanel_CompletedCorrect);
    }

    #endregion

    #region FOODS

    private void OnFoodButtonClicked(SelectionPanel selectionPanel)
    {
        _currentFoodIndex = selectionPanel.Index;
        var foodOptions = FoodDictionaryLoader.FoodDictionary.Keys.ToList();
        ShowOptionsPanel(foodOptions, OnFoodOptionSelected);
    }

    private void OnFoodOptionSelected(string foodName)
    {
        _foodSelectionPanels[_currentFoodIndex].TextTMP.text = foodName;
        FoodNames[_currentFoodIndex] = foodName;

        HighlightFoodSelectionDuplicates(); // 👈 This does the coloring logic

        EnableMenuCanvas(Panels.MainMenu);
        UIHelpers.SetCanvasGroup(_appButtonsCanvasGroup, true);

        bool isCompleted = CheckIfAllFoodNamesAreSet() && CheckIfAllFoodNamesAreDifferent() && _hasSelectedDay;
        _createButton.gameObject.SetActive(isCompleted);

        if (!_hasSelectedDay)
        {
            _daySelectionPanel.SetBGColor(_appColors.selectionPanel_CompletedIncorrect);
        }
    }
    private bool CheckIfAllFoodNamesAreSet()
    {
        if(FoodNames.Count == 0) return false;

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
    private void HighlightFoodSelectionDuplicates()
    {
        // Dictionary to track which panels have which text
        Dictionary<string, List<SelectionPanel>> textToPanels = new Dictionary<string, List<SelectionPanel>>();

        foreach (var panel in _foodSelectionPanels)
        {
            string foodName = panel.TextTMP.text;
            if (string.IsNullOrEmpty(foodName)) continue;

            if (!textToPanels.ContainsKey(foodName))
            {
                textToPanels[foodName] = new List<SelectionPanel>();
            }
            textToPanels[foodName].Add(panel);
        }

        // Apply colors
        foreach (var panel in _foodSelectionPanels)
        {
            string foodName = panel.TextTMP.text;
            if (string.IsNullOrEmpty(foodName))
            {
                continue; // Not evaluated
            }

            if (textToPanels[foodName].Count > 1)
            {
                // Repeated -> incorrect
                panel.SetBGColor(_appColors.selectionPanel_CompletedIncorrect);
            }
            else
            {
                // Unique -> correct
                panel.SetBGColor(_appColors.selectionPanel_CompletedCorrect);
            }
        }
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
[Serializable]
public struct AppColors
{
    public Color selectionPanel_Base;
    public Color selectionPanel_CompletedCorrect;
    public Color selectionPanel_CompletedIncorrect;

    public Color selectionPanelTextColor;
    public Color buttonColor;
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
