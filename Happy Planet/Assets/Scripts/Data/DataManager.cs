using EHTool;
using System.Collections;
using UnityEngine;


public partial class DataManager : MonoSingleton<DataManager> {

    protected override void OnCreate()
    {

    }

    public void RoutineCallMethod(CallbackMethod method, float routine) {
        StartCoroutine(_RoutineDataSave(method, routine));
    }

    IEnumerator _RoutineDataSave(CallbackMethod method, float routine)
    {
        while (true) {
            yield return new WaitForSecondsRealtime(routine);
            method();
        }

    }

}