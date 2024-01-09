using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loader : MonoBehaviour
{
    public static Action onLoaderCallBack;

    public static void Load(String scene)
    {
        onLoaderCallBack = () =>
        {
            SceneManager.LoadScene(scene);
        };
        SceneManager.LoadScene("LoadingScreen");
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
