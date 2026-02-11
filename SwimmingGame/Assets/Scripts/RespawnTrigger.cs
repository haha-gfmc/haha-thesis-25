using Unity.VisualScripting;
using UnityEngine;

public class RespawnTrigger : MonoBehaviour
{
    [Tooltip("Location to respawn at")]
    public Transform origin;

    public bool triggered;

    private Swimmer swimmer;

    void Start(){
        if (origin==null) origin=transform;
        swimmer=FindObjectOfType<Swimmer>();
    }

    void Update(){
        if(triggered){
            swimmer.respawnTransform=transform;
            triggered=false;
        }
    }

    void OnTriggerEnter(Collider other){
        if(other.gameObject.tag=="Player"){
            triggered=true;
        }
    }
}