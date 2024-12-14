using EHTool;
using UnityEngine;

public enum Theme {
    Green, Yellow, White
}

public class GameManager : MonoSingleton<GameManager> {

    public IAuther Auth { get; set; }

    public IField Field { get; set; }

    protected override void OnCreate()
    {
        Field = new DefaultField();
#if !UNITY_WEBGL || UNITY_EDITOR
        Auth = new FirebaseAuther();
#else
        Auth = gameObject.AddComponent<FirebaseAuthWebGL>();
#endif
        Auth.Initialize();
    }

}
