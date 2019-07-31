using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Utilities;
public class TexasPokerHand
{
    public List<Card> allCards, tieBreakerCards;
    public Hand strength;
    public Card rankingCard;


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

        foreach (Card card in hand)
        {
            if (distinctValues.Add(card.value))
                distinctList.Add(card);
        }
        return distinctList;
    }

    #endregion

    #region PokerHand
    public void GetHandStrength(List<Card> playerCards)
    {
        allCards = new List<Card>();
        allCards.AddRange(playerCards);
        allCards.AddRange(Dealer.dealerRef.communityCards);
        allCards.Sort(new CompareCardsByValue());

        if (RoyalFlush(allCards)) { strength= Hand.Royal; return; }
        if (StraightFlush(allCards)) { strength = Hand.StraightFlush; return; }
        if (FourKind(allCards)) { strength = Hand.FourKind; return; }
        if (FullHouse(allCards)) {; strength = Hand.FullHouse; return; }
        if (Flush(allCards)) {; strength = Hand.Flush; return; }
        if (Straight(allCards)) {; strength = Hand.Straight; return; }
        if (ThreeKind(allCards)) {; strength = Hand.ThreeKind; return; }
        if (TwoPairs(allCards)) {; strength = Hand.TwoPair; return; }
        if (Pair(allCards)) {; strength = Hand.Pair; return; }
        High(allCards); strength = Hand.High;
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
                tieBreakerCards = new List<Card>();
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
                tieBreakerCards = new List<Card>();
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
                tieBreakerCards = new List<Card>();
                tieBreakerCards.Add(hand.Where(card => card.value != checkedCard.value).First());
                return true;
            }

        }

        return false;
    }
    bool FullHouse(List<Card> hand)
    {
        List<Card> matchList = new List<Card>();
        Card matchThree = null;
        Card checkedCard;
        int matchCount;

        for (int i = 0; i < hand.Count - 3; i++)
        {
            matchCount = 0;
            checkedCard = hand[i];
            foreach (Card card in hand)
            {
                if (card.Equals(checkedCard))
                    continue;
                if (card.value == checkedCard.value)
                    matchCount++;
            }
            if (matchCount == 2)
            {
                matchThree = checkedCard;
                break;
            }
        }

        if (matchThree != null)
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
            if (matchList.Count >= 1)
            {
                rankingCard = matchThree;
                tieBreakerCards = new List<Card>();
                tieBreakerCards.Add(matchList[0]);
                return true;
            }
        }


        return false;
    }
    bool Flush(List<Card> hand)
    {
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
                if (card.suit == checkedCard.suit)
                    matchList.Add(card);
            }
            if (matchList.Count >= 4)
            {
                rankingCard = checkedCard;
                tieBreakerCards = new List<Card>();
                tieBreakerCards.AddRange(matchList.GetRange(i, 4));
                return true;
            }

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
        for (int i = 0; i <= hand.Count - 3; i++)
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
            if (matchList.Count == 2)
            {
                rankingCard = checkedCard;
                tieBreakerCards = new List<Card>();
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
        for (int i = 0; i < hand.Count - 2; i++)
        {
            checkedCard = hand[i];
            matchCount = 0;
            foreach (Card card in hand)
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

        if (matchList.Count >= 2)
        {
            rankingCard = matchList[0];
            tieBreakerCards = new List<Card>();
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
                tieBreakerCards = new List<Card>();
                tieBreakerCards.AddRange((hand.Where(card => card.value != checkedCard.value).ToList().GetRange(0, 3)));
                return true;
            }
        }
        return false;
    }
    void High(List<Card> hand)
    {
        rankingCard = hand[0];
        tieBreakerCards = new List<Card>();
        tieBreakerCards.AddRange(hand.GetRange(1, 4));
    }
    #endregion
}
