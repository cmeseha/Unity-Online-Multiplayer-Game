using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayFabAuth : MonoBehaviour
{

    public InputField userName;
    public InputField userPass;
    public string SceneToLoad;

    public void Login()
    {
        var request = new LoginWithPlayFabRequest { Username = userName.text, Password = userPass.text };
        PlayFabClientAPI.LoginWithPlayFab(request, OnLoginEmailSuccess, OnLoginEmailFailure);
    }

    private void OnLoginEmailSuccess(PlayFab.ClientModels.LoginResult result)
    {
        Debug.Log("Congratulations, API called successfully. Login method: Email");
        // GetPlayerCombinedInfo();
        Alerts a = new Alerts();
        StartCoroutine(a.CreateNewAlert(userName.text + ", you have logged in!"));
        SceneManager.LoadScene(SceneToLoad);

        
    }

    private void OnLoginEmailFailure(PlayFabError error)
    {
        Debug.Log(error.GenerateErrorReport());
        Alerts a = new Alerts();
        StartCoroutine(a.CreateNewAlert(error.ErrorMessage));
    }
}
