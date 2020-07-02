using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Card", menuName = "Card")]
public class Card : ScriptableObject
{
    [SerializeField] string title;
    [TextArea(3, 5)] [SerializeField] string description;
    [SerializeField] Sprite cardSprite;
    [SerializeField] string cardType;
    public string Title { get { return title; } }
    public string Description { get { return description; } }
    public Sprite Sprite { get { return cardSprite; } }
    public string CardType { get { return cardType; } }
}
