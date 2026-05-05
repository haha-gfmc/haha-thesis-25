using UnityEngine;

public class SexBodyMaterialController : SexMaterialControllerBase
{
    [Header("Default")]
    public Color defaultColor = Color.white;
    public Color defaultEmission = Color.black;
    public float defaultShiftingSpeedX;
    public float defaultShiftingSpeedY;
    public Vector2 defaultTiling = Vector2.one;
    public float defaultNoiseValue;
    public float defaultContrast = 1f;
    public float defaultSaturation = 1f;
    public float defaultSmoothness;
    public float defaultNormal;
    public float defaultAdd;

    [Header("Excited")]
    public Color excitedColor = Color.white;
    public Color excitedEmission = Color.white;
    public float excitedShiftingSpeedX;
    public float excitedShiftingSpeedY;
    public Vector2 excitedTiling = Vector2.one;
    public float excitedNoiseValue;
    public float excitedContrast = 1f;
    public float excitedSaturation = 1f;
    public float excitedSmoothness;
    public float excitedNormal;
    public float excitedAdd;

    private static readonly int ColorID = Shader.PropertyToID("_Color");
    private static readonly int EmissionID = Shader.PropertyToID("_Emission");
    private static readonly int ShiftingSpeedXID = Shader.PropertyToID("_ShiftingSpeedX");
    private static readonly int ShiftingSpeedYID = Shader.PropertyToID("_ShiftingSpeedY");
    private static readonly int TilingID = Shader.PropertyToID("_Tiling");
    private static readonly int NoiseValueID = Shader.PropertyToID("_NoiseValue");
    private static readonly int ContrastID = Shader.PropertyToID("_Contrast");
    private static readonly int SaturationID = Shader.PropertyToID("_Saturation");
    private static readonly int SmoothnessID = Shader.PropertyToID("_Smoothness");
    private static readonly int NormalID = Shader.PropertyToID("_Normal");
    private static readonly int AddID = Shader.PropertyToID("_Add");

    protected override void Awake()
    {
        base.Awake();
    }

    public override void SetExcitement(float excitement, float lerpSpeed)
    {
        if (!UsingNormalRenderer() && !UsingObiRopeRenderer()) return;

        float t = Mathf.Clamp01(excitement);
        float dt = lerpSpeed * Time.deltaTime;

        BeginBlock();

        SetColor(ColorID, Color.Lerp(GetColor(ColorID, defaultColor), Color.Lerp(defaultColor, excitedColor, t), dt));
        SetColor(EmissionID, Color.Lerp(GetColor(EmissionID, defaultEmission), Color.Lerp(defaultEmission, excitedEmission, t), dt));

        SetFloat(ShiftingSpeedXID, Mathf.Lerp(GetFloat(ShiftingSpeedXID, defaultShiftingSpeedX), Mathf.Lerp(defaultShiftingSpeedX, excitedShiftingSpeedX, t), dt));
        SetFloat(ShiftingSpeedYID, Mathf.Lerp(GetFloat(ShiftingSpeedYID, defaultShiftingSpeedY), Mathf.Lerp(defaultShiftingSpeedY, excitedShiftingSpeedY, t), dt));
        SetVector2(TilingID, Vector2.Lerp(GetVector2(TilingID, defaultTiling), Vector2.Lerp(defaultTiling, excitedTiling, t), dt));

        SetFloat(NoiseValueID, Mathf.Lerp(GetFloat(NoiseValueID, defaultNoiseValue), Mathf.Lerp(defaultNoiseValue, excitedNoiseValue, t), dt));
        SetFloat(ContrastID, Mathf.Lerp(GetFloat(ContrastID, defaultContrast), Mathf.Lerp(defaultContrast, excitedContrast, t), dt));
        SetFloat(SaturationID, Mathf.Lerp(GetFloat(SaturationID, defaultSaturation), Mathf.Lerp(defaultSaturation, excitedSaturation, t), dt));
        SetFloat(SmoothnessID, Mathf.Lerp(GetFloat(SmoothnessID, defaultSmoothness), Mathf.Lerp(defaultSmoothness, excitedSmoothness, t), dt));
        SetFloat(NormalID, Mathf.Lerp(GetFloat(NormalID, defaultNormal), Mathf.Lerp(defaultNormal, excitedNormal, t), dt));
        SetFloat(AddID, Mathf.Lerp(GetFloat(AddID, defaultAdd), Mathf.Lerp(defaultAdd, excitedAdd, t), dt));

        EndBlock();
    }
}