using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Utilities;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Photon.Pun;

public class Dealer : MonoBehaviourPun
{
    public static Dealer dealerRef;
    public static event CommunityCardsUpdate OnCommunityUpdate;
    public List<Card> cards;
    public List<Sprite> deckSprites;
    public Card[] communityCards;

    public int pot=0;
    //List<Player> players;

    private void Awake()
    {
        dealerRef = this;
    }

    public static void StartGame()
    {
        dealerRef.BuildDeck();
        dealerRef.ShuffleDeck();
        dealerRef.DealCards();
    }
    void BuildDeck()
    {
        cards = new List<Card>();
        communityCards = new Card[5];
        CardValue value;
        CardSuit suit;
        Card newCard;
        for (int i = 0; i < 4; i++)
        {
            suit = (CardSuit)i;
            for (int j = 2; j < 15; j++)
            {
                value = (CardValue)j;
                newCard = ScriptableObject.CreateInstance<Card>();
                newCard.name = value + " of " + suit;
                newCard.value = value;
                newCard.suit = suit;
                SetCardSprite(newCard);
                dealerRef.cards.Add(newCard);
            }
        }
        //Debug.Log("Built deck of " + cards.Count + " cards");
    }
    void ShuffleDeck()
    {
        Card temp;
        int randomIndex;
        for (int i = 0; i < cards.Count; i++)
        {
            randomIndex = Random.Range(0, cards.Count);
            temp = cards[randomIndex];
            cards[randomIndex] = cards[i];
            cards[i] = temp;

        }
    }
    void DealCards()
    {
        for (int i = 0; i < 2; i++)
        {
            foreach (Player p in PhotonGameManager.players)
            {
                p.Draw();
               // if(p.photonView.IsMine)
                UpdateNetworkPlayers(p,i);
            }
        }
    }
    public void CommunityPull()
    {
        if(communityCards[0]==null)
        {
            communityCards[0] = Pull();
            communityCards[1] = Pull();
            communityCards[2] = Pull();
            if (OnCommunityUpdate != null)
                OnCommunityUpdate();
        }
        else if(communityCards[3] == null)
        {
            communityCards[3] = Pull();
            if (OnCommunityUpdate != null)
                OnCommunityUpdate();
        }
        else
        {
            communityCards[4] = Pull();
            if (OnCommunityUpdate != null)
                OnCommunityUpdate();
            PhotonGameManager.instance.DeclareWinner();
        }
    }
   public void SetCardSprite(Card card)
    {
        int indexer;
        if (card.value == CardValue.Ace)
        {
            indexer = (int)card.suit * 13;
            card.sprite = deckSprites[(int)card.suit * 13];
        }

        else
        {
            indexer = (int)card.suit * 13 + (int)card.value - 1;
            card.sprite = deckSprites[(int)card.suit * 13 + (int)card.value - 1];
        }

    }
    public static Card Pull()
    {
        Card drawnCard = dealerRef.cards[0];
        dealerRef.cards.RemoveAt(0);
        return drawnCard;
    }


    /*Unused Methods
     * public static void AddCard(Card card)
    {

    }
    public static void RemoveCard(Card card)
    {

    }*/

    void UpdateNetworkPlayers(Player player,int cardIndex)
    {
       CardValue tempCardValue =  player.cards[cardIndex].value;
       CardSuit tempCardSuit = player.cards[cardIndex].suit;

        object[] datas = new object[] { tempCardValue,tempCardSuit };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions()
        {
            Receivers = ReceiverGroup.Others,
            CachingOption = EventCaching.DoNotCache
        };
        SendOptions sendOptions = new SendOptions() { Reliability = false };

        PhotonNetwork.RaiseEvent((byte)EventCodes.PlayerCards, datas, raiseEventOptions, sendOptions);

    }
}
