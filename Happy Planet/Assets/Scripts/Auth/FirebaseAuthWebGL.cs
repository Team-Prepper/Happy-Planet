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
    public static extern void FirebaseAuthSignOut(string objectName, string callback);

    [DllImport("__Internal")]
    public static extern void FirebaseAuthSignIn(string id, string pw, string objectName, string callback, string fallback);
    [DllImport("__Internal")]
    public static extern void FirebaseAuthSignUp(string id, string pw, string objectName, string callback, string fallback);

    [DllImport("__Internal")]
    public static extern void FirebaseAuthReverify(string pw, string objectName, string callback, string fallback);

    [DllImport("__Internal")]
    public static extern void FirebaseAuthDelete(string objectName, string callback, string fallback);
    [DllImport("__Internal")]
    public static extern void FirebaseAuthUpdatePW(string pw, string objectName, string callback, string fallback);
    [DllImport("__Internal")]
    public static extern void FirebaseAuthUpdateName(string name, string objectName, string callback, string fallback);

}

public class FirebaseAuthWebGL : MonoBehaviour, IAuther {

    IDictionary<string, object> _currentUser;
    Dictionary<string, object> _user;

    CallbackMethod _nowCallback;
    CallbackMethod<string> _nowFallback;

    public void Callback() {
        _nowCallback?.Invoke();

        _nowCallback = null;
        _nowFallback = null;
    }
    public void Fallback(string msg)
    {
        _nowFallback?.Invoke(msg);

        _nowCallback = null;
        _nowFallback = null;
    }

    public string GetUserId()
    {
        if (_currentUser != null) {
            return _currentUser["uid"].ToString();
        }
        return "Test";
    }
    public string GetName()
    {
        if (_user != null)
        {
            return !_user.ContainsKey("displayName") ? string.Empty : (_user["displayName"] == null ? string.Empty : _user["displayName"].ToString());
        }
        return string.Empty;

    }

    void SetUserData(string json) {

        if (json == null) {
            _currentUser = null;
            _user = null;
            return;
        }

        _currentUser = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
        _user = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(_currentUser["providerData"].ToString())[0];

    }

    public void Initialize()
    {
        FirestoreWebGLBridge.FirestoreConnect("path", AssetOpener.ReadTextAsset("FirebaseConfig"));
        SetUserData(null);
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

        SetUserData(FirebaseAuthWebGLBridge.FirebaseAuthCurrentUser());
        
        return true;
    }

    public void SignOut()
    {
        FirebaseAuthWebGLBridge.FirebaseAuthSignOut(gameObject.name, "SignOutCallback");
    }

    public void SignOutCallback()
    {
        SetUserData(null);

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
        SetUserData(FirebaseAuthWebGLBridge.FirebaseAuthCurrentUser());
        _signInCallback?.Invoke();
    }

    public void SignInFallback(string msg) {
        _signInFallback?.Invoke(msg);
    }

    CallbackMethod _signUpCallback;
    CallbackMethod<string> _signUpFallback;

    public void TrySignUp(string id, string pw, CallbackMethod callback, CallbackMethod<string> fallback)
    {
        _nowCallback = () => {

            SetUserData(FirebaseAuthWebGLBridge.FirebaseAuthCurrentUser());

            callback?.Invoke();
        };

        _nowFallback = (string msg) =>
        {
            fallback?.Invoke(msg);
        };

        FirebaseAuthWebGLBridge.FirebaseAuthSignUp(id, pw, gameObject.name, "Callback", "Fallback");

    }

    public void DisplayNameChange(string newName, CallbackMethod callback, CallbackMethod<string> fallback)
    {
        if (!IsSignIn())
        {
            fallback?.Invoke("Not Logined");
            return;
        }
        _nowCallback = () => {
            
            SetUserData(FirebaseAuthWebGLBridge.FirebaseAuthCurrentUser());

            callback?.Invoke();
        };

        _nowFallback = (string msg) =>
        {
            fallback?.Invoke(msg);
        };

        FirebaseAuthWebGLBridge.FirebaseAuthUpdateName(newName, gameObject.name, "Callback", "Fallback");

    }

    public void PasswordChange(string newPassword, CallbackMethod callback, CallbackMethod<string> fallback)
    {
        if (!IsSignIn())
        {
            fallback?.Invoke("Not Logined");
            return;
        }

        _nowCallback = () => {
            callback?.Invoke();
        };

        _nowFallback = (string msg) =>
        {
            fallback?.Invoke(msg);
        };

        FirebaseAuthWebGLBridge.FirebaseAuthUpdatePW(newPassword, gameObject.name, "Callback", "Fallback");

    }

    public void DeleteUser(CallbackMethod callback, CallbackMethod<string> fallback)
    {
        if (!IsSignIn())
        {
            fallback?.Invoke("Not Logined");
            return;
        }

        _nowCallback = () => {
            SetUserData(null);
            callback?.Invoke();
        };

        _nowFallback = (string msg) =>
        {
            fallback?.Invoke(msg);
        };

        FirebaseAuthWebGLBridge.FirebaseAuthDelete(gameObject.name, "Callback", "Fallback");

    }

    public void EmailVerify(CallbackMethod callback, CallbackMethod<string> fallback)
    {
        fallback?.Invoke("���� ���� ����Դϴ�.");
    }

    public bool IsEmailVerified() {
        return _currentUser["verified"]?.ToString().CompareTo("false") == 0;
    }

    public void ReVerify(string pw, CallbackMethod callback, CallbackMethod<string> fallback)
    {
        if (!IsSignIn())
        {
            fallback?.Invoke("Not Logined");
            return;
        }

        _nowCallback = () => {
            callback?.Invoke();
        };

        _nowFallback = (string msg) =>
        {
            fallback?.Invoke(msg);
        };

        FirebaseAuthWebGLBridge.FirebaseAuthReverify(pw, gameObject.name, "Callback", "Fallback");
    }
}
