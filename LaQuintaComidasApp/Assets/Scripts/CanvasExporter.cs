using System.IO;
using UnityEngine;

public class CanvasExporter : MonoBehaviour
{
    private Camera canvasCamera;           // C�mara que ve el canvas
    public int imageWidth = 1920;
    public int imageHeight = 1080;
    public string fileName = "Menu.png";

    private void Awake()
    {
        canvasCamera = GetComponent<Camera>();
    }

    private void OnEnable()
    {
        AppMenuController.OnDownloadMenuAction += CaptureAndShareCanvas;
    }

    private void OnDisable()
    {
        AppMenuController.OnDownloadMenuAction -= CaptureAndShareCanvas;
    }

    public void CaptureAndShareCanvas()
    {
        LoadScreenController.OnLoadingTextChanged?.Invoke("Renderizando ...");

        // Crear RenderTexture y asignarla a la c�mara
        RenderTexture renderTex = new RenderTexture(imageWidth, imageHeight, 24);
        canvasCamera.targetTexture = renderTex;
        RenderTexture.active = renderTex;

        // Renderizar
        canvasCamera.Render();

        // Copiar a Texture2D
        Texture2D tex = new Texture2D(imageWidth, imageHeight, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, imageWidth, imageHeight), 0, 0);
        tex.Apply();

        // Limpiar
        canvasCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(renderTex);

        // Guardar en almacenamiento accesible
        string path = Path.Combine(Application.persistentDataPath, fileName);
        byte[] bytes = tex.EncodeToPNG();
        File.WriteAllBytes(path, bytes);
        Debug.Log("Imagen guardada en: " + path);

        // Compartir con Native Share
        new NativeShare().AddFile(path)
            //.SetSubject("�Mira esta imagen!")
            //.SetText("Te comparto una imagen desde mi app Unity")
            //.SetTitle("Compartir imagen")
            .SetCallback((result, shareTarget) =>
            {
                Debug.Log("Resultado del share: " + result + ", destino: " + shareTarget);
            })
            .Share();

        LoadScreenController.OnLoadingScreenEnable?.Invoke(false);
    }
}
