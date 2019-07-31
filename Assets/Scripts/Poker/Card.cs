using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

[CreateAssetMenu(menuName =("Poker/Card"))]
public class Card : ScriptableObject, IComparable
{
    public CardValue value;
    public CardSuit suit;
    public bool isCommunityCard;
    public Sprite sprite;
    public bool markedForDiscard;
    public Card(CardValue value, CardSuit suit)
    {
        this.value = value;
        this.suit = suit;
    }
    public Card(CardValue value, CardSuit suit, Sprite sprite)
    {
        this.value = value;
        this.suit = suit;
        this.sprite = sprite;
    }
    public int CompareTo(object obj)
    {
        if(obj is Card)
        {
            int comparedValue=0;
            Card otherCard = obj as Card;
            if (otherCard.suit != suit)
                comparedValue += otherCard.suit - suit;
            comparedValue += otherCard.value - value;
            return comparedValue;

        }
        throw new ArgumentException("Other object is not a Card");
    }


}
