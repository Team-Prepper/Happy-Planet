using UnityEngine;
using System;
using System.Collections.Generic;

namespace BKTools.Gaming.Dice
{
    [Serializable]
    public class DiceValueInfor
    {
        public int value;
        public bool isReverse;

        public DiceValueInfor(int value, bool isReverse)
        {
            this.value = value;
            this.isReverse = isReverse;
        }
    }

    public abstract class PhysicsDiceValueBase : MonoBehaviour
    {
        public abstract bool TryGetValue(out DiceValueInfor value);
    }
}