using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public void Awake()
    {
        Screen.fullScreen = false;
    }

    public void OnClickPlayFabLogin()
    {
        SceneManager.LoadSceneAsync("Login", LoadSceneMode.Additive);
    }

    public void OnClickPlayFabRegister()
    {
        SceneManager.LoadSceneAsync("RegisterScene", LoadSceneMode.Additive);
    }
}
