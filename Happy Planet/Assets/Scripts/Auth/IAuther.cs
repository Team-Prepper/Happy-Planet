public interface IAuther {
    public void Initialize();
    public void TrySignIn(string id, string pw, CallbackMethod callback, CallbackMethod<string> fallback);
    public void TrySignUp(string id, string pw, CallbackMethod callback, CallbackMethod<string> fallback);
    public void SignOut();

    public bool IsSignIn();
    public string GetName();
}

public class DefaultAuther : IAuther {

    public void TrySignIn(string id, string pw, CallbackMethod callback, CallbackMethod<string> fallback) {
        callback();
    }
    
    public void TrySignUp(string id, string pw, CallbackMethod callback, CallbackMethod<string> fallback)
    {
        callback();
    }

    public void SignOut() { 
        
    }

    public bool IsSignIn() => false;

    public string GetName() {
        return "Test";
    }

    public void Initialize()
    {

    }
}