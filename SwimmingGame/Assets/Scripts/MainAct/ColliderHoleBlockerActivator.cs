using System.Collections;
using System.Collections.Generic;
using Obi;
using UnityEngine;

public class ColliderHoleBlockerActivator : MonoBehaviour
{

    public GameObject organHead;
    public float distanceThreshold;
    private Transform initialOrganHeadTransform;
    private float distance;
    // Start is called before the first frame update
    void Start()
    {
        initialOrganHeadTransform = organHead.transform;
    }

    // Update is called once per frame
    void Update()
    {
        distance = Vector3.Distance(transform.position, organHead.transform.position);
        //Debug.Log("Distance moved: " + distance);
        if (distance > distanceThreshold)
        {
            gameObject.GetComponent<SphereCollider>().enabled = true;
            gameObject.GetComponent<ObiCollider>().enabled = true;
        }
    }
}
