using UnityEngine;

[System.Serializable]
public class CameraSettingValue
{
    [SerializeField] public float view;
    [SerializeField] public Vector3 axis;

    public CameraSettingValue(float view, Vector3 axis)
    {
        this.view = view;
        this.axis = axis;
    }
}