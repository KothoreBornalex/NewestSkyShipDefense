using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader
{
    private class LoadingMonobehavior : MonoBehaviour
    {

    }

    public static Action onLoaderCallBack;

    public static void Load(String scene)
    {
        onLoaderCallBack = () =>
        {
            GameObject loadingGameobject = new GameObject("Loading Game Object");
            loadingGameobject.AddComponent<LoadingMonobehavior>().StartCoroutine(LoadSceneAsync(scene));
            //LoadSceneAsync(scene);
        };
        SceneManager.LoadScene("LoadingScreen");
    }

    private static IEnumerator LoadSceneAsync(String scene)
    {
        yield return null;

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(scene);

        while(asyncOperation != null)
        {
            yield return null;
        }
        
    }

    public static void LoaderCallBack()
    {
        if (onLoaderCallBack != null)
        {
            onLoaderCallBack();
            onLoaderCallBack = null;
        }
    }
}
