using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class CopySpriteValues : MonoBehaviour

{
    public SpriteRenderer spriteToCopy;
    public bool copyScale=false;

    public bool copyFlipX=false;
    public bool copyFlipY=false;

    public bool copySpriteLibraryAsset=false;
    public SpriteLibraryAsset[] spriteLibraryAssets; 

    private SpriteRenderer spriteRenderer;

    public bool copyHue=false;

    public bool copyColor=false;

    [Tooltip("Match copied sprite's opacity while maintaining original color.")]
    public bool matchOpacity=false;

    public bool copyImage=false;
    private Color originalColor;

    private bool copied=false;
    [Tooltip("If permanent copy throughout runtime, not just at the start of scene.")]
    public bool permanent=false;

    void Start()
    {
        if(matchOpacity){
            originalColor=GetComponentInChildren<SpriteRenderer>().color;
        }   
    }

    void LateUpdate()
    {
        if(!copied || permanent){
            spriteRenderer=GetComponentInChildren<SpriteRenderer>();

            if(copyScale){
                transform.localScale=spriteToCopy.transform.localScale;
            }

            if(copyFlipX){
                spriteRenderer.flipX=spriteToCopy.flipX;
            }

            if(copyFlipY){
                spriteRenderer.flipY=spriteToCopy.flipY;
            }

            if(copySpriteLibraryAsset){
                SpriteLibrary spriteLibrary=GetComponent<SpriteLibrary>();
                spriteLibrary.spriteLibraryAsset=spriteToCopy.GetComponent<SpriteLibrary>().spriteLibraryAsset;
            }

            if(copyHue){
                if(spriteRenderer.material.HasFloat("_Hue_Offset")){
                    spriteRenderer.material.SetFloat("_Hue_Offset",spriteToCopy.material.GetFloat("_Hue_Offset"));
                }
                else spriteRenderer.material.color=spriteToCopy.material.color;
            }

            if(copyColor){
                spriteRenderer.color=spriteToCopy.color;
            }

            if(matchOpacity){
                Color c=originalColor;
                c.a=spriteToCopy.color.a*c.a;
                spriteRenderer.color=c;
            }

            if(copyImage){
                spriteRenderer.sprite=spriteToCopy.sprite;
            }

            copied=true;
        }
    }

}
