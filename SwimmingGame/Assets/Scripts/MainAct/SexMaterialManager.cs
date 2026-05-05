using System.Collections.Generic;
using UnityEngine;

public class SexMaterialManager : MonoBehaviour
{
    public List<float> excitementLevels = new List<float>();

    [Header("Material Controllers")]
    public List<SexMaterialControllerBase> controllers = new List<SexMaterialControllerBase>();

    public float excitementLerpSpeed = 1f;

    private void Update()
    {
        for (int i = 0; i < controllers.Count; i++)
        {
            if (controllers[i] == null) continue;

            float excitement = 0f;

            if (i < excitementLevels.Count)
                excitement = excitementLevels[i];

            controllers[i].SetExcitement(excitement, excitementLerpSpeed);
        }
    }
}