using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUILoading : MonoBehaviour {

    [SerializeField] Text _progress;
    [SerializeField] Text _state;

    [SerializeField] float _progressRoutine = -.4f;
    public void LoadingOn(string state) {

        _state.text = state;
        gameObject.SetActive(true);
        StartCoroutine(_DotRoutine());

    }

    public void LoadingOff() {
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
