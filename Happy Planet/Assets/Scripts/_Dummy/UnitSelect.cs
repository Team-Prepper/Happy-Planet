using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelect : MonoBehaviour {

    Transform trCenter;
    Vector3 originPos;
    Unit selectedUnit;

    public void Set(Vector3 pos, Transform center, Unit selected) {
        originPos = pos;
        trCenter = center;
        selectedUnit = selected;

        PosSet(pos);
        selected.Selected(transform);

    }

    public void PosSet(Vector3 pos) {
        transform.position = pos;
        transform.LookAt(trCenter);
    }

    public void Reset()
    {
        PosSet(originPos);
    }

    public void Free() {
        selectedUnit.Free();
    }


}
