using EHTool.LangKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUILoading : MonoBehaviour {

    [SerializeField] EHText _state;
    [SerializeField] Text _progress;

    [SerializeField] float _progressRoutine = -.4f;
    public void LoadingOn(string state) {

        gameObject.SetActive(true);
        _state.SetText(state);
        StartCoroutine(_DotRoutine());

    }

    public void LoadingOff() {
        StopAllCoroutines();
        gameObject.SetActive(false);
    }

    IEnumerator _DotRoutine()
    {

        int dotcount = 0;
        _progress.text = " ";
        while (gameObject.activeSelf)
        {

            if (dotcount < 4)
            {
                _progress.text += ". ";
                dotcount++;
            }
            else
            {

                _progress.text = " ";
                dotcount = 0;
            }

            yield return new WaitForSeconds(_progressRoutine);
        }
    }
}
