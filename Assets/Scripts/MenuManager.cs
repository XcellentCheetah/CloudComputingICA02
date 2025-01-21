using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private TMP_Text xp_text;
    [SerializeField] private PFDataMgr pfDataMgr;

    private void Start()
    {
        //xp_text.text = "XP: " + pfDataMgr.GetUserData("XP");

        pfDataMgr.GetUserData("XP", (userData) =>
        {
            if (userData != null) xp_text.text = "XP: " + userData;
            else xp_text.text = "XP: 0";
        });
    }
}
