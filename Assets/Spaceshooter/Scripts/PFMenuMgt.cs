using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine.SceneManagement;

public class PFMenuMgt : MonoBehaviour
{
    [SerializeField] private TMP_Text motdText;

    public void Start()
    {
        ClientGetTitleData();
    }

    public void OnLogOut()
    {
        PlayFabClientAPI.ForgetAllCredentials();
        SceneManager.LoadScene("RegLoginScene");
    }

    public void LaodScene(string name)
    {
        SceneManager.LoadScene(name);
    }

    public void ClientGetTitleData()
    {
        PlayFabClientAPI.GetTitleData(new GetTitleDataRequest(),
            result =>
            {
                if (result.Data == null || !result.Data.ContainsKey("MOTD")) motdText.text = "No MOTD";
                else motdText.text = "MOTD: " + result.Data["MOTD"];
            },
            error =>
            {
                 Debug.LogError("Error getting titleData: " + error.GenerateErrorReport());
            });
    }
}
