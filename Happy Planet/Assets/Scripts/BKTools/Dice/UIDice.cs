using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace BKTools.Gaming.Dice
{
    public class UIDice : DiceBase
    {

        static int random = 1;

        [SerializeField] Vector2Int _range;
        [SerializeField] Text _num;

        public override void Initial(int seed)
        {
            _num.text = string.Format("{0}", random);
        }

        public override void Roll(Action<int> callback)
        {
            Shot(0, callback);
        }
        public override void Shot(float value, Action<int> callback)
        {
            StartCoroutine(Dice(callback));

        }

        IEnumerator Dice(Action<int> callback)
        {

            for (int i = 0; i < 3; i++)
            {
                yield return new WaitForSeconds(.2f);
                _num.text = string.Format("{0}", UnityEngine.Random.Range(_range.x, _range.y));
            }
            random = UnityEngine.Random.Range(_range.x, _range.y);

            _num.text = string.Format("{0}", random);

            yield return new WaitForSeconds(.2f);

            callback?.Invoke(random);
        }

    }
}