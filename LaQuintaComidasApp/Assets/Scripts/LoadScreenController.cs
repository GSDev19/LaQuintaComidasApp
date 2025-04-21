using System;
using TMPro;
using UnityEngine;

public class LoadScreenController : Singleton<LoadScreenController>
{
    public static Action<bool> OnLoadingScreenEnable;
    public static Action<string> OnLoadingTextChanged;


    private CanvasGroup _loaderCanvasGroup = null;
    private TextMeshProUGUI _loadingText = null;

    protected override void Awake()
    {
        base.Awake();

        _loaderCanvasGroup = GetComponent<CanvasGroup>();
        _loadingText = GetComponentInChildren<TextMeshProUGUI>();

        EnableLoadScreen(true);
    }
    private void OnEnable()
    {
        OnLoadingScreenEnable += EnableLoadScreen;
        OnLoadingTextChanged += UpdateLoadingText;
    }
    private void OnDisable()
    {
        OnLoadingScreenEnable -= EnableLoadScreen;
        OnLoadingTextChanged -= UpdateLoadingText;
    }

    private void EnableLoadScreen(bool enable)
    {
        UIHelpers.SetCanvasGroup(_loaderCanvasGroup, enable);
    }
    private void UpdateLoadingText(string text)
    {
        if (_loadingText != null)
        {
            _loadingText.text = text;
        }
    }
}
