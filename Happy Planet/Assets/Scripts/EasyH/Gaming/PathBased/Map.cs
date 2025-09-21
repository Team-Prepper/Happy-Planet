using UnityEngine;

namespace EasyH.Gaming.PathBased
{
    public class Map : MonoBehaviour
    {

        [SerializeField] Plate _startPlate;

        public Plate GetStartPlate()
        {
            return _startPlate;
        }

    }
    
}