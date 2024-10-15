using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Reference to the AudioSource component
    private AudioSource audioSource;

    // Dictionary to store audio clips with string keys
    public Dictionary<string, AudioClip> audioClips;

    private int volumeLevel = 50;

    // Public array to assign clips and names in the Inspector
    public AudioClipEntry[] clipEntries;

    // List of keys and clips for easy assignment in the Inspector
    [System.Serializable]
    public class AudioClipEntry
    {
        public string name;
        public AudioClip clip;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Get the AudioSource component attached to this GameObject
        audioSource = GetComponent<AudioSource>();

        audioSource.volume = volumeLevel / 100f;

        // Initialize the dictionary
        audioClips = new Dictionary<string, AudioClip>();

        // Populate the dictionary with the names and clips from the array
        foreach (var entry in clipEntries)
        {
            if (!audioClips.ContainsKey(entry.name))
            {
                audioClips.Add(entry.name, entry.clip);
            }
            else
            {
                Debug.LogWarning("Duplicate key " + entry.name + " found! Skipping.");
            }
        }
    }

    /// /// <summary>
    /// Plays the audio clip with the specified name.
    /// </summary>
    /// <param name="clipName">The name of the audio clip to play.</param>
    /// <param name="volume">The volume level of the audio clip (default is 1).</param>
    /// 
    public void PlayClip(string clipName, float volume = 1f)
    {
        // Check if the dictionary contains the specified key
        if (audioClips.ContainsKey(clipName))
        {
            // Set the audio clip to the one corresponding to the key
            audioSource.clip = audioClips[clipName];
            audioSource.Play();
        }
        else
        {
            Debug.LogError("Sound " + clipName + " not found!");
        }
    }

    /// <summary>
    /// Changes the volume level of the audio.
    /// </summary>
    /// <param name="newVolume">The new volume level to set. Out of 100, default is 50</param>
    public void changeVolume(float newVolume)
    {
        volumeLevel = (int)newVolume;
    }

    public void addSoundClip(string name, AudioClip clip)
    {
        if (!audioClips.ContainsKey(name))
        {
            audioClips.Add(name, clip);
        }
    }
}
