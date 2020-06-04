using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/*
 * This Scriptable object holds reference
 * list to audio clips of same type.
 * Example: Shooting audio clips.
 */
[CreateAssetMenu(fileName = "AudioData", menuName = "Data/AudioData", order = 1)]
public class AudioData : ScriptableObject
{
    [SerializeField] private List<AudioClip> audioClips = new List<AudioClip>();

    public AudioClip GetRandomClip()
    {
        return audioClips.ElementAt(Random.Range(0, audioClips.Count - 1));
    }
}