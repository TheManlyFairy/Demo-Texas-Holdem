using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
using System.Linq;

public class Debugger : MonoBehaviour
{
    public List<Card>  hand, communityCards, cards, tieBreakerCards;
    public Hand strength, communityStrength;
    public TexasPokerHand pokerHand;
    public Card rankingCard;

    private void Start()
    {
        cards = new List<Card>();
        cards.AddRange(communityCards);
        cards.AddRange(hand);
        cards.Sort(new CompareCardsByValue());
        strength = GetHandStrength(cards);
       /* Debug.Log("Royal Flush: " + RoyalFlush(cards));
        Debug.Log("Straight Flush: " + StraightFlush(cards));
        Debug.Log("Four of a Kind: " + FourKind(cards));
        Debug.Log("Full House: " + FullHouse(cards));
        Debug.Log("Flush: " + Flush(cards));
        Debug.Log("Straight: " + Straight(cards));
        Debug.Log("Three of a Kind: " + ThreeKind(cards));
        Debug.Log("Two Pairs: " + TwoPairs(cards));
        Debug.Log("Pair: " + Pair(cards));*/
    }

    #region Helper Methods
    bool ContainsValue(List<Card> cards, CardValue value)
    {
        foreach (Card card in cards)
        {
            if (card.value == value)
                return true;
        }
        return false;
    }
    bool ContainsCard(List<Card> cards, CardValue value, CardSuit suit)
    {
        foreach (Card card in cards)
        {
            if (card.value == value && card.suit == suit)
                return true;
        }
        return false;
    }

    List<Card> DistinctCardsByValue(List<Card> hand)
    {
        List<Card> distinctList = new List<Card>();
        HashSet<CardValue> distinctValues = new HashSet<CardValue>();

        foreach(Card card in hand)
        {
            if (distinctValues.Add(card.value))
                distinctList.Add(card);
        }
        return distinctList;
    }

    #endregion
    #region PokerHand
    public Hand GetHandStrength(List<Card> hand)
    {
        if (RoyalFlush(hand)) { return Hand.Royal; }
        if (StraightFlush(hand)) { return Hand.StraightFlush; }
        if (FourKind(hand)) { return Hand.FourKind; }
        if (FullHouse(hand)) {; return Hand.FullHouse; }
        if (Flush(hand)) {; return Hand.Flush; }
        if (Straight(hand)) {; return Hand.Straight; }
        if (ThreeKind(hand)) {; return Hand.ThreeKind; }
        if (TwoPairs(hand)) {; return Hand.TwoPair; }
        if (Pair(hand)) {; return Hand.Pair; }
        High(hand); return Hand.High;
    }
    bool RoyalFlush(List<Card> hand)
    {
        if (hand[0].value != CardValue.Ace)
            return false;
        //find all cards with value of 10
        List<Card> listOfTens = new List<Card>();

        foreach (Card card in hand)
        {
            if (card.value == CardValue.Ten)
                listOfTens.Add(card);
        }
        if (listOfTens.Count > 3)
            return false;

        foreach (Card ten in listOfTens)
        {
            if (ContainsCard(hand, CardValue.Jack, ten.suit) &&
               ContainsCard(hand, CardValue.Queen, ten.suit) &&
               ContainsCard(hand, CardValue.King, ten.suit) &&
               ContainsCard(hand, CardValue.Ace, ten.suit))
            {
                rankingCard = hand.Where(card => card.value == CardValue.Ace && card.suit == ten.suit).First();
                tieBreakerCards.Add(ten);
                tieBreakerCards.AddRange(hand.Where(card => (card.suit == ten.suit) &&
                (card.value == CardValue.Jack || card.value == CardValue.Queen || card.value == CardValue.King)));
                tieBreakerCards.Sort(new CompareCardsByValue());
                return true;
            }

        }
        return false;
    }
    bool StraightFlush(List<Card> hand)
    {
        if (hand[hand.Count - 1].value > CardValue.Nine || hand[0].value < CardValue.Five)
            return false;

        List<Card> matchList;
        Card checkedCard;

        for (int i = 0; i < hand.Count - 5; i++)
        {
            matchList = new List<Card>();
            checkedCard = hand[i];
            foreach (Card card in hand)
            {
                if (card.Equals(checkedCard))
                    continue;
                if (card.value < checkedCard.value && card.value >= checkedCard.value - 4 && card.suit == checkedCard.suit)
                    matchList.Add(card);
            }
            if (matchList.Count >= 4)
            {
                rankingCard = checkedCard;
                tieBreakerCards.AddRange(matchList);
                return true;
            }

        }
        return false;
    }
    bool FourKind(List<Card> hand)
    {
        List<Card> matchList;
        Card checkedCard;
        for (int i = 0; i <= hand.Count - 4; i++)
        {
            matchList = new List<Card>();
            checkedCard = hand[i];
            foreach (Card card in hand)
            {
                if (card.Equals(checkedCard))
                    continue;
                if (card.value == checkedCard.value)
                    matchList.Add(card);
            }
            if (matchList.Count == 3)
            {
                rankingCard = checkedCard;
                tieBreakerCards.Add(hand.Where(card => card.value != checkedCard.value).First());
                return true;
            }
            
        }

        return false;
    }
    bool FullHouse(List<Card> hand)
    {
        List<Card> matchList = new List<Card>();
        Card matchThree=null;
        Card checkedCard;
        int matchCount;

        for(int i=0; i<hand.Count-3; i++)
        {
            matchCount = 0;
            checkedCard = hand[i];
            foreach(Card card in hand)
            {
                if (card.Equals(checkedCard))
                    continue;
                if (card.value == checkedCard.value)
                    matchCount++;
            }
            if(matchCount==2)
            {
                matchThree = checkedCard;
                break;
            }
        }

        if(matchThree!=null)
        {
            for (int i = 0; i <= hand.Count - 2; i++)
            {
                matchCount = 0;
                checkedCard = hand[i];
                foreach (Card card in hand)
                {
                    if (card.Equals(checkedCard) || card.value == matchThree.value)
                        continue;
                    if (card.value == checkedCard.value)
                        matchCount++;
                }
                if (matchCount >= 1)
                {
                    matchList.Add(checkedCard);
                }
            }

            matchList = DistinctCardsByValue(matchList);
            if(matchList.Count>=1)
            {
                rankingCard = matchThree;
                tieBreakerCards.Add(matchList[0]);
                return true;
            }
        }
       

        return false;
    }
    bool Flush(List<Card> hand)
    {
        List<Card> matchList;
        List<Card> reverseHand = new List<Card>();
        reverseHand.AddRange(hand);
        reverseHand.Reverse();
        Card checkedCard;
        for (int i = hand.Count - 1; i >= 0; i--)
        {
            matchList = new List<Card>();
            checkedCard = hand[i];
            foreach (Card card in reverseHand)
            {
                if (card.Equals(checkedCard))
                    continue;
                if (card.suit == checkedCard.suit)
                    matchList.Add(card);
            }
            if (matchList.Count >= 4)
                return true;
        }
        return false;
    }
    bool Straight(List<Card> hand)
    {
        if (hand[hand.Count - 1].value > CardValue.Nine || hand[0].value < CardValue.Five)
            return false;

        List<Card> matchList;
        Card checkedCard;

        for (int i = 0; i <= hand.Count - 5; i++)
        {
            matchList = new List<Card>();
            checkedCard = hand[i];
            foreach (Card card in hand)
            {
                if (card.Equals(checkedCard))
                    continue;
                if (card.value < checkedCard.value && card.value >= checkedCard.value - 4)
                    matchList.Add(card);
            }
            matchList = DistinctCardsByValue(matchList);
            if (matchList.Count >= 4)
            {
                rankingCard = checkedCard;
                tieBreakerCards = matchList;
                tieBreakerCards.Sort(new CompareCardsByValue());
                while (tieBreakerCards.Count > 4)
                    tieBreakerCards.RemoveAt(tieBreakerCards.Count - 1);
                return true;
            }
                
        }
        return false;
    }
    bool ThreeKind(List<Card> hand)
    {
        List<Card> matchList;
        Card checkedCard;
        for(int i=0; i<=hand.Count-3;i++)
        {
            matchList = new List<Card>();
            checkedCard = hand[i];
            foreach(Card card in hand)
            {
                if (card.Equals(checkedCard))
                    continue;

                if (card.value == checkedCard.value)
                    matchList.Add(card);
            }
            if(matchList.Count==2)
            {
                rankingCard = checkedCard;
                tieBreakerCards.AddRange((hand.Where(card => card.value != checkedCard.value).ToList().GetRange(0, 2)));
                return true;
            }
        }
        return false;
    }
    bool TwoPairs(List<Card> hand)
    {
        int matchCount;
        List<Card> matchList = new List<Card>();
        Card checkedCard;
        for(int i=0; i<hand.Count-2; i++)
        {
            checkedCard = hand[i];
            matchCount = 0;
            foreach(Card card in hand)
            {
                if (card.Equals(checkedCard))
                    continue;
                if (card.value == checkedCard.value)
                    matchCount++;
            }
            if (matchCount == 1)
                matchList.Add(checkedCard);
        }
        matchList = DistinctCardsByValue(matchList);
        matchList.Sort(new CompareCardsByValue());

        if (matchList.Count < 2)
            return false;

        if(matchList.Count>=2)
        {
            rankingCard = matchList[0];
            tieBreakerCards.Add(matchList[1]);
            tieBreakerCards.Add(hand.Where(card => card.value != matchList[0].value && card.value != matchList[1].value).First());
            return true;
        }

        return false;
    }
    bool Pair(List<Card> hand)
    {
        List<Card> matchList;
        Card checkedCard;
        for (int i = 0; i <= hand.Count - 2; i++)
        {
            matchList = new List<Card>();
            checkedCard = hand[i];
            foreach (Card card in hand)
            {
                if (card.Equals(checkedCard))
                    continue;

                if (card.value == checkedCard.value)
                    matchList.Add(card);
            }
            if (matchList.Count == 1)
            {
                rankingCard = checkedCard;
                tieBreakerCards.AddRange((hand.Where(card => card.value != checkedCard.value).ToList().GetRange(0, 3)));
                return true;
            }
        }
        return false;
    }
    void High(List<Card> hand)
    {
        rankingCard = hand[0];
        tieBreakerCards.AddRange(hand.GetRange(1, 4));
    }
    #endregion
}
