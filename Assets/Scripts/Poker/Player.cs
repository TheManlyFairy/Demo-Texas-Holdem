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
    public int money = 300;
    public bool hasChosenAction = false;
    public PlayStatus playStatus;

    int totalAmountBetThisRound = 0;
    int amountToBet = 0;
    #region Properties
    public int AmountToBet { set { amountToBet = value; } }
    public int TotalBetThisRound { get { return totalAmountBetThisRound; } }
    #endregion
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

        while (cards.Count < 5)
        {
            Draw();
        }

        SetupHand();
        hasChosenAction = true;
    }
    public void OpeningBet()
    {
        money -= Dealer.MinimumBet;
    }
    public void Raise()
    {
        hasChosenAction = true;
        /*if(money + totalAmountBetThisRound > Dealer.HighestBetMade)
        {
            //Debug.Log("Betting " + amountToBet);
            totalAmountBetThisRound += amountToBet;
            money -= amountToBet;
            Dealer.AddBet(amountToBet);
        }
        else
        {
            totalAmountBetThisRound += money;
            amountToBet = money;
            Debug.Log(name+ "IS GOING ALL IN WITH " + amountToBet+"!");
            Dealer.AddBet(money);
            money = 0;
            playStatus = PlayStatus.AllIn;
        }*/
        if (amountToBet < Dealer.MinimumBet)
            amountToBet = Dealer.HighestBetMade + Dealer.MinimumBet;
        else
            amountToBet += Dealer.HighestBetMade - totalAmountBetThisRound;

        if (amountToBet == money)
        {
            Debug.Log(name + " IS GOING ALL IN WITH " + amountToBet + "!");
        }
        else
        {
            Debug.Log(name + " raised the stakes by " + (amountToBet - Dealer.HighestBetMade - totalAmountBetThisRound));
        }
        totalAmountBetThisRound += amountToBet;
        money -= amountToBet;
        Dealer.AddBet(amountToBet);
    }
    public void Call()
    {
        hasChosenAction = true;
        if(amountToBet+money < Dealer.HighestBetMade)
        {
            amountToBet = money;
            playStatus = PlayStatus.AllIn;
        }
        else
        {
            amountToBet = Dealer.HighestBetMade - TotalBetThisRound;
            playStatus = PlayStatus.Checked;
        }
        totalAmountBetThisRound += amountToBet;
        Dealer.AddBet(amountToBet);
        money -= amountToBet;

        Debug.Log(name + " added " + amountToBet + " and called");
    }
    public void Check()
    {
        hasChosenAction = true;
        Debug.Log(name + " checked");
        playStatus = PlayStatus.Checked;
    }
    public void Fold()
    {
        Debug.Log(name + " folded and is no longer in play");
        hasChosenAction = true;
        playStatus = PlayStatus.Folded;
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
