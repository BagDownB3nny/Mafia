using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MouseSenseSlider : MonoBehaviour
{
    [Header("References")]
    public Slider sensitivitySlider;
    public TMP_Text sensitivityValueText;

    [Header("Settings")]
    public static float mouseSensitivity = 2.00f; // Default value

    void Start()
    {
        // Load saved sensitivity (if any)
        if (PlayerPrefs.HasKey("MouseSensitivity"))
        {
            mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity");
            PlayerCamera.instance.mouseSensitivity = mouseSensitivity;
            sensitivitySlider.value = mouseSensitivity;
        }

        // Update UI text on start
        UpdateSensitivityText();

        // Add listener to the slider
        sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
    }

    void OnSensitivityChanged(float value)
    {
        mouseSensitivity = value;
        PlayerPrefs.SetFloat("MouseSensitivity", value);
        PlayerCamera.instance.mouseSensitivity = value; // Assuming you have a method to set mouse sensitivity in PlayerCamera
        UpdateSensitivityText();
    }

    void UpdateSensitivityText()
    {
        sensitivityValueText.text = mouseSensitivity.ToString("0.00");
    }
}