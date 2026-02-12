using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Sprite Collection", menuName = "ScriptableObjects/Sprite Collection", order = 2)]
public class SpriteCollection: ScriptableObject
{
    public Sprite[] sprites;
}

[System.Serializable]
public class SpriteList
{
    public Sprite[] sprites;
}