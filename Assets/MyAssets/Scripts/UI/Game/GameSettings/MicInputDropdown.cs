using System.Collections.Generic;
using Dissonance;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MicrophoneSettings : MonoBehaviour
{
    [SerializeField] private DissonanceComms dissonanceComms;
    [SerializeField] private TMP_Dropdown microphoneDropdown;

    private void Start()
    {
        // Populate dropdown on startup
        RefreshMicrophoneList();

        // Load saved microphone (if any)
        string savedMic = PlayerPrefs.GetString("DefaultMic");
        if (!string.IsNullOrEmpty(savedMic))
            SetMicrophone(savedMic);
    }

    // Fetch available microphones
    public void RefreshMicrophoneList()
    {
        microphoneDropdown.ClearOptions();

        // Get all microphones
        List<string> devices = new List<string>();
        dissonanceComms.GetMicrophoneDevices(devices);
        microphoneDropdown.AddOptions(devices);

        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
        foreach (string device in devices)
        {
            options.Add(new Dropdown.OptionData(device)); // Use microphone name as label
        }

        // Select current microphone
        var currentIndex = devices.FindIndex(d => d == dissonanceComms.MicrophoneName);
        microphoneDropdown.SetValueWithoutNotify(currentIndex);
    }

    // Called when the dropdown value changes
    public void OnDropdownValueChanged(int index)
    {
        string selectedMic = microphoneDropdown.options[index].text;
        SetMicrophone(selectedMic);
    }

    private void SetMicrophone(string deviceName)
    {
        try
        {
            // Update Dissonance microphone
            dissonanceComms.MicrophoneName = deviceName;
            // Save to PlayerPrefs
            PlayerPrefs.SetString("DefaultMic", deviceName);
            PlayerPrefs.Save();
        }
        catch
        {
            Debug.LogError("Failed to set microphone: " + deviceName);
        }
    }
}