using UnityEngine;


[CreateAssetMenu(fileName = "Data_Field_", menuName = "ScriptableObjects/FieldData", order = 1)]
public class FieldData : ScriptableObject
{
    [SerializeField] GameObject _planetPrefab;
    [SerializeField] float _size;
    [SerializeField] float _speed;
    [SerializeField] FieldCameraSet.CameraSettingValue value;

    public float Size => _size;
    public float Speed => _speed;
    public FieldCameraSet.CameraSettingValue CameraSettingValue => value;


    public GameObject GetPlanetPrefab() => _planetPrefab;
}
