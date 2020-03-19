using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "World", menuName = "World")]
public class WorldTemplate : ScriptableObject
{
    public LevelTemplate[] levels;
}
