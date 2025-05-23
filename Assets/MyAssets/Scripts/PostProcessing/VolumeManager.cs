using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
public class VolumeManager : MonoBehaviour
{
    private ColorAdjustments colorAdjustments;

    [SerializeField] private Volume globalVolume;
    public static VolumeManager instance;

    public void Awake()
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

    void Start()
    {
        if (globalVolume.profile.TryGet<ColorAdjustments>(out colorAdjustments))
        {
            colorAdjustments.saturation.overrideState = true;
        }
    }

    public void SetSaturation(float saturationValue)
    {
        colorAdjustments.saturation.value = saturationValue;
    }
}
