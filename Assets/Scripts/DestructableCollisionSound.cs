using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableCollisionSound : MonoBehaviour
{
    private AudioSource audioSource;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        // Audio source may be null during trajectory simulation
        if (!(audioSource is null))
        {
            float volume = 0.1f + collision.impulse.magnitude / 500;

            audioSource.pitch = Random.Range(0.8f, 1.0f);
            audioSource.volume = volume;
            audioSource.Play();
        }
    }

}
