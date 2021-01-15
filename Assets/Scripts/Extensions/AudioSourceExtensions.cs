using UnityEngine;

public static class AudioSourceExtensions
{
    public static void Play(this AudioSource audioSource, AudioClip clip, bool isLoop = false)
    {
        audioSource.loop = isLoop;
        audioSource.clip = clip;
        audioSource.Play();
    }
}
