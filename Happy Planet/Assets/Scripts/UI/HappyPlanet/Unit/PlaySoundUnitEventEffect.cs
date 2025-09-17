using UnityEngine;
using EasyH.Unity.SoundKit;

public class PlaySoundUnitEventEffect : UnitEventEffectBase
{
    [SerializeField] private string _soundKey;

    public override void EffectPlay() {
        SoundManager.Instance.PlaySound(_soundKey, "VFX");
        
    }
}
