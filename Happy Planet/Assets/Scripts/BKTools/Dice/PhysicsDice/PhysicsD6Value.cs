using UnityEngine;
using System.Collections.Generic;

namespace BKTools.Gaming.Dice
{
    public class PhysicsD6Value : PhysicsDiceValueBase
    {

        private IDictionary<Vector3Int, DiceValueInfor> _diceValue
            = new Dictionary<Vector3Int, DiceValueInfor>
                {
                    { new Vector3Int( 0,  0,  1),
                        new DiceValueInfor(1, false) },
                    { new Vector3Int( 0,  1,  0),
                        new DiceValueInfor(2, false) },
                    { new Vector3Int(-1,  0,  0),
                        new DiceValueInfor(3, false) },
                    { new Vector3Int( 1,  0,  0),
                        new DiceValueInfor(4, false) },
                    { new Vector3Int( 0, -1,  0),
                        new DiceValueInfor(5, false) },
                    { new Vector3Int( 0,  0, -1),
                        new DiceValueInfor(6, false) },
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
                new Vector3Int(
                    Mathf.RoundToInt(Vector3.Dot(
                        transform.right, Vector3.up)),
                    Mathf.RoundToInt(Vector3.Dot(
                        transform.up, Vector3.up)),
                    Mathf.RoundToInt(Vector3.Dot(
                        transform.forward, Vector3.up))),
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