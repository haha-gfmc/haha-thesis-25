using UnityEngine;
using Obi;

public abstract class SexMaterialControllerBase : MonoBehaviour
{
    [Header("Normal Renderer Target")]
    public Renderer targetRenderer;
    public int materialIndex = 0;

    [Header("Obi Rope Target")]
    public ObiRopeExtrudedRenderer targetObiRopeRenderer;

    protected MaterialPropertyBlock block;

    private Material originalObiMaterial;
    protected Material runtimeObiMaterial;

    protected virtual void Awake()
    {
        block = new MaterialPropertyBlock();

        if (targetObiRopeRenderer != null)
        {
            originalObiMaterial = targetObiRopeRenderer.material;

            if (originalObiMaterial != null)
            {
                runtimeObiMaterial = new Material(originalObiMaterial);
                runtimeObiMaterial.name = originalObiMaterial.name + " Runtime";
                targetObiRopeRenderer.material = runtimeObiMaterial;
            }
        }
    }

    protected bool UsingNormalRenderer()
    {
        return targetRenderer != null;
    }

    protected bool UsingObiRopeRenderer()
    {
        return targetObiRopeRenderer != null && runtimeObiMaterial != null;
    }

    protected void BeginBlock()
    {
        if (targetRenderer != null)
            targetRenderer.GetPropertyBlock(block, materialIndex);
    }

    protected void EndBlock()
    {
        if (targetRenderer != null)
            targetRenderer.SetPropertyBlock(block, materialIndex);
    }

    protected float GetFloat(int id, float fallback)
    {
        if (UsingNormalRenderer())
        {
            float v = block.GetFloat(id);
            return v == 0f ? fallback : v;
        }

        if (UsingObiRopeRenderer())
            return runtimeObiMaterial.GetFloat(id);

        return fallback;
    }

    protected Color GetColor(int id, Color fallback)
    {
        if (UsingNormalRenderer())
        {
            Color c = block.GetColor(id);
            return c == Color.clear ? fallback : c;
        }

        if (UsingObiRopeRenderer())
            return runtimeObiMaterial.GetColor(id);

        return fallback;
    }

    protected Vector2 GetVector2(int id, Vector2 fallback)
    {
        if (UsingNormalRenderer())
        {
            Vector4 v = block.GetVector(id);
            if (v == Vector4.zero) return fallback;
            return new Vector2(v.x, v.y);
        }

        if (UsingObiRopeRenderer())
        {
            Vector4 v = runtimeObiMaterial.GetVector(id);
            return new Vector2(v.x, v.y);
        }

        return fallback;
    }

    protected void SetFloat(int id, float value)
    {
        if (UsingNormalRenderer())
            block.SetFloat(id, value);

        if (UsingObiRopeRenderer())
            runtimeObiMaterial.SetFloat(id, value);
    }

    protected void SetColor(int id, Color value)
    {
        if (UsingNormalRenderer())
            block.SetColor(id, value);

        if (UsingObiRopeRenderer())
            runtimeObiMaterial.SetColor(id, value);
    }

    protected void SetVector2(int id, Vector2 value)
    {
        if (UsingNormalRenderer())
            block.SetVector(id, value);

        if (UsingObiRopeRenderer())
            runtimeObiMaterial.SetVector(id, value);
    }

    public abstract void SetExcitement(float excitement, float lerpSpeed);

    protected virtual void OnDisable()
    {
        if (targetRenderer != null)
            targetRenderer.SetPropertyBlock(null, materialIndex);

        if (targetObiRopeRenderer != null && originalObiMaterial != null)
            targetObiRopeRenderer.material = originalObiMaterial;

        if (runtimeObiMaterial != null)
            Destroy(runtimeObiMaterial);
    }
}