using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;

public enum Theme {
    Green, Yellow, White
}

public class GameManager : MonoSingleton<GameManager> {

    public int Money { get; private set; }

    public int Pollution { get; private set; } = 100;

    float _spendTime;

    public float SpendTime {
        
        get {
            if (_spendTime < 0)
                return 0;

            return _spendTime;
        }
    }

    public void TimeAdd(float spendTime) {
        _spendTime += spendTime;

    }

    public void AddMoney(int earn) {
        Money += earn;
    }

    public int GetDay() {
        return Mathf.FloorToInt(SpendTime);
    }

    public void AddPollution(int earn) {
        Pollution += earn;

        if (Pollution < 0)
            Pollution = 0;
        else if (Pollution > 100)
            Pollution = 100;
    }

}
