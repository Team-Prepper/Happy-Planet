using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace BKTools.Gaming.Dice
{
    [RequireComponent(typeof(Rigidbody))]
    public class D6Dice : DiceBase
    {

        private Dictionary<Vector3Int, int> _diceInfor =
            new Dictionary<Vector3Int, int>
            {
                { new Vector3Int( 0,  0,  1), 5 },
                { new Vector3Int( 0,  1,  0), 4 },
                { new Vector3Int(-1,  0,  0), 1 },
                { new Vector3Int( 1,  0,  0), 6 },
                { new Vector3Int( 0, -1,  0), 3 },
                { new Vector3Int( 0,  0, -1), 2 },
            };

        public int Value { get; private set; }
        private bool _isRolling;

        private float torqueMin = 5f;
        private float torqueMax = 10f;
        private float throwStrength = 10f;
        private float _waitTime = 1f;

        [SerializeField] private UnityEvent _diceStopEvent;
        private System.Random _rand = new System.Random(-1);

        private Rigidbody rb;
        private Vector3 originPos = new Vector3(0, 3, 0);
        
        private Vector3 InsideUnitSphere
        {

            get
            {

                // 1. 랜덤 방향 생성 (θ: [0, 2π], ϕ: [0, π])
                double theta = 2 * Math.PI * _rand.NextDouble();
                double phi = Math.Acos(1 - 2 * _rand.NextDouble()); // 변환된 φ 범위 조정

                // 2. 단위 벡터 변환
                float x = (float)(Math.Cos(theta) * Math.Sin(phi));
                float y = (float)(Math.Sin(theta) * Math.Sin(phi));
                float z = (float)(Math.Cos(phi));

                // 3. 반지름 r^(1/3) 적용하여 균일 분포 유지
                float r = (float)Math.Pow(_rand.NextDouble(), 1.0 / 3.0);

                // 4. 최종 벡터 반환
                return new Vector3(x, y, z) * r;
            }

        }

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        public override void Initial(int seed)
        {
            _rand = new System.Random(seed);

            Value = 0;
            _isRolling = false;
            rb.useGravity = false;
            rb.isKinematic = false;
            rb.angularVelocity = Vector3.zero;
            rb.linearVelocity = Vector3.zero;
            transform.localPosition = originPos;
        }

        [ContextMenu("Execute Function")]
        private void MyFunction()
        {
            RollDice();
        }

        public override void Roll(Action<int> callback)
        {
            _diceStopEvent.AddListener(() => callback(Value));
            RollDice();
        }

        float RandomRange(float a, float b)
        {
            return Mathf.Lerp(a, b, (float)_rand.NextDouble());
        }

        public void RollDice()
        {
            SyncValue?.Invoke(0);
            Shot(0);
        }

        public override void Shot(float value, Action<int> callback = null)
        {

            if (_isRolling) return;

            _isRolling = true;
            transform.localPosition = originPos;

            rb.useGravity = true;
            rb.linearVelocity = Vector3.up * throwStrength;

            rb.AddForce(InsideUnitSphere * _rand.Next(5, 10),
                ForceMode.Impulse);
            rb.AddTorque(
                InsideUnitSphere * RandomRange(torqueMin, torqueMax),
                ForceMode.Impulse);

            StartCoroutine(CheckIdle());

        }

        private IEnumerator CheckIdle()
        {
            Vector3 lastPosition = rb.position;
            while (true)
            {
                yield return new WaitForSeconds(0.1f);
                if (Vector3.Distance(rb.position, lastPosition) < 0.01f)
                {
                    break;
                }
                lastPosition = rb.position;
            }

            CheckRollResult();
        }

        private void CheckRollResult()
        {
            Vector3Int roundedOrientation = new Vector3Int(
                Mathf.RoundToInt(Vector3.Dot(transform.right, Vector3.up)),
                Mathf.RoundToInt(Vector3.Dot(transform.up, Vector3.up)),
                Mathf.RoundToInt(Vector3.Dot(transform.forward, Vector3.up))
            );

            if (_diceInfor.TryGetValue(roundedOrientation, out int rollValue))
            {
                Value = rollValue;
                StartCoroutine(StopRolling());
            }
            else
            {
                RollDice(); // Roll again if orientation is invalid
            }
        }

        private IEnumerator StopRolling()
        {
            yield return new WaitForSeconds(_waitTime);
            _diceStopEvent.Invoke();
            _isRolling = false;
        }

    }
}