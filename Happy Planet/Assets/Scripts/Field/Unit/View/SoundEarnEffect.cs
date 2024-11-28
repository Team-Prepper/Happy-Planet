using UnityEngine;
using UnityEngine.UI;

public class SoundEffect : IEarnEffect {

    public override void EarnEffectOn(int energy, int money)
    {
        if (!TryGetComponent<AudioSource>(out var audio)) return;

        audio.clip = SoundManager.Instance.GetSound("Earn");
        audio.Play();

    }
}
