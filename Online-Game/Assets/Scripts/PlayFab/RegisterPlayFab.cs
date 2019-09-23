using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;

public class RegisterPlayFab : MonoBehaviour
{
    public InputField userName;
    public InputField userPass;
    public InputField userEmail;

    public void CreatePlayFabAccount()
    {
        var registerRequest = new RegisterPlayFabUserRequest { Email = userEmail.text, Password = userPass.text, Username = userName.text, DisplayName = userName.text };
        PlayFabClientAPI.RegisterPlayFabUser(registerRequest, OnRegisterPlayFabSuccess, OnRegisterPlayFabFailure);
    }

    private void OnRegisterPlayFabFailure(PlayFabError error)
    {
        Debug.Log(error.GenerateErrorReport());
        Alerts a = new Alerts();
        StartCoroutine(a.CreateNewAlert(error.ErrorMessage));
    }

    private void OnRegisterPlayFabSuccess(RegisterPlayFabUserResult result)
    {
        Alerts a = new Alerts();
        StartCoroutine(a.CreateNewAlert(result.Username + "has been created!"));
    }
}
