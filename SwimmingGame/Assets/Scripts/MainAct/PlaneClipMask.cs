using UnityEngine;
using Obi;

[ExecuteAlways]
public class PlaneClipMask : MonoBehaviour
{
    [Header("Toggle")]
    public bool enableClipping = true;

    [Header("Plane")]
    public Transform clipPlane;

    public enum NormalAxis { Forward, Back, Up, Down, Right, Left }
    public NormalAxis normalAxis = NormalAxis.Forward;

    [Header("Unity Renderers")]
    public bool affectUnityRenderers = true;
    public bool includeChildren = true;

    [Header("Obi Rope Renderer")]
    public bool affectObiRopeRenderer = true;
    public ObiRopeExtrudedRenderer obiRopeRenderer;

    private Renderer[] unityRenderers;
    private MaterialPropertyBlock block;

    private Material obiMaterialInstance;

    private static readonly int ClipPlanePositionID = Shader.PropertyToID("_ClipPlanePosition");
    private static readonly int ClipPlaneNormalID = Shader.PropertyToID("_ClipPlaneNormal");

    void OnEnable()
    {
        block = new MaterialPropertyBlock();
        RefreshRenderers();
        SetupObiMaterialInstance();
    }

    void OnValidate()
    {
        RefreshRenderers();
        SetupObiMaterialInstance();
    }

    void LateUpdate()
    {
        if (!enableClipping || clipPlane == null)
        {
            SetDisabledClip();
            return;
        }

        Vector3 normal = GetNormal().normalized;

        if (affectUnityRenderers)
            ApplyToUnityRenderers(clipPlane.position, normal);

        if (affectObiRopeRenderer)
            ApplyToObiRopeRenderer(clipPlane.position, normal);
    }

    public void RefreshRenderers()
    {
        unityRenderers = includeChildren
            ? GetComponentsInChildren<Renderer>(true)
            : GetComponents<Renderer>();

        if (obiRopeRenderer == null)
            obiRopeRenderer = GetComponent<ObiRopeExtrudedRenderer>();
    }

    void SetupObiMaterialInstance()
    {
        if (obiRopeRenderer == null || obiRopeRenderer.material == null)
            return;

        if (Application.isPlaying)
        {
            if (obiMaterialInstance == null)
            {
                obiMaterialInstance = Instantiate(obiRopeRenderer.material);
                obiMaterialInstance.name = obiRopeRenderer.material.name + " (Plane Clip Instance)";
                obiRopeRenderer.material = obiMaterialInstance;
            }
        }
        else
        {
            obiMaterialInstance = obiRopeRenderer.material;
        }
    }

    void ApplyToUnityRenderers(Vector3 position, Vector3 normal)
    {
        if (unityRenderers == null) return;

        foreach (Renderer r in unityRenderers)
        {
            if (r == null) continue;

            r.GetPropertyBlock(block);
            block.SetVector(ClipPlanePositionID, position);
            block.SetVector(ClipPlaneNormalID, normal);
            r.SetPropertyBlock(block);
        }
    }

    void ApplyToObiRopeRenderer(Vector3 position, Vector3 normal)
    {
        if (obiMaterialInstance == null)
            SetupObiMaterialInstance();

        if (obiMaterialInstance == null) return;

        obiMaterialInstance.SetVector(ClipPlanePositionID, position);
        obiMaterialInstance.SetVector(ClipPlaneNormalID, normal);
    }

    void SetDisabledClip()
    {
        Vector3 farAway = new Vector3(999999, 999999, 999999);
        Vector3 normal = Vector3.up;

        if (affectUnityRenderers)
            ApplyToUnityRenderers(farAway, normal);

        if (affectObiRopeRenderer)
            ApplyToObiRopeRenderer(farAway, normal);
    }

    Vector3 GetNormal()
    {
        switch (normalAxis)
        {
            case NormalAxis.Forward: return clipPlane.forward;
            case NormalAxis.Back: return -clipPlane.forward;
            case NormalAxis.Up: return clipPlane.up;
            case NormalAxis.Down: return -clipPlane.up;
            case NormalAxis.Right: return clipPlane.right;
            case NormalAxis.Left: return -clipPlane.right;
            default: return clipPlane.forward;
        }
    }
}