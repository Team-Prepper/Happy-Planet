using EasyH.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Theme {
    Green, Yellow, White
}

public class GameManager : MonoSingleton<GameManager> {


    public IField Field { get; set; }

    private IList<Action> _routineMethodList;

    protected override void OnCreate()
    {
        Field = new DefaultField();

        _routineMethodList = new List<Action>();

        AuthManager.Instance.Auth = GetAuther();
        AuthManager.Instance.Auth.Initialize();

    }
    
    private IAuther GetAuther()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        return new FirebaseAuther();
#else
        return gameObject.AddComponent<FirebaseAuthWebGL>();
#endif

    }

    public int AddRoutineMethod(Action method, float routine) {
        int retval = GetRoutineMethodId();

        _routineMethodList[retval] = method;
        StartCoroutine(_RoutineDataSave(retval, routine));

        return retval;
    }

    public void RemoveRoutineMethod(int id) {
        _routineMethodList[id] = null;
    }

    private int GetRoutineMethodId() {
        for (int i = 0; i < _routineMethodList.Count; i++) {
            if (_routineMethodList[i] == null) return i;
        }
        _routineMethodList.Add(null);
        return _routineMethodList.Count - 1;
    }

    IEnumerator _RoutineDataSave(int id, float routine)
    {
        while (true) {
            yield return new WaitForSecondsRealtime(routine);
            if (_routineMethodList[id] == null) break;
            _routineMethodList[id].Invoke();
        }

    }

}
