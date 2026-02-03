using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandDebugger : MonoBehaviour
{
    public List<MeshRenderer> handMeshDebuggers;
    public List<SkinnedMeshRenderer> armModels;
    public bool debugMode=false;
    public bool showArmModels=true;

    // Update is called once per frame
    void Update()
    {
        if ((Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.D)))
        {
            debugMode = !debugMode;
        }
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.A))
        {
            showArmModels = !showArmModels;
        }
        if (showArmModels)
        {
            foreach (SkinnedMeshRenderer smr in armModels)
            {
                smr.enabled = true;
            }
        }
        else
        {
            foreach (SkinnedMeshRenderer smr in armModels)
            {
                smr.enabled = false;
            }
        }
        if (debugMode)
        {
            foreach (MeshRenderer mr in handMeshDebuggers)
            {
                mr.enabled = true;
            }
        }
        else
        {
            foreach (MeshRenderer mr in handMeshDebuggers)
            {
                mr.enabled = false;
            }
        }
    }
}
