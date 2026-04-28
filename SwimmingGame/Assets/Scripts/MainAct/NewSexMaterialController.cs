using UnityEngine;

public class NewSexMaterialController : SexMaterialControllerBase
{

    [System.Serializable]
    public struct Preset
    {
        public Color dark;
        public Color main;
        public Color glow;
        public Color edge;

        public Vector2 offsetA;
        public Vector2 offsetB;
        public Vector2 offsetC;
        public Vector2 offsetD;

        public float pixelate;
        public Vector2 tilling;

        public float warp1Strength;
        public float warp2Strength;
        public float flowSpeed;
        public float paletteSteps;
        public float normalStrength;
        public float interlaceOffset;
        public float emissionStrength;
        public float glowThreshold;

        public float stripeScale;
        public float stripeWidth;
        public float stripeDistort;

        public float scalineStrength;
        public float scalineDensity;
    }

    public Preset defaultPreset;
    public Preset excitedPreset;

    private static readonly int DarkID = Shader.PropertyToID("_Dark");
    private static readonly int MainID = Shader.PropertyToID("_Main");
    private static readonly int GlowID = Shader.PropertyToID("_Glow");
    private static readonly int EdgeID = Shader.PropertyToID("_Edge");

    private static readonly int OffsetAID = Shader.PropertyToID("_OffsetA");
    private static readonly int OffsetBID = Shader.PropertyToID("_OffsetB");
    private static readonly int OffsetCID = Shader.PropertyToID("_OffsetC");
    private static readonly int OffsetDID = Shader.PropertyToID("_OffsetD");

    private static readonly int PixelateID = Shader.PropertyToID("_Pixelate");
    private static readonly int TillingID = Shader.PropertyToID("_Tilling");

    private static readonly int Warp1StrengthID = Shader.PropertyToID("_Warp1Strength");
    private static readonly int Warp2StrengthID = Shader.PropertyToID("_Warp2Strength");
    private static readonly int FlowSpeedID = Shader.PropertyToID("_FlowSpeed");
    private static readonly int PaletteStepsID = Shader.PropertyToID("_PaletteSteps");
    private static readonly int NormalStrengthID = Shader.PropertyToID("_NormalStrength");
    private static readonly int InterlaceOffsetID = Shader.PropertyToID("_InterlaceOffset");
    private static readonly int EmissionStrengthID = Shader.PropertyToID("_EmissionStrength");
    private static readonly int GlowThresholdID = Shader.PropertyToID("_GlowThreshold");

    private static readonly int StripeScaleID = Shader.PropertyToID("_StripeScale");
    private static readonly int StripeWidthID = Shader.PropertyToID("_StripeWidth");
    private static readonly int StripeDistortID = Shader.PropertyToID("_StripeDistort");

    private static readonly int ScalineStrengthID = Shader.PropertyToID("_ScalineStrength");
    private static readonly int ScalineDensityID = Shader.PropertyToID("_ScalineDensity");

    private void Awake()
    {
        block = new MaterialPropertyBlock();
    }

public override void SetExcitement(float excitement, float lerpSpeed)
{
    if (!UsingNormalRenderer() && !UsingObiRopeRenderer()) return;
    if (block == null) block = new MaterialPropertyBlock();

    float t = Mathf.Clamp01(excitement);
    float dt = lerpSpeed * Time.deltaTime;

    BeginBlock();

    SetColor(DarkID, defaultPreset.dark, excitedPreset.dark, t, dt);
    SetColor(MainID, defaultPreset.main, excitedPreset.main, t, dt);
    SetColor(GlowID, defaultPreset.glow, excitedPreset.glow, t, dt);
    SetColor(EdgeID, defaultPreset.edge, excitedPreset.edge, t, dt);

    SetVector2(OffsetAID, defaultPreset.offsetA, excitedPreset.offsetA, t, dt);
    SetVector2(OffsetBID, defaultPreset.offsetB, excitedPreset.offsetB, t, dt);
    SetVector2(OffsetCID, defaultPreset.offsetC, excitedPreset.offsetC, t, dt);
    SetVector2(OffsetDID, defaultPreset.offsetD, excitedPreset.offsetD, t, dt);

    SetFloat(PixelateID, defaultPreset.pixelate, excitedPreset.pixelate, t, dt);
    SetVector2(TillingID, defaultPreset.tilling, excitedPreset.tilling, t, dt);

    SetFloat(Warp1StrengthID, defaultPreset.warp1Strength, excitedPreset.warp1Strength, t, dt);
    SetFloat(Warp2StrengthID, defaultPreset.warp2Strength, excitedPreset.warp2Strength, t, dt);
    SetFloat(FlowSpeedID, defaultPreset.flowSpeed, excitedPreset.flowSpeed, t, dt);
    SetFloat(PaletteStepsID, defaultPreset.paletteSteps, excitedPreset.paletteSteps, t, dt);
    SetFloat(NormalStrengthID, defaultPreset.normalStrength, excitedPreset.normalStrength, t, dt);
    SetFloat(InterlaceOffsetID, defaultPreset.interlaceOffset, excitedPreset.interlaceOffset, t, dt);
    SetFloat(EmissionStrengthID, defaultPreset.emissionStrength, excitedPreset.emissionStrength, t, dt);
    SetFloat(GlowThresholdID, defaultPreset.glowThreshold, excitedPreset.glowThreshold, t, dt);

    SetFloat(StripeScaleID, defaultPreset.stripeScale, excitedPreset.stripeScale, t, dt);
    SetFloat(StripeWidthID, defaultPreset.stripeWidth, excitedPreset.stripeWidth, t, dt);
    SetFloat(StripeDistortID, defaultPreset.stripeDistort, excitedPreset.stripeDistort, t, dt);

    SetFloat(ScalineStrengthID, defaultPreset.scalineStrength, excitedPreset.scalineStrength, t, dt);
    SetFloat(ScalineDensityID, defaultPreset.scalineDensity, excitedPreset.scalineDensity, t, dt);

    EndBlock();
}

    private void SetFloat(int id, float defaultValue, float excitedValue, float t, float dt)
    {
        float current = block.GetFloat(id);
        float target = Mathf.Lerp(defaultValue, excitedValue, t);
        block.SetFloat(id, Mathf.Lerp(current, target, dt));
    }

    private void SetColor(int id, Color defaultValue, Color excitedValue, float t, float dt)
    {
        Color current = block.GetColor(id);

        if (current == Color.clear)
            current = defaultValue;

        Color target = Color.Lerp(defaultValue, excitedValue, t);
        block.SetColor(id, Color.Lerp(current, target, dt));
    }

    private void SetVector2(int id, Vector2 defaultValue, Vector2 excitedValue, float t, float dt)
    {
        Vector4 currentRaw = block.GetVector(id);
        Vector2 current = new Vector2(currentRaw.x, currentRaw.y);

        if (current == Vector2.zero)
            current = defaultValue;

        Vector2 target = Vector2.Lerp(defaultValue, excitedValue, t);
        block.SetVector(id, Vector2.Lerp(current, target, dt));
    }

    private void OnDisable()
    {
        if (targetRenderer != null)
        {
            targetRenderer.SetPropertyBlock(null, materialIndex);
        }
    }
}