using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Collection/Banner")]
public class Banner : ScriptableObject 
{
    [SerializeField] string title;
    [TextArea(2, 4)] [SerializeField] string description;
    [SerializeField] Sprite sprite;

    public string Title { get { return title; } }
    public string Description { get { return description; } }
    public Sprite Sprite { get { return sprite; } }
}
