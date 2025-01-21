using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;

public class PFDataMgr : MonoBehaviour
{
    [SerializeField] private TMP_Text xp_text;

    public void SetUserData(string dataName, string data)
    {
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>()
            {
                {dataName, data}
            }
        },
        result => Debug.Log("Successfully updated user data"),
        error =>
        {
            Debug.Log("Got error setting user data XP" + error.GenerateErrorReport());
        });
    }

    public void GetUserData(string dataName, Action<string> callback)
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(),
            result =>
            {
                Debug.Log("Got user data:");
                if (result.Data == null || !result.Data.ContainsKey(dataName))
                {
                    Debug.Log("No " + dataName);
                    callback?.Invoke(null);
                }
                else
                {
                    Debug.Log(dataName + ": " + result.Data[dataName].Value);
                    callback?.Invoke(result.Data[dataName].Value);
                }
            },
            error =>
            {
                Debug.Log("Got error retrieving user data: ");
                Debug.Log(error.GenerateErrorReport());
                callback?.Invoke(null);
            });
    }
}
