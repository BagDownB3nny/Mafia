using Dissonance.Audio.Playback;
using Mirror;
using UnityEngine;

public class CustomVoicePlayback : VoicePlayback
{
    private AudioLowPassFilter _lowPassFilter;
    protected override void Start()
    {
        base.Start();
        _lowPassFilter = GetComponent<AudioLowPassFilter>();
    }
    protected override void Update()
    {
        base.Update();
        ApplyOcclusion();
    }

    private void ApplyOcclusion()
    {
        Transform listenerTransform = _player.transform;
        if (listenerTransform == null || _lowPassFilter == null) return;

        Vector3 direction = listenerTransform.position - transform.position;
        float distance = direction.magnitude;

        // Check if a wall or obstacle is blocking the sound path
        if (Physics.Raycast(transform.position, direction, out RaycastHit hit, distance))
        {
            if (hit.collider.CompareTag("Wall"))  // Adjust tag as needed
            {
                _lowPassFilter.cutoffFrequency = 500f; // Muffled effect when occluded
            }
        }
        else
        {
            _lowPassFilter.cutoffFrequency = 22000f; // Normal speech when clear
        }
    } 
}
