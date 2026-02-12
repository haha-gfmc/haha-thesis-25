using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Sprite Collection List", menuName = "ScriptableObjects/Sprite Collection List", order = 2)]
public class SpriteCollectionList: ScriptableObject
{
    public SpriteList[] spriteList;
}