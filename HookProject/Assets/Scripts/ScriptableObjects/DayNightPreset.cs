using UnityEngine;

[CreateAssetMenu(menuName = "Presets/DayNight Preset")]
public class DayNightPreset : ScriptableObject
{
    public Gradient fogColor;
    public Gradient SkyboxColor;
    public Gradient lightingColor;
    public Gradient ambientLightColor;
}
