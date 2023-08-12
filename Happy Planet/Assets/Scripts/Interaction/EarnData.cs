using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EarnData : MonoBehaviour
{

    public void PopUp(int money, int exp) {
        StopAllCoroutines();

        gameObject.SetActive(true);

        StartCoroutine(Disapear());

    }

    IEnumerator Disapear() {

        yield return new WaitForSeconds(1f);

        gameObject.SetActive(false);
    }


}
