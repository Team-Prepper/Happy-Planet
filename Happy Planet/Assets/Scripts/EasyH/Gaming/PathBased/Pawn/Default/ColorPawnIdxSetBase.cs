using UnityEngine;

namespace EasyH.Gaming.PathBased {

    public class ColorPawnIdxSet : PawnIdxSetBase
    {

        [SerializeField] private Color[] _pawnColor;

        [SerializeField] protected SpriteRenderer _sprite;

        public override void SetIdx(int idx) {
            _sprite.color = _pawnColor[idx];
        }
    }
}