using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public GameObject emailLoginPanel;
    public GameObject mainLoginPanel;
    public GameObject emailRegisterPanel;
    public GameObject gameModePanel;

    private string userEmail;
    private string userPassword;
    private string userName;

    #region Email Input
    public void GetUserEmail(string email)
    {
        userEmail = email;
    }

    public void GetUserPassword(string pass)
    {
        userPassword = pass;
    }

    public void GetUsername(string username)
    {
        userName = username;
    }

    #endregion

    #region Button Click Events

    public void OnClickBack()
    {
        if (emailLoginPanel.activeSelf == true)
        {
            emailLoginPanel.SetActive(false);
            mainLoginPanel.SetActive(true);
        }
        if (emailRegisterPanel.activeSelf == true)
        {
            emailRegisterPanel.SetActive(false);
            emailLoginPanel.SetActive(true);
        }
    }

    //Play as Guest button on Main Login Page
    public void OnClickLoginGuest()
    {
        PlayFabController.PFC.CreateMobileDeviceGuest();
    }

    //Login with Email button on main login page
    public void OnClickEmailLogin()
    {
        mainLoginPanel.SetActive(false);
        emailLoginPanel.SetActive(true);
    }

    //login button on Email login page
    public void OnClickEmailLoginInput()
    {
        PlayFabController.PFC.LoginEmail(userEmail, userPassword);
    }

    //Register Button on Email Register Panel
    public void OnClickEmailRegisterInput()
    {
        PlayFabController.PFC.RegisterEmail(userEmail, userPassword, userName);
    }


    //register button on Email Login Panel
    public void OnClickRegisterEmail()
    {
        emailLoginPanel.SetActive(false);
        emailRegisterPanel.SetActive(true);
    }

    //Login Button on Email Register Panel
    public void OnClickLoginFromEmailRegister()
    {
        emailRegisterPanel.SetActive(false);
        emailLoginPanel.SetActive(true);
    }

    public void OnClickLoginWithFacebook()
    {
        PlayFabController.PFC.LoginFacebook();
    }
    #endregion

    public void OnClickSubmitDisplayNameButton()
    {
        PlayFabController.PFC.UpdateDisplayName(userName);
    }

    public void OnClickConnectPhorButton()
    {
        gameModePanel.SetActive(false);
        //if online, call the online version
        if (PlayFabController.PFC.IsConnectedToPlayfab())
        {

        } else
        {

        }
        //else, call the AI version
    }

    public void OnClickConnect4Button()
    {
        gameModePanel.SetActive(false);
        //if online, call the online version
        if (PlayFabController.PFC.IsConnectedToPlayfab())
        {

        }
        //else, call the AI version
        else
        {

        }

    }

}
