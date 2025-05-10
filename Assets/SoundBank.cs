using UnityEngine;

public class SoundBank : MonoBehaviour
{
    [System.Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;
    }

    public Sound[] sounds;
    public AudioSource source;

    public void PlaySound(string name)
    {
        var clip = GetClip(name);
        if (clip != null && source != null)
            source.PlayOneShot(clip);
    }

    public AudioClip GetClip(string name)
    {
        foreach (var sound in sounds)
        {
            if (sound.name == name)
                return sound.clip;
        }
        Debug.LogWarning($"Sound '{name}' not found in SoundBank.");
        return null;
    }
}
