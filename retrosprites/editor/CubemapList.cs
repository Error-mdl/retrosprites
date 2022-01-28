using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Cubemap List", menuName = "Texture Lists/Cubemap List", order = 1)]
public class CubemapList : ScriptableObject {
    public Cubemap[] TexArray;
}
