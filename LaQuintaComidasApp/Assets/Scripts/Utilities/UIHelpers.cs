using System;
using System.Collections;
using UnityEngine;

public static class UIHelpers
{
    /// <summary>
    /// Static method to enable or disable a Canvas Group with alpha and interactable
    /// </summary>
    /// <param name="canvasGroup"> Target Canvas Group</param>
    /// <param name="value"></param>
    public static void SetCanvasGroup(CanvasGroup canvasGroup,bool value)
    {
        switch(value)
        {
            case true:
                canvasGroup.alpha = 1;
                break;

            case false:
                canvasGroup.alpha = 0;               
                break;
        }
        canvasGroup.blocksRaycasts = value;
        canvasGroup.interactable = value;
    }

    public static void SetInteractableCanvasGroup(CanvasGroup canvasGroup, bool value)
    {
        canvasGroup.blocksRaycasts = value;
        canvasGroup.interactable = value;      
    }

    /// <summary>
    /// Static method to Fade a Canvas Group in some time
    /// </summary>
    /// <param name="canvasGroup"></param>
    /// <param name="startAlpha"></param>
    /// <param name="targetAlpha"></param>
    /// <param name="duration"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public static IEnumerator FadeCanvasGroupCoroutine(CanvasGroup canvasGroup, float startAlpha, float targetAlpha, float duration, Action callback = null)
    {
        float startTime = Time.time;

        while (Time.time - startTime < duration)
        {
            float progress = (Time.time - startTime) / duration;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, progress);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;

        callback?.Invoke();
    }

}
