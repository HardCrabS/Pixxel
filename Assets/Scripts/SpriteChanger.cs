using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteChanger : MonoBehaviour
{
    Sprite spriteToSet;
    public void ChangeSprite()
    {
        GetComponent<Image>().sprite = spriteToSet;
    }

    public void SetSprite(Sprite sprite)
    {
        spriteToSet = sprite;
    }
}