using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mirror;

public class DayCycleManager : NetworkBehaviour
{
    [Header("Time settings")]
    private static Dictionary<string, Dictionary<string, float>> timeSettings = new Dictionary<string, Dictionary<string, float>>
    {
        {"8AmSettings", new Dictionary<string, float> {
            {"sunIntensity", 0.5f},
            {"moonIntensity", 0.05f},
            {"environmentLightingIntensity", 0.4f},
            {"environmentReflectionsIntensity", 0.5f},
            {"skyboxExposure", 0.3f},
            {"fogDensity", 0f}
        }},

        {"2PmSettings", new Dictionary<string, float> {
            {"sunIntensity", 1f},
            {"moonIntensity", 0.05f},
            {"environmentLightingIntensity", 1f},
            {"environmentReflectionsIntensity", 1f},
            {"skyboxExposure", 1.5f},
            {"fogDensity", 0f}
        }},

        {"6PmSettings", new Dictionary<string, float> {
            {"sunIntensity", 0.5f},
            {"moonIntensity", 0.05f},
            {"environmentLightingIntensity", 0.4f},
            {"environmentReflectionsIntensity", 0.5f},
            {"skyboxExposure", 0.3f},
            {"fogDensity", 0f}
        }},

        {"7PmSettings", new Dictionary<string, float> {
            {"sunIntensity", 0f},
            {"moonIntensity", 0.05f},
            {"environmentLightingIntensity", 0.4f},
            {"environmentReflectionsIntensity", 0.5f},
            {"skyboxExposure", 0.3f},
            {"fogDensity", 0f}
        }},

        {"12AmSettings", new Dictionary<string, float> {
            {"sunIntensity", 0f},
            {"moonIntensity", 0.05f},
            {"environmentLightingIntensity", 0.2f},
            {"environmentReflectionsIntensity", 0.5f},
            {"skyboxExposure", 0.1f},
            {"fogDensity", 0.01f}
        }},

        {"7AmSettings", new Dictionary<string, float> {
            {"sunIntensity", 0f},
            {"moonIntensity", 0.05f},
            {"environmentLightingIntensity", 0.2f},
            {"environmentReflectionsIntensity", 0.5f},
            {"skyboxExposure", 0.1f},
            {"fogDensity", 0.01f}
        }},
    };

    private static Dictionary<string, Dictionary<string, object>> transitionSettings = new Dictionary<string, Dictionary<string, object>>
    {
        {"7Am", new Dictionary<string, object> {
            {"hour", 7},
            {"nextTransition", "8Am"},
            {"settings", timeSettings["7AmSettings"]},
            {"durationToNextTransition", 1}
        }},
        {"8Am", new Dictionary<string, object> {
            {"hour", 8},
            {"nextTransition", "2Pm"},
            {"settings", timeSettings["8AmSettings"]},
            {"durationToNextTransition", 6}
        }},
        {"2Pm", new Dictionary<string, object> {
            {"hour", 14},
            {"nextTransition", "6Pm"},
            {"settings", timeSettings["2PmSettings"]},
            {"durationToNextTransition", 4}
        }},
        {"6Pm", new Dictionary<string, object> {
            {"hour", 18},
            {"nextTransition", "7Pm"},
            {"settings", timeSettings["6PmSettings"]},
            {"durationToNextTransition", 1}
        }},
        {"7Pm", new Dictionary<string, object> {
            {"hour", 19},
            {"nextTransition", "12Am"},
            {"settings", timeSettings["7PmSettings"]},
            {"durationToNextTransition", 5}
        }},
        {"12Am", new Dictionary<string, object> {
            {"hour", 0},
            {"nextTransition", "7Am"},
            {"settings", timeSettings["12AmSettings"]},
            {"durationToNextTransition", 7}
        }},
    };

    [Header("Internal params")]

    private Dictionary<string, float> previousSettings;
    private Dictionary<string, float> currentSettings;
    private Dictionary<string, float> targetSettings;
    private float currentSunRotation;
    private float rotationPerIrlSecond;

    public static DayCycleManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public override void OnStartClient()
    {
        InitialiseTransitions();
    }

    private void InitialiseTransitions()
    {
        for (int i = 0; i < transitionSettings.Count; i++)
        {
            KeyValuePair<string, Dictionary<string, object>> transition = transitionSettings.ElementAt(i);
            string key = transition.Key;
            Dictionary<string, object> value = transition.Value;

            int hour = (int)value["hour"];
            string nextTransition = (string)value["nextTransition"];
            Dictionary<string, float> settings = (Dictionary<string, float>)value["settings"];

            TimeManagerV2.instance.hourlyClientEvents[hour].AddListener(() =>
            {
                previousSettings = new Dictionary<string, float>(settings);
                currentSettings = new Dictionary<string, float>(settings);
                targetSettings = timeSettings[nextTransition + "Settings"];
                StartCoroutine(TransitionSky((int)value["durationToNextTransition"]));
            });
        }
        Debugger.instance.Log("DayCycleManager initialised transitions");
    }

    [Server]
    public void StartGame()
    {
        Debugger.instance.Log("DayCycleManager started game");
        int currentHour = TimeManagerV2.instance.currentHour;
        int currentMinute = TimeManagerV2.instance.currentMinute;
        int rotationPerGameHour = 360 / 24;
        // 7am = 0, 8am = 15, 6pm = 165, 7pm = 180
        currentSunRotation = rotationPerGameHour * (currentHour - 7) + (currentMinute / 60f) * rotationPerGameHour;
        rotationPerIrlSecond = rotationPerGameHour / TimeManagerV2.instance.irlSecondsPerGameHour;
    }

    [Client]
    private IEnumerator TransitionSky(float hoursToNextTransition)
    {
        float elapsed = 0f;
        float duration = hoursToNextTransition * TimeManagerV2.instance.irlSecondsPerGameHour;

        while (elapsed < duration)
        {

            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            List<string> keys = new List<string>(currentSettings.Keys);

            foreach (string key in keys)
            {
                currentSettings[key] = Mathf.Lerp(previousSettings[key], targetSettings[key], t);
            }

            ApplySkySettings();
            yield return null;
        }
    }

    private void UpdateSunRotation()
    {
        currentSunRotation += rotationPerIrlSecond * Time.deltaTime;
        Light sun = GameObject.Find("Sun").GetComponent<Light>();
        sun.transform.rotation = Quaternion.Euler(currentSunRotation, 0, 0);
        sun.intensity = currentSettings["sunIntensity"];
        // Debugger.instance.Log($"Sun rotation: {currentSunRotation}");
    }

    [Client]
    private void ApplySkySettings()
    {
        // Apply currentSettings to your skybox, sun, moon, etc.
        // Example:
        RenderSettings.skybox.SetFloat("_Exposure", currentSettings["skyboxExposure"]);
        RenderSettings.ambientIntensity = currentSettings["environmentLightingIntensity"];
        RenderSettings.reflectionIntensity = currentSettings["environmentReflectionsIntensity"];

        // Assuming you have references to your sun and moon lights
        // Light moon = GameObject.Find("Moon").GetComponent<Light>();

        UpdateSunRotation();
        // change fog density
        RenderSettings.fogDensity = currentSettings["fogDensity"];

        // moon.transform.rotation = Quaternion.Euler(currentSettings["moonRotation"], 0, 0);
        // moon.intensity = currentSettings["moonIntensity"];
    }

    private void LogCurrentSettings()
    {
        Debug.Log($"Sun intensity: {currentSettings["sunIntensity"]}");
        Debug.Log($"Moon intensity: {currentSettings["moonIntensity"]}");
        Debug.Log($"Environment lighting intensity: {currentSettings["environmentLightingIntensity"]}");
        Debug.Log($"Environment reflections intensity: {currentSettings["environmentReflectionsIntensity"]}");
        Debug.Log($"Skybox exposure: {currentSettings["skyboxExposure"]}");
    }
}
