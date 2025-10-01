using System.Collections;
using System;
using UnityEngine;

namespace BKTools.Gaming.Dice
{
    public class RouletteDice : DiceBase
    {

        [SerializeField] private Vector2 _speedRange = Vector2.right;
        [SerializeField] private float _maxTime = 1f;
        [SerializeField] private Transform _rouletteTr;

        [SerializeField] private Transform[] _numTr;
        [SerializeField] private int _minValue;

        private System.Random _rand = new System.Random(-1);

        public override void Initial(int seed)
        {

            _rand = new System.Random(seed);
            _rouletteTr.eulerAngles = Vector3.forward * _rand.Next(-180, 180);

        }

        private int GetValue()
        {
            float maxValue = float.MinValue;
            int idx = -1;

            for (int i = 0; i < _numTr.Length; i++)
            {
                float value = Vector3.Dot(_numTr[i].up, Vector3.up);
                if (value > maxValue)
                {
                    maxValue = value;
                    idx = i;
                }
            }

            return idx + _minValue;
        }

        public override void Roll(Action<int> callback)
        {
            SyncValue?.Invoke(_rouletteTr.eulerAngles.z);
            Shot(_rouletteTr.eulerAngles.z, callback);
        }

        public override void Shot(float value, Action<int> callback = null)
        {

            _rouletteTr.eulerAngles = new Vector3(0, 0, value);

            StartCoroutine(Dice(callback));

        }

        private IEnumerator Dice(Action<int> callback)
        {
            float spendTime = 0;
            float speed = Mathf.Lerp(_speedRange.x, _speedRange.y, (float)_rand.NextDouble());

            while (spendTime < _maxTime)
            {
                _rouletteTr.Rotate(Vector3.forward * Mathf.Lerp(speed, 0, spendTime / _maxTime) * Time.deltaTime);
                spendTime += Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(.2f);

            callback?.Invoke(GetValue());
        }
    }
}