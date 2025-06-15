using Dissonance;
using UnityEngine;

public class AudioSettingsManager : MonoBehaviour
{
    public DissonanceComms dissonanceComms;

    // Update is called once per frame
    public void Start()
    {
        string microphoneName = PlayerPrefs.GetString("DefaultMic", "Default");
        dissonanceComms.MicrophoneName = microphoneName;
    }
}
