using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    public AudioClip digClip;
    [SerializeField] [Range(0, 1)] float digVolume = 1f;
    public AudioClip eatClip;
    [SerializeField] [Range(0, 1)] float eatVolume = 1f;
    public AudioClip stepClip;
    [SerializeField] [Range(0, 1)] float stepVolume = 1f;
    public static AudioPlayer instance;
    void Awake()
    {
        ManageSingleton();
    }
    void ManageSingleton()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void PlayDigClip(Vector3 position)
    {
        playClip(digClip, digVolume, position);
    }
    public void PlayEatClip(Vector3 position)
    {
        playClip(eatClip, eatVolume, position);
    }
    public void PlayStepClip(Vector3 position)
    {
        playClip(stepClip, stepVolume, position);
    }
    void playClip(AudioClip clip, float volume, Vector3 position)
    {
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, position, volume);
        }
    }
}
