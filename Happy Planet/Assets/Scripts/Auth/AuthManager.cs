using EasyH.Unity;

public class AuthManager : MonoSingleton<AuthManager>
{
    public IAuther Auth { get; set; }
    
    protected override void OnCreate()
    {
        
    }

}
