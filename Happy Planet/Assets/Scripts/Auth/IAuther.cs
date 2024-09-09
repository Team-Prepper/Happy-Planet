using System.Runtime.InteropServices.WindowsRuntime;

public interface IAuther {
    public void Initialize();
    public void TrySignIn(string id, string pw, CallbackMethod callback, CallbackMethod<string> fallback);
    public void TrySignUp(string id, string pw, CallbackMethod callback, CallbackMethod<string> fallback);
    public void SignOut();
    public bool IsSignIn();

    public void EmailVerify(CallbackMethod callback, CallbackMethod<string> fallback);
    public bool IsEmailVerified();
    public void ReVerify(string pw, CallbackMethod callback, CallbackMethod<string> fallback);

    public void DisplayNameChange(string newName, CallbackMethod callback, CallbackMethod<string> fallback);
    public void PasswordChange(string newPassword, CallbackMethod callback, CallbackMethod<string> fallback);
    public void DeleteUser(CallbackMethod callback, CallbackMethod<string> fallback);

    public string GetUserId();
    public string GetName();
}

public class DefaultAuther : IAuther {

    public void TrySignIn(string id, string pw, CallbackMethod callback, CallbackMethod<string> fallback) {
        callback?.Invoke();
    }
    
    public void TrySignUp(string id, string pw, CallbackMethod callback, CallbackMethod<string> fallback)
    {
        callback?.Invoke();
    }

    public void SignOut() { 
        
    }

    public bool IsSignIn() => false;

    public void EmailVerify(CallbackMethod callback, CallbackMethod<string> fallback) {
        callback?.Invoke();
    }

    public bool IsEmailVerified() => false;

    public void ReVerify(string pw, CallbackMethod callback, CallbackMethod<string> fallback) {
        callback?.Invoke();
    }

    public string GetUserId() => "Test";

    public string GetName() => "Test";

    public void Initialize()
    {

    }

    public void DisplayNameChange(string newName, CallbackMethod callback, CallbackMethod<string> fallback)
    {
        callback?.Invoke();
    }

    public void PasswordChange(string newPassword, CallbackMethod callback, CallbackMethod<string> fallback)
    {
        callback?.Invoke();
    }

    public void DeleteUser(CallbackMethod callback, CallbackMethod<string> fallback)
    {
        callback?.Invoke();
    }

}