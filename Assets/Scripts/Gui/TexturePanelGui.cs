#region

using System.Collections;
using General;
using Settings;
using UnityEngine;

#endregion

namespace Gui
{
    public abstract class TexturePanelGui : MonoBehaviour, IHideable
    {
        protected Vector2Int ImageSize = new Vector2Int(1024, 1024);
        protected bool IsNewTexture;
        protected bool IsReadyToProcess;
        protected bool StuffToBeDone;
        protected Rect WindowRect;
        protected float GuiScale = 1.0f;
        protected int WindowId;
        protected TexturePanelSettings Settings;

        public GameObject TestObject;

        public Material ThisMaterial;

        public bool Active
        {
            get => gameObject.activeSelf;
            set => gameObject.SetActive(value);
        }

        public bool Hide { get; set; }

        public void DoStuff()
        {
            StuffToBeDone = true;
        }

        public void NewTexture()
        {
            IsNewTexture = true;
        }

        public void Close()
        {
            CleanupTextures();
            gameObject.SetActive(false);
        }

        protected abstract void CleanupTextures();

        protected void PostAwake()
        {
            var windowRectSize = WindowRect.size;
            windowRectSize.y += 40;
            WindowRect.size = windowRectSize;
            GuiScale -= 0.1f;
            WindowId = ProgramManager.Instance.GetWindowId;
        }

        public IEnumerator StartProcessing()
        {
            while (!ProgramManager.Lock()) yield return null;

            while (!IsReadyToProcess) yield return null;

            StartCoroutine(Process());

            yield return new WaitForSeconds(0.1f);

            MessagePanel.HideMessage();

            ProgramManager.Unlock();
        }

        protected abstract IEnumerator Process();

        protected void OnDisable()
        {
            CleanupTextures();
            IsReadyToProcess = false;
        }

        protected void DrawGuiExtras(int offsetX, int offsetY)
        {
            offsetY += 10;
            if (GUI.Button(new Rect(offsetX + 10, offsetY, 260, 25), "Reset to Defaults"))
            {
                ResetSettings();
            }
        }

        protected abstract void ResetSettings();

        #region TextureIDs

        protected const float BlurScale = 1.0f;
        protected static readonly int BlurScaleId = Shader.PropertyToID("_BlurScale");
        protected static readonly int ImageSizeId = Shader.PropertyToID("_ImageSize");
        protected static readonly int Isolate = Shader.PropertyToID("_Isolate");
        protected static readonly int Blur0Weight = Shader.PropertyToID("_Blur0Weight");
        protected static readonly int Blur1Weight = Shader.PropertyToID("_Blur1Weight");
        protected static readonly int Blur2Weight = Shader.PropertyToID("_Blur2Weight");
        protected static readonly int Blur3Weight = Shader.PropertyToID("_Blur3Weight");
        protected static readonly int Blur4Weight = Shader.PropertyToID("_Blur4Weight");
        protected static readonly int Blur5Weight = Shader.PropertyToID("_Blur5Weight");
        protected static readonly int Blur6Weight = Shader.PropertyToID("_Blur6Weight");
        protected static readonly int Blur0Contrast = Shader.PropertyToID("_Blur0Contrast");
        protected static readonly int Blur1Contrast = Shader.PropertyToID("_Blur1Contrast");
        protected static readonly int Blur2Contrast = Shader.PropertyToID("_Blur2Contrast");
        protected static readonly int Blur3Contrast = Shader.PropertyToID("_Blur3Contrast");
        protected static readonly int Blur4Contrast = Shader.PropertyToID("_Blur4Contrast");
        protected static readonly int Blur5Contrast = Shader.PropertyToID("_Blur5Contrast");
        protected static readonly int Blur6Contrast = Shader.PropertyToID("_Blur6Contrast");
        protected static readonly int FinalGain = Shader.PropertyToID("_FinalGain");
        protected static readonly int FinalContrast = Shader.PropertyToID("_FinalContrast");
        protected static readonly int FinalBias = Shader.PropertyToID("_FinalBias");
        protected static readonly int Slider = Shader.PropertyToID("_Slider");
        protected static readonly int BlurTex0 = Shader.PropertyToID("_BlurTex0");
        protected static readonly int HeightFromNormal = Shader.PropertyToID("_HeightFromNormal");
        protected static readonly int BlurTex1 = Shader.PropertyToID("_BlurTex1");
        protected static readonly int BlurTex2 = Shader.PropertyToID("_BlurTex2");
        protected static readonly int BlurTex3 = Shader.PropertyToID("_BlurTex3");
        protected static readonly int BlurTex4 = Shader.PropertyToID("_BlurTex4");
        protected static readonly int BlurTex5 = Shader.PropertyToID("_BlurTex5");
        protected static readonly int BlurTex6 = Shader.PropertyToID("_BlurTex6");
        protected static readonly int AvgTex = Shader.PropertyToID("_AvgTex");
        protected static readonly int Spread = Shader.PropertyToID("_Spread");
        protected static readonly int SpreadBoost = Shader.PropertyToID("_SpreadBoost");
        protected static readonly int Samples = Shader.PropertyToID("_Samples");
        protected static readonly int MainTex = Shader.PropertyToID("_MainTex");
        protected static readonly int BlendTex = Shader.PropertyToID("_BlendTex");
        protected static readonly int IsNormal = Shader.PropertyToID("_IsNormal");
        protected static readonly int BlendAmount = Shader.PropertyToID("_BlendAmount");
        protected static readonly int Progress = Shader.PropertyToID("_Progress");
        protected static readonly int IsolateSample1 = Shader.PropertyToID("_IsolateSample1");
        protected static readonly int UseSample1 = Shader.PropertyToID("_UseSample1");
        protected static readonly int SampleColor1 = Shader.PropertyToID("_SampleColor1");
        protected static readonly int SampleUv1 = Shader.PropertyToID("_SampleUV1");
        protected static readonly int HueWeight1 = Shader.PropertyToID("_HueWeight1");
        protected static readonly int SatWeight1 = Shader.PropertyToID("_SatWeight1");
        protected static readonly int LumWeight1 = Shader.PropertyToID("_LumWeight1");
        protected static readonly int MaskLow1 = Shader.PropertyToID("_MaskLow1");
        protected static readonly int MaskHigh1 = Shader.PropertyToID("_MaskHigh1");
        protected static readonly int Sample1Height = Shader.PropertyToID("_Sample1Height");
        protected static readonly int IsolateSample2 = Shader.PropertyToID("_IsolateSample2");
        protected static readonly int UseSample2 = Shader.PropertyToID("_UseSample2");
        protected static readonly int SampleColor2 = Shader.PropertyToID("_SampleColor2");
        protected static readonly int SampleUv2 = Shader.PropertyToID("_SampleUV2");
        protected static readonly int HueWeight2 = Shader.PropertyToID("_HueWeight2");
        protected static readonly int SatWeight2 = Shader.PropertyToID("_SatWeight2");
        protected static readonly int LumWeight2 = Shader.PropertyToID("_LumWeight2");
        protected static readonly int MaskLow2 = Shader.PropertyToID("_MaskLow2");
        protected static readonly int MaskHigh2 = Shader.PropertyToID("_MaskHigh2");
        protected static readonly int Sample2Height = Shader.PropertyToID("_Sample2Height");
        protected static readonly int SampleBlend = Shader.PropertyToID("_SampleBlend");
        protected static readonly int BlurContrast = Shader.PropertyToID("_BlurContrast");
        protected static readonly int BlurSamples = Shader.PropertyToID("_BlurSamples");
        protected static readonly int BlurSpread = Shader.PropertyToID("_BlurSpread");
        protected static readonly int BlurDirection = Shader.PropertyToID("_BlurDirection");
        protected static readonly int AoBlend = Shader.PropertyToID("_AOBlend");
        protected static readonly int ImageInput = Shader.PropertyToID("ImageInput");
        protected static readonly int HeightTex = Shader.PropertyToID("_HeightTex");
        protected static readonly int Depth = Shader.PropertyToID("_Depth");
        protected static readonly int NormalTex = Shader.PropertyToID("_NormalTex");
        protected static readonly int LightMaskPow = Shader.PropertyToID("_LightMaskPow");
        protected static readonly int LightPow = Shader.PropertyToID("_LightPow");
        protected static readonly int DarkMaskPow = Shader.PropertyToID("_DarkMaskPow");
        protected static readonly int DarkPow = Shader.PropertyToID("_DarkPow");
        protected static readonly int HotSpot = Shader.PropertyToID("_HotSpot");
        protected static readonly int DarkSpot = Shader.PropertyToID("_DarkSpot");
        protected static readonly int ColorLerp = Shader.PropertyToID("_ColorLerp");
        protected static readonly int Saturation = Shader.PropertyToID("_Saturation");
        protected static readonly int BlurTex = Shader.PropertyToID("_BlurTex");
        protected static readonly int MetalColor = Shader.PropertyToID("_MetalColor");
        protected static readonly int SampleUv = Shader.PropertyToID("_SampleUV");
        protected static readonly int HueWeight = Shader.PropertyToID("_HueWeight");
        protected static readonly int SatWeight = Shader.PropertyToID("_SatWeight");
        protected static readonly int LumWeight = Shader.PropertyToID("_LumWeight");
        protected static readonly int MaskLow = Shader.PropertyToID("_MaskLow");
        protected static readonly int MaskHigh = Shader.PropertyToID("_MaskHigh");
        protected static readonly int BlurOverlay = Shader.PropertyToID("_BlurOverlay");
        protected static readonly int OverlayBlurTex = Shader.PropertyToID("_OverlayBlurTex");
        protected static readonly int Angularity = Shader.PropertyToID("_Angularity");
        protected static readonly int AngularIntensity = Shader.PropertyToID("_AngularIntensity");
        protected static readonly int LightDir = Shader.PropertyToID("_LightDir");
        protected static readonly int HeightBlurTex = Shader.PropertyToID("_HeightBlurTex");
        protected static readonly int Desaturate = Shader.PropertyToID("_Desaturate");
        protected static readonly int LightTex = Shader.PropertyToID("_LightTex");
        protected static readonly int LightBlurTex = Shader.PropertyToID("_LightBlurTex");
        protected static readonly int LightRotation = Shader.PropertyToID("_LightRotation");
        protected static readonly int ShapeRecognition = Shader.PropertyToID("_ShapeRecognition");
        protected static readonly int ShapeBias = Shader.PropertyToID("_ShapeBias");
        protected static readonly int DiffuseTex = Shader.PropertyToID("_DiffuseTex");
        protected static readonly int MetalSmoothness = Shader.PropertyToID("_MetalSmoothness");
        protected static readonly int Sample1Smoothness = Shader.PropertyToID("_Sample1Smoothness");
        protected static readonly int Sample2Smoothness = Shader.PropertyToID("_Sample2Smoothness");
        protected static readonly int IsolateSample3 = Shader.PropertyToID("_IsolateSample3");
        protected static readonly int UseSample3 = Shader.PropertyToID("_UseSample3");
        protected static readonly int SampleColor3 = Shader.PropertyToID("_SampleColor3");
        protected static readonly int SampleUv3 = Shader.PropertyToID("_SampleUV3");
        protected static readonly int HueWeight3 = Shader.PropertyToID("_HueWeight3");
        protected static readonly int SatWeight3 = Shader.PropertyToID("_SatWeight3");
        protected static readonly int LumWeight3 = Shader.PropertyToID("_LumWeight3");
        protected static readonly int MaskLow3 = Shader.PropertyToID("_MaskLow3");
        protected static readonly int MaskHigh3 = Shader.PropertyToID("_MaskHigh3");
        protected static readonly int Sample3Smoothness = Shader.PropertyToID("_Sample3Smoothness");
        protected static readonly int BaseSmoothness = Shader.PropertyToID("_BaseSmoothness");
        protected static readonly int MetallicTex = Shader.PropertyToID("_MetallicTex");

        #endregion
    }
}