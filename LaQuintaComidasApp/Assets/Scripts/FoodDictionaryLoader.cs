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

    public static UDictionary<string, string> FoodDictionary => Instance.foodDictionary;
    public UDictionary<string, string> foodDictionary = new UDictionary<string, string>();

    public UDictionary<string, Sprite> imagesDictionary = new UDictionary<string, Sprite>();

    public Button _optionButtonPrefab;

    [Header("Options Pool")]
    public Transform _optionsContentParent;
    private int _maxOptionButtons = 200;
    private List<Button> _optionButtonsPool = new List<Button>();
    public static List<Button> OptionButtonsPool => Instance._optionButtonsPool;


    protected override void Awake()
    {
        base.Awake();
        foodDictionary.Clear();
        imagesDictionary.Clear();
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

        int pendingDownloads = 0;

        foreach (string row in rows)
        {
            string[] columns = row.Split(',');

            if (columns.Length >= 2)
            {
                string name = columns[0].Trim().Replace("\"", "");
                string link = columns[1].Trim().Replace("\"", "");

                if (!foodDictionary.ContainsKey(name))
                {
                    foodDictionary.Add(name, link);
                    pendingDownloads++;
                    StartCoroutine(DownloadImage(name, ConvertToDirectLink(link), () =>
                    {
                        pendingDownloads--;
                    }));
                }
            }
        }

        LoadScreenController.OnLoadingTextChanged?.Invoke("Cargando imagenes ...");

        yield return new WaitUntil(() => pendingDownloads <= 0);

        _maxOptionButtons = foodDictionary.Count;
        StartCoroutine(InitializeOptionButtonsPoolCoroutine());
    }

    string ConvertToDirectLink(string shareLink)
    {
        string id = "";
        var parts = shareLink.Split('/');
        for (int i = 0; i < parts.Length; i++)
        {
            if (parts[i] == "d" && i + 1 < parts.Length)
            {
                id = parts[i + 1];
                break;
            }
        }

        return $"https://drive.google.com/uc?export=download&id={id}";
    }

    IEnumerator DownloadImage(string name, string url, Action onComplete)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
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

            Texture2D resized = ResizeTexture(texture, 512, 512);
            Sprite sprite = Sprite.Create(resized, new Rect(0, 0, 512, 512), new Vector2(0.5f, 0.5f));

            if (!imagesDictionary.ContainsKey(name))
            {
                imagesDictionary.Add(name, sprite);
                // Debug.Log($"🖼️ Sprite loaded: {name}");
            }
        }

        onComplete?.Invoke();
    }

    Texture2D ResizeTexture(Texture2D source, int width, int height)
    {
        RenderTexture rt = RenderTexture.GetTemporary(width, height);
        Graphics.Blit(source, rt);

        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = rt;

        Texture2D result = new Texture2D(width, height);
        result.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        result.Apply();

        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(rt);

        return result;
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
