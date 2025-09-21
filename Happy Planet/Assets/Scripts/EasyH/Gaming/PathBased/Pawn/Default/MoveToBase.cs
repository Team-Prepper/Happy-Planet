using System;
using UnityEngine;

namespace EasyH.Gaming.PathBased
{

    public abstract class MoveToBase : MonoBehaviour
    {
        public abstract void MoveTo(Vector3 pos, Action callback);
    }
}