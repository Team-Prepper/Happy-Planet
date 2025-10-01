using System.Collections;
using UnityEngine;
using System;


namespace BKTools.Gaming.Dice
{
    public class DartDice : DiceBase
    {

        bool _beforeHit = false;

        [SerializeField] Transform _plateTr;
        [SerializeField] Transform _dartPoint;
        [SerializeField] float _rotateSpeed;
        [SerializeField] float _stopTime;

        [SerializeField] Transform[] _numTr;
        [SerializeField] int _minValue;

        private System.Random rand = new System.Random(-1);

        public override void Initial(int seed)
        {
            rand = new System.Random(seed);
            _plateTr.eulerAngles = Vector3.forward * rand.Next(-180, 180);

        }

        private void Update()
        {
            if (_beforeHit) return;

            _plateTr.Rotate(_rotateSpeed * Time.deltaTime * Vector3.forward);
        }

        public int GetValue()
        {
            float maxValue = float.MinValue;
            int idx = -1;

            for (int i = 0; i < _numTr.Length; i++)
            {
                float value = Vector3.Dot(_numTr[i].up, _dartPoint.up);
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
            SyncValue?.Invoke(_plateTr.eulerAngles.z);
            Shot(_plateTr.eulerAngles.z, callback);
        }

        public override void Shot(float value, Action<int> callback = null)
        {
            _plateTr.eulerAngles = new Vector3(0, 0, value);
            _beforeHit = true;
            _dartPoint.SetParent(_plateTr);

            StartCoroutine(Stop(callback));

        }

        IEnumerator Stop(Action<int> callback)
        {
            float spendTime = 0;

            while (spendTime < _stopTime)
            {
                yield return null;

                transform.Rotate(Mathf.Lerp(_rotateSpeed, 0, spendTime / _stopTime) * Time.deltaTime * Vector3.forward);
                spendTime += Time.deltaTime;

            }

            yield return new WaitForSeconds(1f);

            callback?.Invoke(GetValue());
        }


    }
}