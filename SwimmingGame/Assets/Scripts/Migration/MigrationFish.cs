using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MigrationFish : MigrationNPC
{
    public SpriteCollectionList spriteLists;

    public override void Start()
    {
        SpriteRenderer spriteRenderer=GetComponentInChildren<SpriteRenderer>();
        //spriteRenderer.material.color=Color.HSVToRGB(Random.Range(0f,1f),33f/255f,1f);
        NPCOverworld npcOverworld=GetComponentInChildren<NPCOverworld>();
        npcOverworld.strokeFrequency=npcOverworld.strokeFrequency+Random.Range(-strokeFrequencyVariance,strokeFrequencyVariance);
        int i=Random.Range(0,spriteLists.spriteList.Length);
        GetComponentInChildren<SpriteAnimator>().sprites=spriteLists.spriteList[i].sprites;
        float s=Random.Range(0.9f,1.5f);
        spriteRenderer.transform.localScale=Vector3.one*s;

        if(particleSystem!=null && Random.Range(0f,1f)<=particleSystemLikeliness){
            particleSystem.SetActive(true);
        }
    }

}
