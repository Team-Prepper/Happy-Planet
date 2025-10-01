using System;
using UnityEngine;

namespace BKTools.Gaming.Dice
{
    public abstract class DiceBase : MonoBehaviour
    {

        public Action<float> SyncValue { get; set; }

        public abstract void Initial(int seed);
        public abstract void Roll(Action<int> callback);
        public abstract void Shot(float value, Action<int> callback = null);
        

    }

}