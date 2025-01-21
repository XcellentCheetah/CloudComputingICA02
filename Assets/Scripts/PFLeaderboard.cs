using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine.SceneManagement;

public class PFLeaderboard : MonoBehaviour
{
    [SerializeField] private TMP_Text leaderboardText;
    [SerializeField] private GameObject textHolder;
    [SerializeField] private GameObject individualTextHolder;
    [SerializeField] private bool showLeaderboard = false;

    void Start()
    {
        if (showLeaderboard) ShowLeaderboard();
    }

    public void ShowLeaderboard()
    {
        var lbreq = new GetLeaderboardRequest
        {
            StatisticName = "highestscore",
            StartPosition = 0,
            MaxResultsCount = 10
        };
        PlayFabClientAPI.GetLeaderboard(lbreq, OnLeaderboardGet, OnError);
    }

    void OnLeaderboardGet(GetLeaderboardResult r)
    {
        DeleteAllChildren(textHolder);
        foreach (var user in r.Leaderboard)
        {
            GameObject currentIndividualTextHolder = Instantiate(individualTextHolder, textHolder.transform) as GameObject;
            TMP_Text numberText = currentIndividualTextHolder.transform.GetChild(0).GetComponent<TMP_Text>();
            TMP_Text nameText = currentIndividualTextHolder.transform.GetChild(1).GetComponent<TMP_Text>();
            TMP_Text scoreText = currentIndividualTextHolder.transform.GetChild(2).GetComponent<TMP_Text>();
            numberText.text = (user.Position + 1).ToString();
            nameText.text = user.DisplayName;
            scoreText.text = user.StatValue.ToString();
        }
    }

    void OnNearbyLeaderboardGet(GetLeaderboardAroundPlayerResult r)
    {
        DeleteAllChildren(textHolder);
        foreach (var user in r.Leaderboard)
        {
            GameObject currentIndividualTextHolder = Instantiate(individualTextHolder, textHolder.transform) as GameObject;
            TMP_Text numberText = currentIndividualTextHolder.transform.GetChild(0).GetComponent<TMP_Text>();
            TMP_Text nameText = currentIndividualTextHolder.transform.GetChild(1).GetComponent<TMP_Text>();
            TMP_Text scoreText = currentIndividualTextHolder.transform.GetChild(2).GetComponent<TMP_Text>();
            numberText.text = (user.Position + 1).ToString();
            nameText.text = user.DisplayName;
            scoreText.text = user.StatValue.ToString();
        }
    }

    public void SendToLeaderboard(int score)
    {
        var req = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate()
                {
                    StatisticName = "highestscore",
                    Value = int.Parse(score.ToString())
                }
            }
        };
        Debug.Log("Submitting score:" + score);
        PlayFabClientAPI.UpdatePlayerStatistics(req, OnLeaderboardUpdate, OnError);
        
    }

    void OnLeaderboardUpdate(UpdatePlayerStatisticsResult r)
    {
        Debug.Log("Successful leaderboard sent: " + r.ToString());
    }

    void OnError(PlayFabError e) //report any errors here!
    {
        Debug.LogError("Error"+ e.GenerateErrorReport());
    }

    public void GetNearbyLeaderboard()
    {
        var request = new GetLeaderboardAroundPlayerRequest
        {
            StatisticName = "highestscore",
            MaxResultsCount = 10
        };

        PlayFabClientAPI.GetLeaderboardAroundPlayer(request, OnNearbyLeaderboardGet, OnError);
    }

    public void GoBack()
    {
        SceneManager.LoadScene("Menu");
    }

    public void DeleteAllChildren(GameObject parent)
    {
        if (parent == null)
        {
            Debug.LogWarning("Parent object is null. Cannot delete children.");
            return;
        }

        for (int i = parent.transform.childCount - 1; i >= 0; i--)
        {
            Transform child = parent.transform.GetChild(i);
            Destroy(child.gameObject);
        }

        Debug.Log("All children of " + parent.name + " have been deleted.");
    }
}
