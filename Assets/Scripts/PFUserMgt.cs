using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.MultiplayerModels;
using TMPro;

public class PFUserMgt : MonoBehaviour
{
    [SerializeField] private TMP_InputField userEmail, userPassword, userName, currentScore, displayName;
    [SerializeField] private TMP_InputField userLoginEmailUsername, userLoginPassword;
    [SerializeField] private TMP_InputField userRecoveryEmail;
    [SerializeField] private TextMeshProUGUI Msg;
    [SerializeField] private string sceneToLoad;

    void UpdateMsg(string msg, Color color) //to display in console and messagebox
    {
        Debug.Log(msg);
        Msg.color = color;
        Msg.text = msg;
    }

    void OnError(PlayFabError e) //report any errors here!
    {
        UpdateMsg("Error" + e.GenerateErrorReport(), Color.red);
    }

    public void OnButtonRegUser() //for button click
    {
        var registerRequest = new RegisterPlayFabUserRequest
        {
            Email = userEmail.text,
            Password = userPassword.text,
            Username = userName.text
        };
        PlayFabClientAPI.RegisterPlayFabUser(registerRequest, OnRegSuccess, OnError);
    }

    void OnRegSuccess(RegisterPlayFabUserResult r)
    {
        UpdateMsg("Registration success!", Color.green);

        //To create a player display name
        var req = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = displayName.text
        };
        //update to profile
        PlayFabClientAPI.UpdateUserTitleDisplayName(req, OnDisplayNameUpdate, OnError);
    }

    void OnDisplayNameUpdate(UpdateUserTitleDisplayNameResult r)
    {
        UpdateMsg("display name updated!" + r.DisplayName, Color.green);
    }


    void OnLoginSuccess(LoginResult r)
    {
        UpdateMsg("Login Success!" + r.PlayFabId + r.InfoResultPayload.PlayerProfile.DisplayName, Color.green);
        SceneManager.LoadScene(sceneToLoad);
    }

    void OnGuestLoginSuccess(LoginResult r)
    {
        UpdateMsg("Login Success!" + r.PlayFabId, Color.green);
        SceneManager.LoadScene(sceneToLoad);
    }

    public void OnButtonLoginEmail() //login using email + password
    {
        var loginRequest = new LoginWithEmailAddressRequest
        {
            Email = userLoginEmailUsername.text,
            Password = userLoginPassword.text,
            //to get player profile, to get displayname
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
            {
                GetPlayerProfile = true
            }
        };
        PlayFabClientAPI.LoginWithEmailAddress(loginRequest, OnLoginSuccess, OnError);
    }

    public void OnButtonLoginUserName() // login using username + password
    {
        var loginRequest = new LoginWithPlayFabRequest
        {
            Username = userLoginEmailUsername.text,
            Password = userLoginPassword.text,
            //to get player profile, including displayname
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
            {
                GetPlayerProfile = true
            }
        };
        PlayFabClientAPI.LoginWithPlayFab(loginRequest, OnLoginSuccess, OnError);

    }

    public void OnResetPassword()
    {
        var ResetPassReq = new SendAccountRecoveryEmailRequest
        {
            Email = userRecoveryEmail.text,
            TitleId = PlayFabSettings.TitleId
        };
        PlayFabClientAPI.SendAccountRecoveryEmail(ResetPassReq, OnResetPassSucc, OnError);
    }

    void OnResetPassSucc(SendAccountRecoveryEmailResult r)
    {
        UpdateMsg("Recovery Email Sent! Check your mail.", Color.green);
    }


    public void GuestLogin()
    {
        var loginRequest = new LoginWithCustomIDRequest()
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            TitleId = PlayFabSettings.TitleId,
            CreateAccount = true
        };
        PlayFabClientAPI.LoginWithCustomID(loginRequest, OnGuestLoginSuccess, OnError);
    }


    public void OnLogOut()
    {
        PlayFabClientAPI.ForgetAllCredentials();
        SceneManager.LoadScene("RegLoginScene");
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
