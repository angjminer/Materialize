﻿﻿#region

using System.Collections;
using General;
using Settings;
using UnityEngine;

#endregion

namespace Gui
{
    public class AoFromNormalGui : MonoBehaviour, IProcessor, IHideable
    {
        public bool Active
        {
            get => gameObject.activeSelf;
            set => gameObject.SetActive(value);
        }

        private static readonly int FinalContrast = Shader.PropertyToID("_FinalContrast");
        private static readonly int FinalBias = Shader.PropertyToID("_FinalBias");
        private static readonly int AoBlend = Shader.PropertyToID("_AOBlend");
        private static readonly int ImageInput = Shader.PropertyToID("ImageInput");
        private static readonly int MainTex = Shader.PropertyToID("_MainTex");
        private static readonly int ImageSize = Shader.PropertyToID("_ImageSize");
        private static readonly int Spread = Shader.PropertyToID("_Spread");
        private static readonly int HeightTex = Shader.PropertyToID("_HeightTex");
        private static readonly int BlendTex = Shader.PropertyToID("_BlendTex");
        private static readonly int Depth = Shader.PropertyToID("_Depth");
        private static readonly int BlendAmount = Shader.PropertyToID("_BlendAmount");
        private static readonly int Progress = Shader.PropertyToID("_Progress");
        private static readonly int Slider = Shader.PropertyToID("_Slider");
        private Texture2D _aoMap;

        private AoSettings _aos;
        private RenderTexture _blendedAoMap;
        private bool _doStuff;

        private int _imageSizeX;
        private int _imageSizeY;
        private bool _newTexture;

        private bool _settingsInitialized;

        private Rect _windowRect;
        private float _slider = 0.5f;

        public GameObject TestObject;

        public Material ThisMaterial;
        private Coroutine _processingNormalCoroutine;
        private Renderer _testObjectRenderer;
        private int _windowId;

        public ComputeShader AoCompute;
        private int _kernelAo;
        private int _kernelCombine;
        private static readonly int NormalTex = Shader.PropertyToID("_NormalTex");
        private bool _readyToProcess;

        private void Awake()
        {
            _testObjectRenderer = TestObject.GetComponent<Renderer>();
            ThisMaterial = new Material(ThisMaterial.shader);
            _windowRect = new Rect(10.0f, 265.0f, 300f, 280f);
        }

        private void OnDisable()
        {
            CleanupTextures();
            _readyToProcess = false;
        }

        public void GetValues(ProjectObject projectObject)
        {
            InitializeSettings();
            projectObject.AoSettings = _aos;
        }

        public void SetValues(ProjectObject projectObject)
        {
            if (projectObject.AoSettings != null)
            {
                _aos = projectObject.AoSettings;
            }
            else
            {
                _settingsInitialized = false;
                InitializeSettings();
            }

            _doStuff = true;
        }

        private void InitializeSettings()
        {
            if (_settingsInitialized) return;
            _aos = new AoSettings {Blend = TextureManager.Instance && TextureManager.Instance.HeightMap ? 1.0f : 0.0f};

            _settingsInitialized = true;
        }

        private void Start()
        {
            _windowId = ProgramManager.Instance.GetWindowId;
            InitializeSettings();

            _kernelAo = AoCompute.FindKernel("CSAo");
            _kernelCombine = AoCompute.FindKernel("CSCombineAo");
        }

        public void DoStuff()
        {
            _doStuff = true;
        }

        public void NewTexture()
        {
            _newTexture = true;
        }

        private void Update()
        {
            if (ProgramManager.IsLocked) return;

            if (_newTexture)
            {
                InitializeTextures();
                _newTexture = false;
            }

            if (_doStuff)
            {
                StopAllCoroutines();
                _processingNormalCoroutine = StartCoroutine(ProcessNormalDepth());
                _doStuff = false;
            }

            ThisMaterial.SetFloat(FinalContrast, _aos.FinalContrast);
            ThisMaterial.SetFloat(FinalBias, _aos.FinalBias);
            ThisMaterial.SetFloat(AoBlend, _aos.Blend);
            ThisMaterial.SetFloat(Slider, _slider);
        }

        private void DoMyWindow(int windowId)
        {
            const int offsetX = 10;
            var offsetY = 30;

            GUI.Label(new Rect(offsetX, offsetY, 250, 30), "AO Reveal Slider");
            _slider = GUI.HorizontalSlider(new Rect(offsetX, offsetY + 20, 280, 10), _slider, 0.0f, 1.0f);
            offsetY += 35;

            if (GuiHelper.Slider(new Rect(offsetX, offsetY, 280, 50), "AO pixel Spread", _aos.Spread,
                out _aos.Spread, 10.0f, 100.0f)) _doStuff = true;
            offsetY += 40;

            if (GuiHelper.Slider(new Rect(offsetX, offsetY, 280, 50), "Pixel Depth", _aos.Depth,
                out _aos.Depth, 0.0f, 256.0f)) _doStuff = true;
            offsetY += 40;

            GuiHelper.Slider(new Rect(offsetX, offsetY, 280, 50), "Blend Normal AO and Depth AO", _aos.Blend,
                out _aos.Blend, 0.0f, 1.0f);
            offsetY += 40;

            GuiHelper.Slider(new Rect(offsetX, offsetY, 280, 50), "AO Power", _aos.FinalContrast,
                out _aos.FinalContrast, 0.1f, 10.0f);
            offsetY += 40;

            GuiHelper.Slider(new Rect(offsetX, offsetY, 280, 50), "AO Bias", _aos.FinalBias,
                out _aos.FinalBias, -1.0f, 1.0f);

            GUI.enabled = true;
            GUI.DragWindow(new Rect(0, 0, 10000, 20));
        }

        private void OnGUI()
        {
            if (Hide) return;
            MainGui.MakeScaledWindow(_windowRect, _windowId, DoMyWindow, "Normal + Depth to AO");
        }

        public void InitializeTextures()
        {
            _testObjectRenderer.sharedMaterial = ThisMaterial;

            CleanupTextures();

            _imageSizeX = TextureManager.Instance.NormalMap.width;
            _imageSizeY = TextureManager.Instance.NormalMap.height;

            General.Logger.Log("Initializing Textures of size: " + _imageSizeX + "x" + _imageSizeY);

            _blendedAoMap = TextureManager.Instance.GetTempRenderTexture(_imageSizeX, _imageSizeY);
            ThisMaterial.SetTexture(NormalTex, TextureManager.Instance.NormalMap);
        }

        public void Close()
        {
            CleanupTextures();
            gameObject.SetActive(false);
        }

        private void CleanupTextures()
        {
            RenderTexture.ReleaseTemporary(_blendedAoMap);
        }

        public IEnumerator Process()
        {
            while (!ProgramManager.Lock())
            {
                yield return null;
            }

            while (!_readyToProcess)
            {
                yield return null;
            }

            General.Logger.Log("Processing AO Map");

            var tempAoMap = TextureManager.Instance.GetTempRenderTexture(_imageSizeX, _imageSizeY);

            AoCompute.SetFloat(FinalBias, _aos.FinalBias);
            AoCompute.SetFloat(FinalContrast, _aos.FinalContrast);
            AoCompute.SetTexture(_kernelCombine, ImageInput, _blendedAoMap);
            AoCompute.SetFloat(AoBlend, _aos.Blend);
            AoCompute.SetVector(ImageSize, new Vector2(_imageSizeX, _imageSizeY));

            AoCompute.SetTexture(_kernelCombine, "ImageInput", _blendedAoMap);
            AoCompute.SetTexture(_kernelCombine, "Result", tempAoMap);
            var groupsX = (int) Mathf.Ceil(_imageSizeX / 8f);
            var groupsY = (int) Mathf.Ceil(_imageSizeY / 8f);
            AoCompute.Dispatch(_kernelCombine, groupsX, groupsY, 1);

            TextureManager.Instance.GetTextureFromRender(tempAoMap, ProgramEnums.MapType.Ao);
            RenderTexture.ReleaseTemporary(tempAoMap);

            yield return null;

            ProgramManager.Unlock();
        }


        public IEnumerator ProcessNormalDepth()
        {
            while (!ProgramManager.Lock())
            {
                yield return null;
            }

            General.Logger.Log("Processing Normal Depth to AO");

            AoCompute.SetVector(ImageSize, new Vector4(_imageSizeX, _imageSizeY, 0, 0));
            AoCompute.SetFloat(Spread, _aos.Spread);

            AoCompute.SetTexture(_kernelAo, ImageInput, TextureManager.Instance.NormalMap);

            Texture heightMap;
            if (TextureManager.Instance.HdHeightMap)
            {
                heightMap = TextureManager.Instance.HdHeightMap;
            }
            else if (TextureManager.Instance.HeightMap)
            {
                heightMap = TextureManager.Instance.HeightMap;
            }
            else
            {
                heightMap = Texture2D.blackTexture;
            }

            AoCompute.SetTexture(_kernelAo, HeightTex, heightMap);

            AoCompute.SetTexture(_kernelAo, BlendTex, _blendedAoMap);
            AoCompute.SetFloat(Depth, _aos.Depth);
            ThisMaterial.SetTexture(MainTex, _blendedAoMap);

            AoCompute.SetTexture(_kernelAo, "ImageInput", TextureManager.Instance.NormalMap);
            AoCompute.SetTexture(_kernelAo, "Result", _blendedAoMap);

            for (var i = 1; i < 100; i++)
            {
                AoCompute.SetFloat(BlendAmount, 1.0f / i);
                AoCompute.SetFloat(Progress, i / 100.0f);
                var groupsX = (int) Mathf.Ceil(_imageSizeX / 8f);
                var groupsY = (int) Mathf.Ceil(_imageSizeY / 8f);
                AoCompute.Dispatch(_kernelAo, groupsX, groupsY, 1);


                if (i % 25 == 0) yield return null;
            }

            yield return null;
            yield return null;

            _readyToProcess = true;

            ProgramManager.Unlock();
        }

        public bool Hide { get; set; }
    }
}