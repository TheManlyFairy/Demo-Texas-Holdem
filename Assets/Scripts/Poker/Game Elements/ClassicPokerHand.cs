using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
using System.Linq;
public class ClassicPokerHand
{
    public List<Card> tieBreakerCards;
    public Hand strength;
    public Card rankingCard;
    int matchCount;
    Card currentCardInCheck;
    public void SetHandStrength(List<Card> hand)
    {
        if (RoyalFlush(hand)) { strength = Hand.Royal; return; }
        if (StraightFlush(hand)) { strength = Hand.StraightFlush; return; }
        if (FourKind(hand)) { strength = Hand.FourKind; return; }
        if (FullHouse(hand)) { strength = Hand.FullHouse; return; }
        if (Flush(hand)) { strength = Hand.Flush; return; }
        if (Straight(hand)) { strength = Hand.Straight; return; }
        if (ThreeKind(hand)) { strength = Hand.ThreeKind; return; }
        if (TwoPairs(hand)) { strength = Hand.TwoPair; return; }
        if (Pairs(hand)) { strength = Hand.Pair; return; }
        strength = Hand.High;
    }
    bool RoyalFlush(List<Card> hand)
    {
        if(StraightFlush(hand) && hand[4].value == CardValue.Ace && hand[0].value==CardValue.Ten)
        {
            rankingCard = hand[4];
            return true;
        }
        return false;
    }
    bool StraightFlush(List<Card> hand)
    {
        return Flush(hand)&&Straight(hand);
    }
    bool FourKind(List<Card> hand)
    {
        int matchCount;
        Card currentCardInCheck;
        for (int i = 0; i < 5; i++)
        {
            matchCount = 0;
            currentCardInCheck = hand[i];

            foreach (Card card in hand)
            {
                if (currentCardInCheck.Equals(card))
                    continue;
                if (currentCardInCheck.value == card.value)
                    matchCount++;
            }
            if (matchCount == 3)
            {
                rankingCard = currentCardInCheck;
                tieBreakerCards = new List<Card>();
                //tieBreakerCards.Add(currentCardInCheck);
                foreach(Card card in hand)
                {
                    if(card.value!= rankingCard.value)
                    {
                        tieBreakerCards.Add(card);
                        break;
                    }
                }
                return true;
            }
        }
        return false;
    }
    bool FullHouse(List<Card> hand)
    {
        //Debug.Log("ThreeKind returns " + ThreeKind(hand));
        //Debug.Log("Pairs returns: " + Pairs(hand));
        return Pairs(hand) && ThreeKind(hand);
    }
    bool Flush(List<Card> hand)
    {
        CardSuit firstCardSuit = hand[0].suit;

        for (int i = 0; i < 5; i++)
        {
            if (hand[i].suit != firstCardSuit)
                return false;
        }

        rankingCard = hand.Max();
        tieBreakerCards = new List<Card>();
        foreach(Card card in hand)
        {
            if (card.value != rankingCard.value)
                tieBreakerCards.Add(card);
        }
        return true;
    }
    bool Straight(List<Card> hand)
    {
        CardValue card1 = hand[0].value;
        CardValue card2 = hand[1].value;
        CardValue card3 = hand[2].value;
        CardValue card4 = hand[3].value;
        CardValue card5 = hand[4].value;

        if ((card4 - card3) == 1 && (card3 - card2 == 1) && (card2 - card1 == 1))
        {
            if (card5 == CardValue.Ace && card1 == CardValue.Two)
            {
                rankingCard = hand[4];
                tieBreakerCards = hand;
                hand[4].value = CardValue.LowAce;
                tieBreakerCards.Sort(new CompareCardsByValue());
                return true;
            }
            else if (card5 - card4 == 1)
            {
                rankingCard = hand[4];
                tieBreakerCards = hand;
                return true;
            }
        }
        return false;
    }
    bool ThreeKind(List<Card> hand)
    {
        
        for (int i = 0; i < 5; i++)
        {
            matchCount = 0;
            currentCardInCheck = hand[i];

            foreach (Card card in hand)
            {
                if (currentCardInCheck.Equals(card))
                    continue;
                if (currentCardInCheck.value == card.value)
                    matchCount++;
            }
            if (matchCount == 2)
            {
                tieBreakerCards = new List<Card>();
                rankingCard = currentCardInCheck;
                //tieBreakerCards.Add(currentCardInCheck);
                foreach(Card card in hand)
                {
                    if (card.value != rankingCard.value)
                        tieBreakerCards.Add(card);
                }
                return true;
            }
        }
        return false;
    }
    bool TwoPairs(List<Card> hand)
    {
        Card firstMatch = null;
        for (int i = 0; i < 5; i++)
        {
            matchCount = 0;
            currentCardInCheck = hand[i];
            {
                foreach (Card card in hand)
                {
                    if (currentCardInCheck.Equals(card))
                        continue;

                    if (currentCardInCheck.value == card.value)
                        matchCount++;
                }
                if (matchCount == 1)
                {
                    firstMatch = currentCardInCheck;
                    break;
                }
            }
        }
        if (firstMatch != null)
        {
            for (int i = 0; i < 5; i++)
            {
                matchCount = 0;
                currentCardInCheck = hand[i];
                {
                    foreach (Card card in hand)
                    {
                        if (currentCardInCheck.Equals(card) || currentCardInCheck.value == firstMatch.value)
                            continue;

                        if (currentCardInCheck.value == card.value)
                            matchCount++;
                    }
                    if (matchCount == 1)
                    {
                        tieBreakerCards = new List<Card>();
                        rankingCard = firstMatch.value > currentCardInCheck.value ? firstMatch : currentCardInCheck;
                        tieBreakerCards.Add(firstMatch.value < currentCardInCheck.value ? firstMatch : currentCardInCheck);
                        //tieBreakerCards.Add(firstMatch);
                        //tieBreakerCards.Add(currentCardInCheck);
                        foreach (Card card in hand)
                        {
                            if (card.value != firstMatch.value && card.value != currentCardInCheck.value)
                            {
                                tieBreakerCards.Add(card);
                                return true;
                            }
                        }
                    }
                }
            }
        }


        return false;
    }
    bool Pairs(List<Card> hand)
    {
        for (int i = 0; i < 5; i++)
        {
            matchCount = 0;
            currentCardInCheck = hand[i];

            foreach (Card card in hand)
            {
                if (currentCardInCheck.Equals(card))
                    continue;
                if (currentCardInCheck.value == card.value)
                    matchCount++;


            }
            if (matchCount == 1)
            {
                tieBreakerCards = new List<Card>();
                rankingCard = currentCardInCheck;
                //tieBreakerCards.Add(currentCardInCheck);
                foreach(Card card in hand)
                {
                    if (card.value != rankingCard.value)
                        tieBreakerCards.Add(card);
                }
                return true;
            }
        }
        return false;
    }
    void High(List<Card> hand)
    {
        tieBreakerCards = new List<Card>();
        rankingCard = hand.Max();
        foreach(Card card in hand)
        {
            if (card.value != rankingCard.value)
                tieBreakerCards.Add(card);
        }
    }
}
