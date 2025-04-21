using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Utilities;

public class FoodDictionaryLoader : MonoBehaviour
{
    public string sheetCSVUrl = "https://docs.google.com/spreadsheets/d/e/YOUR_ID/pub?output=csv";

    public UDictionary<string, string> foodDictionary = new UDictionary<string, string>();
    public UDictionary<string, Sprite> imagesDictionary = new UDictionary<string, Sprite>();

    void Start()
    {
        StartCoroutine(LoadCSV());
    }

    IEnumerator LoadCSV()
    {
        UnityWebRequest www = UnityWebRequest.Get(sheetCSVUrl);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error loading sheet: " + www.error);
            yield break;
        }

        string text = www.downloadHandler.text;
        string[] rows = text.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);

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
                    StartCoroutine(DownloadImage(name, ConvertToDirectLink(link)));
                }
            }
        }
    }

    string ConvertToDirectLink(string shareLink)
    {
        // Extract file ID
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

    IEnumerator DownloadImage(string name, string url)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Error downloading image for {name}: {www.error}");
            yield break;
        }

        Texture2D texture = DownloadHandlerTexture.GetContent(www);

        // Resize to 512x512 if needed
        Texture2D resized = ResizeTexture(texture, 512, 512);
        Sprite sprite = Sprite.Create(resized, new Rect(0, 0, 512, 512), new Vector2(0.5f, 0.5f));

        if (!imagesDictionary.ContainsKey(name))
        {
            imagesDictionary.Add(name, sprite);
            Debug.Log($"🖼️ Sprite loaded: {name}");
        }
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
}
