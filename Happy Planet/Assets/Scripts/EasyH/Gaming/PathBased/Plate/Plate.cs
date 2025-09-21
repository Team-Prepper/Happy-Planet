using System;
using UnityEngine;

namespace EasyH.Gaming.PathBased
{

    public class Plate : MonoBehaviour
    {

        [SerializeField] private NextPlateSelectorBase _nextPlateSelector;

        private Pawn _nowPawn;

        public void Arrive(Pawn pawn, Action callback = null)
        {
            SetPawn(pawn);
            callback?.Invoke();
            
        }

        public void SetPawn(Pawn target)
        {
            _nowPawn = target;
            _nowPawn.SetPlateAt(this);
        }

        public void Leave(Pawn target, Action<Plate> callback)
        {
            _nowPawn = null;
            callback?.Invoke(_nextPlateSelector.NextPlate(null));
        }

        public void NextPlate(Plate from, Action<Plate> callback)
        {
            callback?.Invoke(_nextPlateSelector.NextPlate(from));
        }

        public virtual int GetValue()
        {
            return _nextPlateSelector.GetValue();
        }

        public Pawn GetPawn() {
            return _nowPawn;
        }

    }
}