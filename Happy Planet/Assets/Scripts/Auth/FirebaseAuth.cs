
public class FirebaseAuth : IAuth {

    public void TryAuth(string id, string pw, CallbackMethod callback)
    {
        callback();
    }
    public string GetName()
    {
        return "Test";
    }

}