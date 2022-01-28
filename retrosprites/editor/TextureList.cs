using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Texture List", menuName = "Texture Lists/2D Texture List", order = 1)]
public class TextureList : ScriptableObject {
    public Texture2D[] TexArray;
}
