#if !UNITY_WEBGL || UNITY_EDITOR
using Firebase.Auth;
using Firebase.Extensions;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseAuther : IAuther {

    FirebaseAuth _auth;

    public void Initialize() {
        _auth = FirebaseAuth.DefaultInstance;

    }

    public void TrySignUp(string id, string pw, CallbackMethod callback, CallbackMethod<string> fallback)
    {
        _auth.CreateUserWithEmailAndPasswordAsync(id, pw).ContinueWithOnMainThread(task => {

            if (task.IsCanceled)
            {
                fallback("CreateUserWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                fallback("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }

            // Firebase user has been created.
            AuthResult result = task.Result;
            Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                result.User.DisplayName, result.User.UserId);

            callback();
        });
    }

    public void TrySignIn(string id, string pw, CallbackMethod callback, CallbackMethod<string> fallback)
    {
        _auth.SignInWithEmailAndPasswordAsync(id, pw).ContinueWithOnMainThread(task => {
            if (task.IsCanceled)
            {
                fallback("SignInWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                fallback("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }

            AuthResult result = task.Result;

            callback();
        });
    }

    public void SignOut() {
        _auth.SignOut();
    }

    public bool IsSignIn() => _auth.CurrentUser != null;

    public string GetName() {

        if (IsSignIn()) return _auth.CurrentUser.UserId;

        return "Test";
    }

}
#endif