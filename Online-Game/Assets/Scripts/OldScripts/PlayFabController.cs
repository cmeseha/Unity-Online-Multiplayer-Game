using System;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;
using Facebook.Unity;
using System.Collections.Generic;

public class PlayFabController : MonoBehaviour
{

    public static PlayFabController PFC;

    public GameObject mainLoginPanel;
    public GameObject emailLoginPanel;
    public GameObject emailRegisterPanel;
    public GameObject usernameInputPanel;
    public GameObject gameModePanel; //TO DO NEXT - SET TO TRUE NEXT TO MAINLOGINPANEL = FALSE

    public GameObject LoggedInDialog; 

    public Texture2D NoProfilePic;

    //private or public?
    private bool isOnline;

    private void Awake()
    {
        FB.Init(SetInit, OnHideUnity);
        PlayerPrefs.SetInt("Tester", 1);
    }

    //Singleton Makes sure only one PlayFabController controller throughout game
    //Also makes sure that login happens once
    private void OnEnable()
    {
        if (PlayFabController.PFC == null)
        {
            PlayFabController.PFC = this;
        }
        else
        {
            if (PlayFabController.PFC != this)
            {
                Destroy(this.gameObject);
            }
        }
        DontDestroyOnLoad(this.gameObject);
    }

    public void Start()
    {
        //Note: Setting title Id here can be skipped if you have set the value in Editor Extensions already.
        if (string.IsNullOrEmpty(PlayFabSettings.TitleId))
        {
            PlayFabSettings.TitleId = "546BA"; // Please change this value to your own titleId from PlayFab Game Manager
        }

        //PlayerPrefs.DeleteAll(); //DEBUGGING: Disable / Enable automatic login 
                                 /* FOR DEBUGGING ONLY - Should not use this request in practice
                                 var request = new LoginWithCustomIDRequest { CustomId = "GettingStartedGuide", CreateAccount = true };
                                 PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
                                 */
        
        Debug.Log(PlayerPrefs.GetInt("Tester"));
        if (PlayerPrefs.GetInt("Tester") == 1)
        {
            mainLoginPanel.SetActive(true);
        } else
        {
            FrictionlessLoginMobileDevice();
        }
        Image ProfilePic = LoggedInDialog.GetComponent<Image>();
        ProfilePic.sprite = Sprite.Create(NoProfilePic, new Rect(0, 0, 128, 128), new Vector2());

        Text userName = LoggedInDialog.GetComponent<Text>();
        userName.text = "";
        isOnline = false;

    }

    #region Frictionless Login
    private void FrictionlessLoginMobileDevice()
    {
#if UNITY_ANDROID
        var requestAndroid = new LoginWithAndroidDeviceIDRequest { AndroidDeviceId = ReturnMobileID(), CreateAccount = false };
        PlayFabClientAPI.LoginWithAndroidDeviceID(requestAndroid, OnFrictionlessLoginMobileSuccess, OnFrictionlessLoginMobileFailure);
#endif
#if UNITY_IOS
        var requestIOS = new LoginWithIOSDeviceIDRequest { DeviceId = ReturnMobileID(), CreateAccount = false };
        PlayFabClientAPI.LoginWithIOSDeviceID(requestIOS, OnFrictionlessLoginMobileSuccess, OnFrictionlessLoginMobileFailure);
#endif
    }

    private void OnFrictionlessLoginMobileSuccess(PlayFab.ClientModels.LoginResult result)
    {
        Debug.Log("Loggged in, API called successfully. Login method: Device ID");
        mainLoginPanel.SetActive(false);
        gameModePanel.SetActive(true);
        GetPlayerCombinedInfo();
        LoggedInDialog.SetActive(true);
        isOnline = true;
    }

    private void OnFrictionlessLoginMobileFailure(PlayFabError error)
    {
        Debug.Log(error.GenerateErrorReport());
        CreateMobileDeviceGuest();
        //TODO: if failure is due to network error, create a panel that says that says "No network, you internet seems to be down" and put practice offline button 

    }
    #endregion

    #region Create/Login Guest Account with DeviceID
    //creates a PlayFab account if one doesn't exist for this device
    public void CreateMobileDeviceGuest()
    {
#if UNITY_ANDROID
        var requestAndroid = new LoginWithAndroidDeviceIDRequest { AndroidDeviceId = ReturnMobileID(), CreateAccount = true };
        PlayFabClientAPI.LoginWithAndroidDeviceID(requestAndroid, OnCreateLoginGuestSuccess, OnCreateLoginGuestFailure);
#endif
#if UNITY_IOS
        var requestIOS = new LoginWithIOSDeviceIDRequest { DeviceId = ReturnMobileID(), CreateAccount = true };
        PlayFabClientAPI.LoginWithIOSDeviceID(requestIOS, OnLoginGuestSuccess, OnLoginGuestFailure);
#endif
    }

    //if new Guest account successfully created with this device id 
    private void OnCreateLoginGuestSuccess(PlayFab.ClientModels.LoginResult result)
    {
        Debug.Log("Loggged in, API called successfully. Login method: Guest - Device ID");
        mainLoginPanel.SetActive(false);
        gameModePanel.SetActive(true);
        Text userName = LoggedInDialog.GetComponent<Text>();
        userName.text = "Guest";
        LoggedInDialog.SetActive(true);
        isOnline = true;
    }

    //Guest account could not be created with this device id 
    private void OnCreateLoginGuestFailure(PlayFabError error)
    {
        Debug.Log(error.GenerateErrorReport());
        mainLoginPanel.SetActive(true);
        isOnline = false;
    }

    #endregion

    #region Login / Register with Email Address

    public void LoginEmail(string email, string password)
    {
        var request = new LoginWithEmailAddressRequest { Email = email, Password = password };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginEmailSuccess, OnLoginEmailFailure);
    }

    private void OnLoginEmailSuccess(PlayFab.ClientModels.LoginResult result)
    {
        Debug.Log("Congratulations, API called successfully. Login method: Email");
        LinkDevice();
        mainLoginPanel.SetActive(false);
        gameModePanel.SetActive(true);
        GetPlayerCombinedInfo();
        LoggedInDialog.SetActive(true);
        isOnline = true;
    }

    private void OnLoginEmailFailure(PlayFabError error)
    {
        Debug.Log(error.GenerateErrorReport());
        isOnline = false;
    }

    public void RegisterEmail(string userEmail, string userPass, string userName)
    {
        var registerRequest = new RegisterPlayFabUserRequest { Email = userEmail, Password = userPass, Username = userName };
        PlayFabClientAPI.RegisterPlayFabUser(registerRequest, OnRegisterEmailSuccess, OnRegisterEmailFailure);
    }

    private void OnRegisterEmailFailure(PlayFabError error)
    {
        Debug.Log(error.GenerateErrorReport());
        isOnline = false;
    }

    private void OnRegisterEmailSuccess(RegisterPlayFabUserResult result)
    {
        Debug.Log("New Email Registered.");
        LinkDevice();
        UpdateDisplayName(result.Username);
        isOnline = true; //CHECKK if register email also logs player in
    }
    #endregion

    #region Login with Facebook

    private void SetInit()
    {
        if (FB.IsLoggedIn)
        {
            Debug.Log("Facebook is logged in");
            Debug.Log("Facebook Access Token: " + AccessToken.CurrentAccessToken.TokenString);
            Debug.Log("Facebook Access Token Expiration: " + AccessToken.CurrentAccessToken.ExpirationTime);
            var LoginWithFacebook = new LoginWithFacebookRequest { CreateAccount = false, AccessToken = AccessToken.CurrentAccessToken.TokenString };
            PlayFabClientAPI.LoginWithFacebook(LoginWithFacebook, OnPlayfabFacebookAuthComplete, OnPlayfabFacebookAuthFailed);
        }
        else
        {
            Debug.Log("Facebook is not logged in");
        }
    }

    private void OnHideUnity(bool isUnityShown)
    {
        if (!isUnityShown)
        {
            Time.timeScale = 0; //pause
        }
        else
        {
            Time.timeScale = 1; //unpause
        }
    }

    public void LoginFacebook()
    {
        List<string> permissions = new List<string>();
        permissions.Add("public_profile");
        FB.LogInWithPublishPermissions(permissions, FBAuthCallBack);
    }

    private void FBAuthCallBack(ILoginResult result)
    {
        if(result.Error != null)
        {
            Debug.Log(result.Error); //check error
        } else
        {
            if (FB.IsLoggedIn)
            {
                Debug.Log("Facebook is logged in");
                Debug.Log("Facebook Auth Complete. Access Token: " + AccessToken.CurrentAccessToken.TokenString);
                Debug.Log("Facebook Access Token Expiration: " + AccessToken.CurrentAccessToken.ExpirationTime);
                var LoginWithFacebook = new LoginWithFacebookRequest { CreateAccount = false, AccessToken = AccessToken.CurrentAccessToken.TokenString };
                PlayFabClientAPI.LoginWithFacebook(LoginWithFacebook, OnPlayfabFacebookAuthComplete, OnPlayfabFacebookAuthFailed);
            }
            else
            {
                Debug.Log("Facebook is not logged in");
            }
        }
    }

    private void OnPlayfabFacebookAuthComplete(PlayFab.ClientModels.LoginResult result)
    {
        Debug.Log("PlayFab Facebook Auth Complete. Session ticket: " + result.SessionTicket);
        LinkDevice();
        mainLoginPanel.SetActive(false);
        FB.API("/me/picture?type=square&height=128&width=128", HttpMethod.GET, SetFBProfilePicToPlayfab);
        gameModePanel.SetActive(true);
        GetPlayerCombinedInfo();
        LoggedInDialog.SetActive(true);
        isOnline = true;
    }

    //account not found with this facebook, tries creating an account instead
    private void OnPlayfabFacebookAuthFailed(PlayFabError error)
    {
        Debug.Log("PlayFab Facebook Auth Failed: " + error.GenerateErrorReport());
        var LoginWithFacebook = new LoginWithFacebookRequest { CreateAccount = true, AccessToken = AccessToken.CurrentAccessToken.TokenString };
        PlayFabClientAPI.LoginWithFacebook(LoginWithFacebook, OnCreatePlayfabFacebookAccountSuccess, OnCreatePlayfabFacebookAccountFailure);
        isOnline = false;
    }


    //Playfab account created with Facebook account
    private void OnCreatePlayfabFacebookAccountSuccess(PlayFab.ClientModels.LoginResult result)
    {
        Debug.Log("PlayFab Account created with Facebook. Session ticket: " + result.SessionTicket);
        mainLoginPanel.SetActive(false);
        usernameInputPanel.SetActive(true);
        FB.API("/me/picture?type=square&height=128&width=128", HttpMethod.GET, SetFBProfilePicToPlayfab);
        isOnline = true;
    }

    private void OnCreatePlayfabFacebookAccountFailure(PlayFabError error)
    {
        Debug.Log(error.GenerateErrorReport());
        mainLoginPanel.SetActive(true);
        isOnline = false;
    }

    
    //sets the image if logged in or account created with facebook
    private void SetFBProfilePicToPlayfab(IGraphResult result)
    {
        //string url;
        Image ProfilePic = LoggedInDialog.GetComponent<Image>();
        if (result.Texture != null)
        {
            ProfilePic.sprite = Sprite.Create(result.Texture, new Rect(0, 0, 128, 128), new Vector2());
           // url = result.ResultDictionary["url"].ToString();
           // var requestAvatarUpdate = new UpdateAvatarUrlRequest { ImageUrl = url };
           // PlayFabClientAPI.UpdateAvatarUrl(requestAvatarUpdate, OnAvatarUpdateSuccess, OnAvatarUpdateFailure);

        } else
        {
           //ProfilePic.sprite = null;
            ProfilePic.sprite = Sprite.Create(NoProfilePic, new Rect(0, 0, 128, 128), new Vector2());
        }

        //for storing in playfab
        //url = result.ResultDictionary["url"].ToString();
        //Debug.Log("Profile Pic URL: " + url);
    }
    /*
     * 
    private void OnAvatarUpdateSuccess(EmptyResponse result)
    {
        Debug.Log("Avatar Updated: Success");
    }

    private void OnAvatarUpdateFailure(PlayFabError error)
    {
        Debug.Log("Avatar was not updated: Failure");
    }

   
    private void DisplayFBUsername(IResult result)
    {
        Text userName = LoggedInDialog.GetComponent<Text>();
        if (result.Error == null)
        {
            userName.text = result.ResultDictionary["first_name"].ToString();
        } else
        {
            Debug.Log(result.Error);
        }
    }*/

    #endregion

    #region Helper Functions

    private static string ReturnMobileID()
    {
        string deviceID = SystemInfo.deviceUniqueIdentifier;
        return deviceID;
    }


    #region Link Device to PlayFab account
    private void LinkDevice()
    {

#if UNITY_ANDROID
        var requestAndroidLink = new LinkAndroidDeviceIDRequest { AndroidDeviceId = ReturnMobileID(), ForceLink = true };
        PlayFabClientAPI.LinkAndroidDeviceID(requestAndroidLink, OnAndroidLinkSuccess, OnAndroidLinkFailure);
#endif

#if UNITY_IOS
        var requestIOSLink = new LinkIOSDeviceIDRequest { DeviceId = ReturnMobileID(), ForceLink = true };
        PlayFabClientAPI.LinkIOSDeviceID(requestIOSLink, OnIOSLinkSuccess, OnIOSLinkFailure);
#endif
    }

    private void OnAndroidLinkSuccess(LinkAndroidDeviceIDResult result)
    {
        Debug.Log("New Android Device Successfully linked");
    }

    private void OnAndroidLinkFailure(PlayFabError error)
    {
        Debug.Log(error.GenerateErrorReport());
        // if (error.Error == PlayFabErrorCode.LinkedDeviceAlreadyClaimed)   
    }

    private void OnIOSLinkSuccess(LinkIOSDeviceIDResult result)
    {
        Debug.Log("New Android Device Successfully linked");
    }

    private void OnIOSLinkFailure(PlayFabError error)
    {
        Debug.Log(error.GenerateErrorReport());
        // if (error.Error == PlayFabErrorCode.LinkedDeviceAlreadyClaimed)   
    }
    #endregion


    //updates the display name
    public void UpdateDisplayName(string displayName)
    {
        var requestUpdateDisplayName = new UpdateUserTitleDisplayNameRequest() { DisplayName = displayName };
        PlayFabClientAPI.UpdateUserTitleDisplayName(requestUpdateDisplayName, OnUpdateDisplayNameSuccess, OnUpdateDisplayNameFailure);
    }

    private void OnUpdateDisplayNameSuccess(UpdateUserTitleDisplayNameResult result)
    {
        Debug.Log("Display Name Successfully updated: " + result.DisplayName);
        usernameInputPanel.SetActive(false);
        gameModePanel.SetActive(true);
        Text userName = LoggedInDialog.GetComponent<Text>();
        userName.text = result.DisplayName;
        LoggedInDialog.SetActive(true);
    }

    private void OnUpdateDisplayNameFailure(PlayFabError error)
    {
        Debug.Log(error.GenerateErrorReport());
    }



    private void GetPlayerCombinedInfo()
    {
        //link email to device if it isn't already linked
        var requestInfo = new GetPlayerCombinedInfoRequest()
        {
            //PlayFabId = result.PlayFabId,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams()
            {
                GetUserAccountInfo = true
            }
        };
        PlayFabClientAPI.GetPlayerCombinedInfo(requestInfo, OnInfoRequestSuccess, OnInfoRequestFailure);
    }

    private void OnInfoRequestSuccess(GetPlayerCombinedInfoResult result)
    {
        Text userName = LoggedInDialog.GetComponent<Text>();
        if (result.InfoResultPayload.AccountInfo.TitleInfo.DisplayName == "")
        {
            userName.text = "Guest";
        }
        else
        {
            userName.text = result.InfoResultPayload.AccountInfo.TitleInfo.DisplayName;
        }
    }

    private void OnInfoRequestFailure(PlayFabError error)
    {
        Debug.Log(error.GenerateErrorReport());
    }

    public bool IsConnectedToPlayfab()
    {
        return isOnline;
    }
    #endregion


}

