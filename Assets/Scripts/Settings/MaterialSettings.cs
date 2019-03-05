using System.ComponentModel;
using General;
using UnityEngine;

namespace Gui
{
    public class MaterialSettings
    {
        private const float LightIntensityDefault = 10f;
        public float DisplacementAmplitude;
        public float DisplacementCenter;

        public float LightB;

        public float LightG;

        public float LightIntensity;

        public float LightR;

        public TrackableProperty Metallic;
        public float NormalStrength;
        [DefaultValue(0)] public float AoRemapMin;
        [DefaultValue(1)] public float AoRemapMax;
        [DefaultValue(0)] public float SmoothnessRemapMin;
        [DefaultValue(1)] public float SmoothnessRemapMax;
        [DefaultValue(1)] public float TexTilingX;
        [DefaultValue(1)] public float TexTilingY;
        [DefaultValue(0)] public float TexOffsetX;
        [DefaultValue(0)] public float TexOffsetY;

        public MaterialSettings()
        {
            var light = ProgramManager.Instance.MainLight;

            var color = light != null ? light.color : new Color(1.0f, 1.0f, 1.0f);

            LightR = color.r;
            LightG = color.g;
            LightB = color.b;
            LightIntensity = light != null ? light.intensity : LightIntensityDefault;

            TexTilingX = 1;
            TexTilingY = 1;

            TexOffsetX = 0;
            TexOffsetY = 0;
        }
    }
}