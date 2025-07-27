using UnityEngine;
public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip destroyClip;
    [SerializeField] private AudioClip ballHitClip;

    public void PlayBallHitSound()
    {
        audioSource.PlayOneShot(ballHitClip);
    }
    public void PlayDestroyBlockSound()
    {
        audioSource.PlayOneShot(destroyClip);
    }

    private void OnEnable()
    {
        GameEvents.OnBallHit += PlayBallHitSound;
        GameEvents.OnBlocsDestroyed += PlayDestroyBlockSound;
    }

    private void OnDisable()
    {
        GameEvents.OnBallHit -= PlayBallHitSound;
        GameEvents.OnBlocsDestroyed -= PlayDestroyBlockSound;
    }
}
