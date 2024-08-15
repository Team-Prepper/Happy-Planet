using EHTool;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

static class FirebaseAuthWebGLBridge {

    [DllImport("__Internal")]
    public static extern bool IsConnect();

    [DllImport("__Internal")]
    public static extern string FirebaseAuthCurrentUser();

    [DllImport("__Internal")]
    public static extern void FirebaseAuthSignOut();

    [DllImport("__Internal")]
    public static extern void FirebaseAuthSignIn(string id, string pw, string objectName, string callback, string fallback);
    [DllImport("__Internal")]
    public static extern void FirebaseAuthSignUp(string id, string pw, string objectName, string callback, string fallback);

}

public class FirebaseAuthWebGL : MonoBehaviour, IAuther {

    IDictionary<string, object> _currentUser;

    public string GetName()
    {
        if (_currentUser != null) {
            return _currentUser["uid"].ToString();
        }
        return "Test";
    }

    public void Initialize()
    {
        FirestoreWebGLBridge.FirestoreConnect("path", AssetOpener.ReadTextAsset("FirebaseConfig"));
        _currentUser = null;
    }

    public bool IsSignIn()
    {
        if (_currentUser != null) {
            return true;
        }
        if (!FirebaseAuthWebGLBridge.IsConnect())
        {
            return false;
        }

        string json = FirebaseAuthWebGLBridge.FirebaseAuthCurrentUser();

        if (json == null) return false;

        _currentUser = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

        return true;
    }

    public void SignOut()
    {
        _currentUser = null;
        FirebaseAuthWebGLBridge.FirebaseAuthSignOut();
    }

    CallbackMethod _signInCallback;
    CallbackMethod<string> _signInFallback;

    public void TrySignIn(string id, string pw, CallbackMethod callback, CallbackMethod<string> fallback)
    {
        _signInCallback = callback;
        _signInFallback = fallback;
        FirebaseAuthWebGLBridge.FirebaseAuthSignIn(id, pw, gameObject.name, "SignInCallback", "SignInFallback");
    }

    public void SignInCallback(string json)
    {
        _currentUser = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
        _signInCallback();
    }

    public void SignInFallback(string msg) {
        _signInFallback(msg);
    }

    CallbackMethod _signUpCallback;
    CallbackMethod<string> _signUpFallback;

    public void TrySignUp(string id, string pw, CallbackMethod callback, CallbackMethod<string> fallback)
    {
        _signUpCallback = callback;
        _signUpFallback = fallback;

        FirebaseAuthWebGLBridge.FirebaseAuthSignUp(id, pw, gameObject.name, "SignUpCallback", "SignUpFallback");

    }
    public void SignUpCallback()
    {
        _signUpCallback();

    }

    public void SignUpFallback(string msg)
    {
        _signUpFallback(msg);

    }
}
