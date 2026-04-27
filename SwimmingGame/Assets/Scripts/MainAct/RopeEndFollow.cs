using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeEndFollow : MonoBehaviour
{
    public Transform reference;
    public string triggerTag;
    public bool isFollowing = true;

    private Vector3 positionOffset;

    void Start()
    {
        if (reference == null) return;
        positionOffset = transform.position - reference.position;
    }

    void Update()
    {
        if (!isFollowing || reference == null) return;

        transform.position = reference.position + positionOffset;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(triggerTag))
            StopFollowing();
    }

    public void StopFollowing()
    {
        isFollowing = false;

        var rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}