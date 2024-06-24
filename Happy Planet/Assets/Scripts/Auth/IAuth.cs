public interface IAuth {
    public void TryAuth(string id, string pw, CallbackMethod callback);
    public string GetName();
}

public class DefaultAuth : IAuth {

    public void TryAuth(string id, string pw, CallbackMethod callback) {
        callback();
    }
    public string GetName() {
        return "Test";
    }
}