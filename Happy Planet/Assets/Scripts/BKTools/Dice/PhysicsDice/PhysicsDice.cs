using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

// 

namespace BKTools.Gaming.Dice
{

    [RequireComponent(typeof(Rigidbody))]
    public class PhysicsDice : DiceBase
    {

        [SerializeField] private PhysicsDiceValueBase _diceValue;

        public int Value { get; private set; }

        private Vector3 originPos = new Vector3(0, 3, 0);

        private bool _isRolling;

        [Space(20)]
        [SerializeField] float torqueMin = 100f;
        [SerializeField] float torqueMax = 200f;
        [SerializeField] private float throwStrength = 50;
        [SerializeField] private GameObject walls;
        [SerializeField] private float _waitTime = 1f;

        [SerializeField] private UnityEvent _diceSet;
        [SerializeField] private UnityEvent _diceStopEvent;

        private float idleTime = 0.1f;
        private Vector3 lastPosition;
        private PhysicsDiceRandom _rand
            = new PhysicsDiceRandom();

        private Rigidbody rb;

        // rotation 
        private float rotationSpeed = 0.5f;
        private Quaternion initialRotation;
        private Quaternion targetRotation;

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            _isRolling = false;
        }

        public override void Initial(int seed)
        {
            _rand.SetSeed(seed);

            Value = 0;
            _isRolling = false;
            rb.useGravity = false;
            rb.isKinematic = false;
            rb.angularVelocity = Vector3.zero;
            rb.linearVelocity = Vector3.zero;
            transform.localPosition = originPos;
        }

        public override void Roll(Action<int> callback)
        {
            _diceStopEvent.AddListener(() => callback(Value));
            RollDice();
        }

        public void RollDice()
        {
            SyncValue?.Invoke(0);
            Shot(0, null);
        }

        public override void Shot(float value, Action<int> callback)
        {
            if (_isRolling) return;

            _isRolling = true;
            walls.SetActive(true);
            transform.localPosition = originPos;
            lastPosition = rb.position;

            rb.useGravity = true;
            rb.linearVelocity = Vector3.up * throwStrength;

            rb.AddForce(_rand.InsideUnitSphere *
                Random.Range(50, 100), ForceMode.Impulse);
            rb.AddTorque(_rand.InsideUnitSphere *
                torqueMax + torqueMin * Vector3.one);

            StartCoroutine(CheckIdle());
        }

        public IEnumerator CheckIdle()
        {
            while (true)
            {
                yield return new WaitForSeconds(idleTime);
                if (transform.localPosition.y < -10)
                {
                    transform.localPosition = originPos;
                }

                if (rb.position == lastPosition)
                {
                    break;
                }
                lastPosition = rb.position;
            }

            //Debug.Log("is Stop");
            ChkRoll();
        }

        public void ChkRoll()
        {

            if (_diceValue.TryGetValue(out DiceValueInfor rollValue))
            {
                initialRotation = transform.rotation;
                targetRotation = Quaternion.Euler(initialRotation.eulerAngles.x, rollValue.isReverse ? 180 : 0, initialRotation.eulerAngles.z);

                Value = rollValue.value;

                StartCoroutine(RotateSmoothly(rollValue.value));

                return;

            }

            rb.AddForce(Vector3.one, ForceMode.Impulse);
            walls.SetActive(false);
            StartCoroutine(CheckIdle());
        }

        private IEnumerator RotateSmoothly(int rollValue)
        {

            float elapsedTime = 0f;
            Quaternion startRotation = transform.rotation;

            while (elapsedTime < rotationSpeed)
            {
                transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime / rotationSpeed);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.rotation = targetRotation;

            yield return new WaitForSeconds(_waitTime);

            _diceStopEvent.Invoke();
            _isRolling = false;
        }

    }
}