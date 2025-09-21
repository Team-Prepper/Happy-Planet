using UnityEngine;
using EasyH.Unity.SoundKit;

public class SoundEffect : EarnEffectBase {

    public override void EarnEffectOn(int money, int energy)
    {
        if (!TryGetComponent<AudioSource>(out var audio)) return;

        if (money == 0) return;

        if (money > 0)
        {
            audio.clip = SoundManager.Instance.GetSound("Earn");
        }
        else {
            audio.clip = SoundManager.Instance.GetSound("Loss");
        }

        audio.Play();

    }
}
