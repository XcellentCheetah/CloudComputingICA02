using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuUIManager : MonoBehaviour
{
    [SerializeField] private GameObject signInPage, signUpPage, forgetPasswordPage;
    public void GoToSignUp()
    {
        signInPage.SetActive(false);
        signUpPage.SetActive(true);
        forgetPasswordPage.SetActive(false);
    }

    public void GoToSignIn()
    {
        signUpPage.SetActive(false);
        signInPage.SetActive(true);
        forgetPasswordPage.SetActive(false);
    }

    public void GoToForgetPassword()
    {
        signUpPage.SetActive(false);
        signInPage.SetActive(false);
        forgetPasswordPage.SetActive(true);
    }

}
