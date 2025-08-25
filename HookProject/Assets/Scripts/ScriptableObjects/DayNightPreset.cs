using UnityEngine;

[CreateAssetMenu(menuName = "Presets/DayNight Preset")]
public class DayNightPreset : ScriptableObject
{
    public Gradient fogColor;
    public Gradient SkyboxColor;
    public Gradient sunColor;
    public Gradient moonColor;
    public Gradient ambientLightColor;
}
