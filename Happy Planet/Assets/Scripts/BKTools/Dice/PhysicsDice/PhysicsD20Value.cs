using System.Collections.Generic;
using UnityEngine;

namespace BKTools.Gaming.Dice
{
    public class PhysicsD20Value : PhysicsDiceValueBase
    {
        private IDictionary<Vector3Int, DiceValueInfor> _diceValue
            = new Dictionary<Vector3Int, DiceValueInfor>
            {
                { new Vector3Int(-3, -9, -2),
                    new DiceValueInfor(1, false) },
                { new Vector3Int(8, 6, -2),
                new DiceValueInfor(2, false) },
                { new Vector3Int(-5, -4, 8),
                    new DiceValueInfor(3, false) },
                { new Vector3Int(-2, 6, -8),
                    new DiceValueInfor(4, true) },
                { new Vector3Int(5, -4, -8),
                    new DiceValueInfor(5, true) },
                { new Vector3Int(-8, 6, 2),
                    new DiceValueInfor(6, false) },
                { new Vector3Int(3, -9, 2),
                    new DiceValueInfor(7, false) },
                { new Vector3Int(2, 6, 8),
                    new DiceValueInfor(8, false) },
                { new Vector3Int(-10, 0, -2),
                    new DiceValueInfor(9, false) },
                { new Vector3Int(6, 0, 8),
                    new DiceValueInfor(10, false) },
                { new Vector3Int(-6, 0, -8),
                    new DiceValueInfor(11, true) },
                { new Vector3Int(10, 0, 2),
                    new DiceValueInfor(12, false) },
                { new Vector3Int(-2, -5, -8),
                    new DiceValueInfor(13, true) },
                { new Vector3Int(-3, 9, -2),
                    new DiceValueInfor(14, false) },
                { new Vector3Int(8, -6, -2),
                    new DiceValueInfor(15, false) },
                { new Vector3Int(-5, 4, 8),
                    new DiceValueInfor(16, false) },
                { new Vector3Int(2, -5, 8),
                    new DiceValueInfor(17, false) },
                { new Vector3Int(5, 4, -8),
                    new DiceValueInfor(18, true) },
                { new Vector3Int(-8, -6, 2),
                    new DiceValueInfor(19, false) },
                { new Vector3Int(3, 9, 1),
                    new DiceValueInfor(20, false) }
            };


        public override bool TryGetValue(out DiceValueInfor value)
        {
            float yDot = Mathf.Round(Vector3.Dot(
                transform.up.normalized, Vector3.up) * 10);
            float zDot = Mathf.Round(Vector3.Dot(
                transform.forward.normalized, Vector3.up) * 10);
            float xDot = Mathf.Round(Vector3.Dot(
                transform.right.normalized, Vector3.up) * 10);

            if (_diceValue.TryGetValue(
                new Vector3Int((int)xDot, (int)yDot, (int)zDot),
                out DiceValueInfor rollValue))
            {
                value = rollValue;
                return true;

            }

            value = null;
            return false;
        }
        
    }
}