using UnityEngine;

public static class LoadAssetHelper 
{
    public static string ConvertToDirectLink(string shareLink)
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
    public static Texture2D ResizeTexture(Texture2D source, int width, int height)
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
