using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Utilities;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class Player : MonoBehaviourPunCallbacks//, IOnEventCallback
{
    public List<Card> cards = new List<Card>();
    public TexasPokerHand hand;
    public int money;
    public bool hasChosenAction = false;
    public PlayStatus playStatus;
    public PlayerDisplay playerSeat;

    int totalAmountBetThisRound = 0;
    int amountToBet = 0;
    #region Properties
    public int AmountToBet { get { return amountToBet; } set { amountToBet = value; } }
    public int TotalBetThisRound { get { return totalAmountBetThisRound; } }
    #endregion

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    private void Start()
    {
        hand = new TexasPokerHand();
        playStatus = PlayStatus.Betting;
        //DebugShowPlayerHand();

        //  SendViewIdToServer();
    }
    public void Draw()
    {
        Card card = Dealer.Pull();
        cards.Add(card);
    }

    public void OpeningBet()
    {
        money -= Dealer.MinimumBet;
        totalAmountBetThisRound = Dealer.MinimumBet;
        playerSeat.UpdatePlayerMoney(totalAmountBetThisRound, money);
        UpdateClientMoney();
    }
    public void Raise(int amountToRaise)
    {
        hasChosenAction = true;

        int minimumRequiredRaise = Dealer.HighestBetMade - TotalBetThisRound + Dealer.MinimumBet;

        /*//if the first player at the beginning of the game selects to raise, the highestBeMade and TotalBetThisRound are equal, so minimum bet is doubled
        if (minimumRequiredRaise == Dealer.MinimumBet)
            minimumRequiredRaise *= 2;*/

       //if the first player at the beginning of the game selects to raise and didnt move his raise slider, the amountToRaise is sent as 0;
        if (amountToRaise < minimumRequiredRaise)
            amountToRaise = minimumRequiredRaise;

        if (amountToRaise == money)
        {
            playStatus = PlayStatus.AllIn;
            Debug.Log(name + " IS GOING ALL IN WITH " + amountToRaise + "!");
        }
        else
        {
            Debug.Log(name + " raised the stakes by " + amountToRaise);
        }
        totalAmountBetThisRound += amountToRaise;
        money -= amountToRaise;
        amountToBet = amountToRaise;
        playerSeat.UpdatePlayerMoney(totalAmountBetThisRound, money);
        UpdateClientMoney();
    }
    public void Call()
    {
        hasChosenAction = true;

        if (Dealer.HighestBetMade >= money + totalAmountBetThisRound)
        {
            amountToBet = money;
            totalAmountBetThisRound += money;
            money = 0;
            playStatus = PlayStatus.AllIn;
            Debug.Log(name + "IS GOING ALL IN");
        }
        else
        {
            amountToBet = Dealer.HighestBetMade - totalAmountBetThisRound;
            money -= amountToBet;
            totalAmountBetThisRound += amountToBet;
        }
        playerSeat.UpdatePlayerMoney(totalAmountBetThisRound, money);
        UpdateClientMoney();
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
        playerSeat.GreyOutIcon();
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
    public void SetHandStrength()
    {
        hand.GetHandStrength(cards);
    }
    public void AddWinningsToMoney(int moneyEarned)
    {
        money += moneyEarned;
        playerSeat.UpdatePlayerMoney(totalAmountBetThisRound, money);
        UpdateClientMoney();
    }
    void UpdateClientMoney()
    {
        object[] datas = new object[] { photonView.ViewID, money, totalAmountBetThisRound };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions()
        {
            Receivers = ReceiverGroup.Others,
            CachingOption = EventCaching.DoNotCache
        };
        SendOptions sendOptions = new SendOptions() { Reliability = false };

        PhotonNetwork.RaiseEvent((byte)EventCodes.UpdateCurrentPlayerMoney, datas, raiseEventOptions, sendOptions);
    }
    public void PlayerTurnUpdate()
    {
        object[] datas = new object[] { true, photonView.ViewID };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions()
        {
            Receivers = ReceiverGroup.Others,
            CachingOption = EventCaching.DoNotCache
        };
        SendOptions sendOptions = new SendOptions() { Reliability = false };

        PhotonNetwork.RaiseEvent((byte)EventCodes.PlayerTurn, datas, raiseEventOptions, sendOptions);
    }
    void CreateLocalPlayerCard(object[] data)
    {
        Card newCard;
        CardValue value = (CardValue)data[0];
        newCard = ScriptableObject.CreateInstance<Card>();
        newCard.name = value + " of " + (CardSuit)data[1];
        newCard.value = value;
        newCard.suit = (CardSuit)data[1];
        Dealer.SetCardSprite(newCard);
        cards.Add(newCard);
        Debug.Log("Player " + name + " Recieved card " + (CardValue)data[0] + " of " + (CardSuit)data[1]);
    }

    /* Unused Code
     * //void SendViewIdToServer()
    //{
    //    if (photonView.IsMine)
    //    {
    //        object[] datas = new object[] { photonView.ViewID };
    //        RaiseEventOptions raiseEventOptions = new RaiseEventOptions()
    //        {
    //            Receivers = ReceiverGroup.MasterClient
    //        };
    //        SendOptions sendOptions = new SendOptions() { Reliability = false };

    //        PhotonNetwork.RaiseEvent((byte)EventCodes.PlayerViewId, datas, raiseEventOptions, sendOptions);
    //    }
    //}
     * public void OnEvent(EventData photonEvent)
     {
         byte eventCode = photonEvent.Code;

         //if (eventCode == (byte)EventCodes.PlayerCards && photonView.IsMine)
         //{
         //    object[] data = (object[])photonEvent.CustomData;
         //    CreateLocalPlayerCard(data);
         //}
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
     }*/
}
