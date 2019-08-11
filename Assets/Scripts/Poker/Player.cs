using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Utilities;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;



public class Player : MonoBehaviourPunCallbacks, IOnEventCallback
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

        SendViewIdToServer();
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

    void SendViewIdToServer()
    {
        if (photonView.IsMine)
        {
            object[] datas = new object[] { photonView.ViewID };
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions()
            {
                Receivers = ReceiverGroup.MasterClient
            };
            SendOptions sendOptions = new SendOptions() { Reliability = false };

            PhotonNetwork.RaiseEvent((byte)EventCodes.PlayerViewId, datas, raiseEventOptions, sendOptions);
        }
        
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;

        if (eventCode == (byte)EventCodes.PlayerCards && photonView.IsMine)
        {
            object[] data = (object[])photonEvent.CustomData;
            Card newCard;
            CardValue value = (CardValue)data[0];
            newCard = ScriptableObject.CreateInstance<Card>();
            newCard.name = value + " of " + (CardSuit)data[1];
            newCard.value = value;
            newCard.suit = (CardSuit)data[1];
            Dealer.dealerRef.SetCardSprite(newCard);
            cards.Add(newCard);
            Debug.Log("Player " + this.photonView.ViewID + " Recieved card " + (CardValue)data[0] + " of " + (CardSuit)data[1]);
        }
    }
}
