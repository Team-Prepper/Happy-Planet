using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EarnEffect : MonoBehaviour, IEffect
{
    [SerializeField] float _showTime = 0.3f;

    [SerializeField] Transform _inforTr;
    [SerializeField] Text _polution;
    [SerializeField] Text _money;

    public void SetEarnData(int polution, int money) { 
        _polution.text = (polution < 0 ? "" : "+") + polution.ToString();
        _money.text = (money < 0 ? "" : "+") + money.ToString();
    }

    public void EffectOn() {
        gameObject.SetActive(true);
        transform.LookAt(Camera.main.transform);
        _inforTr.up = Camera.main.transform.up;
        _inforTr.position = transform.position;
        StopAllCoroutines();
        StartCoroutine(_Effect());
    }

    IEnumerator _Effect() {
        float spendTime = 0;

        while (spendTime < _showTime)
        {
            _inforTr.Translate(Vector3.up * Time.deltaTime);
            transform.LookAt(Camera.main.transform);
            spendTime += Time.deltaTime;
            yield return null;
        }

        gameObject.SetActive(false);
    }
}
