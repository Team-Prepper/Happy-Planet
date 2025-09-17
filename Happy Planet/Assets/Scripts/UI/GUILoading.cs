using EasyH.Tool.LangKit;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GUILoading : MonoBehaviour {

    [SerializeField] private EHText _state;
    [SerializeField] private Text _progress;

    [SerializeField] private float _progressRoutine = -.4f;
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
            yield return new WaitForSeconds(_progressRoutine);

            if (dotcount < 4)
            {
                _progress.text += ". ";
                dotcount++;

                continue;
            }
            _progress.text = " ";
            dotcount = 0;
        }
    }
}
