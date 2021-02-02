using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerFootstepAudio : MonoBehaviour
{
    /// <summary>
    /// Reference to set of footstep sounds associated with ground materials
    /// </summary>
    public FPSFootstepMaterials fpsFootstepMaterials;

    /// <summary>
    /// The layers we use for ground materials
    /// </summary>
    public LayerMask groundLayers = default;

    /// <summary>
    /// The ground material we are walking on
    /// </summary>
    Material currentGroundMaterial;

    /// <summary>
    /// The ground collider we are walking on. Used to avoid excessive GetComponent() calls
    /// </summary>
    Collider currentGroundCollider;

    /// <summary>
    /// The audio source used to play audio
    /// </summary>
    AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayFootstep()
    {
        if (audioSource)
        {
            if (fpsFootstepMaterials)
            {
                audioSource.PlayOneShot(fpsFootstepMaterials.GetFootstepAudioClip(currentGroundMaterial));
            }
            else
            {
                audioSource.Play();
            }
        }
    }

    public void PlayRustle() {
        if (audioSource && fpsFootstepMaterials)
        {
            audioSource.PlayOneShot(fpsFootstepMaterials.GetRustleAudioClip());
        }
    }

    private void FixedUpdate()
    {
        Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1f, groundLayers);
        if (hit.collider && hit.collider != currentGroundCollider)
        {
            currentGroundCollider = hit.collider;
            var meshRenderer = hit.collider.gameObject.GetComponent<MeshRenderer>();
            currentGroundMaterial = meshRenderer ? meshRenderer.sharedMaterial : null;
        }
    }
}
