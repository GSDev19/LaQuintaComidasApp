using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Utilities;

public class FoodDictionaryLoader : Singleton<FoodDictionaryLoader>
{
    public static Action OnAppLoaded;

    public string sheetCSVUrl = "https://docs.google.com/spreadsheets/d/e/YOUR_ID/pub?output=csv";
    public static UDictionary<string, string> FoodDictionary => Instance.foodLinkDictionary;
    public UDictionary<string, string> foodLinkDictionary = new UDictionary<string, string>();
    private static List<Sprite> _loadedSprites = new List<Sprite>();

    public Button _optionButtonPrefab;

    [Header("Options Pool")]
    public Transform _optionsContentParent;
    private int _maxOptionButtons = 200;
    private List<Button> _optionButtonsPool = new List<Button>();
    public static List<Button> OptionButtonsPool => Instance._optionButtonsPool;


    protected override void Awake()
    {
        base.Awake();
        foodLinkDictionary.Clear();
        _maxOptionButtons = 0;
    }

    void Start()
    {
        StartCoroutine(LoadApp());
    }

    IEnumerator LoadApp()
    {
        LoadScreenController.OnLoadingScreenEnable?.Invoke(true);
        LoadScreenController.OnLoadingTextChanged?.Invoke("Cargando links ...");

        UnityWebRequest www = UnityWebRequest.Get(sheetCSVUrl);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            string error = $"Error loading sheet: {www.error}";
            LoadScreenController.OnLoadingTextChanged?.Invoke(error);
            Debug.LogError(error);
            yield break;
        }

        string text = www.downloadHandler.text;
        string[] rows = text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (string row in rows)
        {
            string[] columns = row.Split(',');

            if (columns.Length >= 2)
            {
                string name = columns[0].Trim().Replace("\"", "");
                string link = columns[1].Trim().Replace("\"", "");

                if (!foodLinkDictionary.ContainsKey(name))
                {
                    foodLinkDictionary.Add(name, link);
                }
            }
        }

        yield return new WaitForEndOfFrame();

        _maxOptionButtons = foodLinkDictionary.Count;
        StartCoroutine(InitializeOptionButtonsPoolCoroutine());
    }
    public static void RequestSprite(string name, Image targetImage, Action onComplete = null)
    {
        Instance.StartCoroutine(DownloadImage(name, targetImage, onComplete));
    }

    public static IEnumerator DownloadImage(string name, Image targetImage, Action onComplete = null)
    {
        string textureLink = LoadAssetHelper.ConvertToDirectLink(FoodDictionary[name]);
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(textureLink);

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            string error = $"Error downloading image for {name}: {www.error}";
            LoadScreenController.OnLoadingTextChanged?.Invoke(error);
            Debug.LogError(error);
        }
        else
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(www);

            Texture2D resized = LoadAssetHelper.ResizeTexture(texture, 512, 512);
            Sprite sprite = Sprite.Create(resized, new Rect(0, 0, 512, 512), new Vector2(0.5f, 0.5f));

            targetImage.sprite = sprite;
            _loadedSprites.Add(sprite);
        }

        if(onComplete != null) onComplete?.Invoke();
    }
    public static void ClearLoadedSprites()
    {
        foreach (var sprite in _loadedSprites)
        {
            if (sprite != null)
            {
                Destroy(sprite.texture);  // destroy the texture first
                Destroy(sprite);          // destroy the sprite itself
            }
        }

        _loadedSprites.Clear();
    }

    private IEnumerator InitializeOptionButtonsPoolCoroutine()
    {
        _optionButtonsPool = new List<Button>();
        LoadScreenController.OnLoadingTextChanged?.Invoke("Creando opciones...");

        for (int i = 0; i < _maxOptionButtons; i++)
        {
            Button button = Instantiate(_optionButtonPrefab, _optionsContentParent);
            button.gameObject.SetActive(false);
            _optionButtonsPool.Add(button);

            // Wait for next frame to reduce frame impact
            yield return null;
        }

        LoadScreenController.OnLoadingScreenEnable?.Invoke(false);
        OnAppLoaded?.Invoke();
    }
    public static void TurnOffOptionButtonsPool()
    {
        foreach (var item in OptionButtonsPool)
        {
            item.gameObject.SetActive(false);
        }
    }
}
