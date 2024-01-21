using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerScreen : MonoBehaviour
{
    [SerializeField] private int _width;
    [SerializeField] private int _height;
    [SerializeField] private bool _isFullscreen;

    // Start is called before the first frame update
    void Start()
    {
        if (Screen.width > Screen.height)
        {
            Screen.SetResolution(_width, _height, _isFullscreen);
        }
    }
}
