using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioData", menuName = "Data/AudioData", order = 1)]
public class AudioData : ScriptableObject
{
    [SerializeField] private List<AudioClip> audioClips = new List<AudioClip>();

    public AudioClip GetRandomClip()
    {
        return audioClips.ElementAt(Random.Range(0, audioClips.Count - 1));
    }
}