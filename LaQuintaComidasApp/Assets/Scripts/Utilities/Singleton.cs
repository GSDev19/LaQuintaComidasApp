using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                // Updated to use the recommended method
                _instance = FindFirstObjectByType<T>();

                if (_instance == null)
                {
                    //Debug.LogWarning("Singleton instance of type " + typeof(T) + " not found in the scene.");
                }
                //else
                //{
                //    Debug.Log($"Singleton instance found: {_instance.gameObject.name}");
                //}
            }
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            //DontDestroyOnLoad(gameObject);  // Optional, uncomment if you want this to persist across scenes
            //Debug.Log($"Singleton instance set: {_instance.gameObject.name}");
        }
        else if (_instance != this)
        {
            //Debug.LogWarning($"Instance already exists: {_instance.gameObject.name}, destroying duplicate: {gameObject.name}");
            Destroy(gameObject);  // Destroy the duplicate
        }
    }
}
