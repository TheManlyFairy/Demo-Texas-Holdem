using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Utilities;
using UnityEngine.UI;
public class Player : MonoBehaviour
{
    public List<Card> cards = new List<Card>();
    public TexasPokerHand hand;

    public bool hasDiscardedCards;
    public PlayStatus playStatus;
    private void Start()
    {
        hand = new TexasPokerHand();
        playStatus = PlayStatus.Betting;
        //DebugShowPlayerHand();
    }
    public void Draw()
    {
        Card card = Dealer.Pull();
        cards.Add(card);
    }
    public void Discard()
    {
        cards = cards.Where(card => !card.markedForDiscard).ToList();

        while(cards.Count<5)
        {
            Draw();
        }

        SetupHand();
        hasDiscardedCards = true;
    }
    public void SetupHand()
    {

        cards.Sort(new CompareCardsByValue());
        //hand.SetHandStrength(cards);
       /* Debug.LogWarning(name + "'s poker hand is " + hand.strength);
        Debug.Log(name + "'s ranking card value is " + hand.rankingCard.name);
        Debug.Log(name + "'s tie breakers are: ");
        foreach (Card card in hand.tieBreakerCards) { Debug.Log(card.name);  }*/

    }
    public void DebugGetCardSprite()
    {

    }
    public void SetHandStrength()
    {
        hand.GetHandStrength(cards);
    }
}
