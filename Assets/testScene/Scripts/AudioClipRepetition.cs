using UnityEngine;

[CreateAssetMenu(fileName ="AudioClipRepetition")]
public class AudioClipRepetition : ScriptableObject
{
    [SerializeField]
    private AudioClip[] clips = default;

    private AudioClip lastClip;

    public AudioClip GetAudioClip()
    {
        if (clips.Length > 1)
        {
            AudioClip randomClip;
            do
            {
                randomClip = clips[Random.Range(0, clips.Length)];
            } while (randomClip == lastClip);

            return lastClip = randomClip;
        }

        return clips[0];
    }
}
