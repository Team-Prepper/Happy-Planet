using UnityEngine;
using System;

namespace EasyH.Gaming.PathBased
{


    public abstract class PawnMoveBase : MonoBehaviour
    {

        public abstract void MoveTo(Plate startPos, Pawn target, int amount, Action arrive, Action<Plate> moveEnd);

        public abstract Plate Predict(Plate startPos, Pawn target, int amount);

        public abstract void DisposeTo(Vector3 pos);

    }
}