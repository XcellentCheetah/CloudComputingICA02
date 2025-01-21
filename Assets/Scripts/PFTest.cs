using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public class PFTest : MonoBehaviour
{
    public void Start()
    {
        var request = new LoginWithCustomIDRequest { CustomId = "GettingStartedGuide", CreateAccount = true };
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
    }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Congratulations, you made your first successful API call!");
    }

    private void OnLoginFailure(PlayFabError error)
    {
        Debug.LogWarning("Something went wrong with your first API call.");
        Debug.LogError("Here's some debug information: ");
        Debug.LogError(error.GenerateErrorReport());
    }
}
