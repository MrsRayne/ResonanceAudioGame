using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.Characters.FirstPerson;


[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]

public class FPSFootstepMaterials : MonoBehaviour
{
    /// <summary>
    /// Footstep audio clips associated with specific materials and walk speeds
    /// </summary>
    public MaterialAudioClips[] materialAudioClipsWalk = default;

    /// <summary>
    /// Rustle audio clips associated with specific walk speeds
    /// </summary>
    public RustleAudioClips[] rustleAudioClips = default;

    /// <summary>
    /// The rigidbody of the FPS player
    /// </summary>
    Rigidbody rb;
    

    /// <summary>
    /// The rigidbody-based first person controller
    /// </summary>
    IMovementSpeed movementSpeed;


    public AudioClip GetFootstepAudioClip(Material groundMaterial)
    {
        FootstepSpeed speed = rb.velocity.magnitude > movementSpeed.WalkSpeed ? FootstepSpeed.Run : FootstepSpeed.Walk;

        AudioClip audioClip = default;
        foreach (var material in materialAudioClipsWalk)
        {
            if (groundMaterial == material.Material && speed == material.Speed)
            {
                audioClip = material.AudioClipRepetition.GetAudioClip();
                break;
            }
        }
        
        return audioClip;
    }

    public AudioClip GetRustleAudioClip()
    {
        FootstepSpeed speed = rb.velocity.magnitude > movementSpeed.WalkSpeed ? FootstepSpeed.Run : FootstepSpeed.Walk;

        AudioClip audioClip = default;
        foreach (var rustle in rustleAudioClips)
        {
            if (speed == rustle.Speed)
            {
                audioClip = rustle.AudioClipRepetition.GetAudioClip();
                break;
            }
        }

        return audioClip;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        movementSpeed = GetComponent<IMovementSpeed>();       
    }

    [Serializable]
    public class MaterialAudioClips
    {
        public Material Material;
        public FootstepSpeed Speed;
        public AudioClipRepetition AudioClipRepetition;
    }

    [Serializable]
    public class RustleAudioClips
    {
        public FootstepSpeed Speed;
        public AudioClipRepetition AudioClipRepetition;
    }

    public enum FootSide
    {
        Left,
        Right
    }

    public enum FootstepSpeed
    {
        Walk,
        Run
    }
}
