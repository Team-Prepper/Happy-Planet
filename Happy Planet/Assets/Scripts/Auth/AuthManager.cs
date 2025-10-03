using EasyH.Unity;

public class AuthManager : MonoSingleton<AuthManager>
{
    public IAuther Auth { get; set; }
    
    protected override void OnCreate()
    {
        Auth = GetAuther();
        Auth.Initialize();
    }
    
    private IAuther GetAuther()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        return new FirebaseAuther();
#else
        return gameObject.AddComponent<FirebaseAuthWebGL>();
#endif

    }

}
