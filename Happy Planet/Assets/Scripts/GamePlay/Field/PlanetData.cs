using UnityEngine;


[CreateAssetMenu(fileName = "Data_Field_", menuName = "ScriptableObjects/FieldData", order = 1)]
public class PlanetData : ScriptableObject
{
    [SerializeField] GameObject _planetPrefab;
    [SerializeField] float _size = 3.5f;
    [SerializeField] float _speed = 1f;
    [SerializeField] FieldCameraSet.CameraSettingValue value = 
        new FieldCameraSet.CameraSettingValue(40, new Vector3(0, 0, 30));

    public float Size => _size;
    public float Speed => _speed;
    public FieldCameraSet.CameraSettingValue CameraSettingValue => value;

    public GameObject GetPrefab() => _planetPrefab;
}
