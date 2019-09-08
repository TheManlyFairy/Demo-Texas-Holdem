using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Utilities;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Photon.Pun;

public class Dealer : MonoBehaviourPunCallbacks, IOnEventCallback
{
    #region Variables
    public static event CommunityCardsUpdate OnCommunityUpdate;
    public static event InterfaceUpdate OnInterfaceUpdate;
    public static List<Player> bettingPlayers;
    public List<Sprite> deckSprites;
    public List<Player> DebugShowBettingPlayers;

    static Card[] communityCards;
    static List<Card> deck;
    public static Dealer dealerRef;
    static int minimumBet;
    static int currentBetToMatch;
    static int pot = 0;
    static bool finalBettingRound;
    #endregion
    #region Properties
    public static Card[] CommunityCards { get { return communityCards; } }
    public static int HighestBetMade { get { return currentBetToMatch; } }
    public static int Pot { get { return pot; } }
    public static int MinimumBet { get { return minimumBet; } }

    #endregion
    //List<Player> players;

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

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
        deck = new List<Card>();
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
                deck.Add(newCard);
            }
        }
        //Debug.Log("Built deck of " + cards.Count + " cards");
    }
    void ShuffleDeck()
    {
        Card temp;
        int randomIndex;
        for (int i = 0; i < deck.Count; i++)
        {
            randomIndex = Random.Range(0, deck.Count);
            temp = deck[randomIndex];
            deck[randomIndex] = deck[i];
            deck[i] = temp;

        }
    }
    void DealCards()
    {
        for (int i = 0; i < 2; i++)
        {
            foreach (Player p in PhotonGameManager.players)
            {
                p.Draw();
                UpdateNetworkPlayers(p, i);
            }
        }
    }
    public static void CommunityPull()
    {
        if (communityCards[0] == null)
        {
            Debug.LogWarning("Flop betting round!");
            communityCards[0] = Pull();
            communityCards[1] = Pull();
            communityCards[2] = Pull();
            if (OnCommunityUpdate != null)
                OnCommunityUpdate();

            dealerRef.StartCoroutine(dealerRef.BettingRound());
        }
        else if (communityCards[3] == null)
        {
            Debug.LogWarning("Turn betting round!");
            communityCards[3] = Pull();
            if (OnCommunityUpdate != null)
                OnCommunityUpdate();

            dealerRef.StartCoroutine(dealerRef.BettingRound());
        }
        else
        {
            Debug.LogWarning("River betting round! Last Round!");
            finalBettingRound = true;
            communityCards[4] = Pull();
            if (OnCommunityUpdate != null)
                OnCommunityUpdate();
            PhotonGameManager.DeclareWinner(bettingPlayers);
            dealerRef.StartCoroutine(dealerRef.BettingRound());
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
        Card drawnCard = deck[0];
        deck.RemoveAt(0);
        return drawnCard;
    }
    public static void AddBet(int bet)
    {
        pot += bet;
        UIManager.instance.UpdatePot();
        currentBetToMatch = currentBetToMatch > PhotonGameManager.CurrentPlayer.TotalBetThisRound ? currentBetToMatch : PhotonGameManager.CurrentPlayer.TotalBetThisRound;
        Debug.Log("Highest bet is now: " + currentBetToMatch);

        if (OnInterfaceUpdate != null)
            OnInterfaceUpdate();
    }
    public static void StartBettingRound()
    {
        minimumBet = 5;
        currentBetToMatch = 0;
        UpdateClientDealer();
        bettingPlayers = new List<Player>();
        bettingPlayers.AddRange(PhotonGameManager.players);
        dealerRef.DebugShowBettingPlayers = bettingPlayers;
        foreach (Player p in bettingPlayers)
        {
            p.OpeningBet();
        }
        UpdatePlayerMoney(-minimumBet);
        pot = minimumBet * bettingPlayers.Count;
        UIManager.instance.UpdatePot();
        UpdateClientDealer();
        Debug.LogWarning("First betting round!");
        dealerRef.StartCoroutine(dealerRef.BettingRound());
    }
    IEnumerator BettingRound()
    {
        ResetPlayerActions();
        ParsePlayersStillBetting();
        UpdateClientDealer();
        while (!AllPlayersDoneBetting() && bettingPlayers.Count > 1)
        {
            ResetPlayerActions();

            foreach (Player player in bettingPlayers)
            {

                if (player.playStatus == PlayStatus.AllIn)
                {
                    //Debug.Log(player.name + "is all in and cannot bet any more.");
                    continue;
                }

                PhotonGameManager.CurrentPlayer = player;
                PhotonGameManager.CurrentPlayer.PlayerTurnUpdate();
                Debug.Log(player.photonView.ViewID + "'s turn: ");

                if (OnInterfaceUpdate != null)
                    OnInterfaceUpdate();

                while (player.playStatus == PlayStatus.Betting)
                {
                    //Debug.Log("Stuck in a loop?");
                    yield return null;
                }
                ParsePlayersStillBetting();
                if (bettingPlayers.Count == 1)
                    break;
            }
            yield return null;
        }

        if (bettingPlayers.Count == 1 || finalBettingRound)
        {
            PhotonGameManager.DeclareWinner(bettingPlayers);
        }
        else
        {
            CommunityPull();
        }
    }

    void ParsePlayersStillBetting()
    {
        List<Player> playersStillInGame = new List<Player>();
        foreach (Player p in bettingPlayers)
        {
            if (p.playStatus != PlayStatus.Folded)
                playersStillInGame.Add(p);
        }
        bettingPlayers = playersStillInGame;
    }
    void ResetPlayerActions()
    {
        foreach (Player p in bettingPlayers)
        {
            p.hasChosenAction = false;
            p.playStatus = PlayStatus.Betting;
        }
    }
    bool AllPlayersDoneBetting()
    {
        foreach (Player p in bettingPlayers)
        {
            if (p.playStatus == PlayStatus.AllIn)
                continue;

            if (p.TotalBetThisRound < currentBetToMatch)
            {
                Debug.Log(p + " hasn't matched the bet yet");
                return false;
            }
            if (p.playStatus == PlayStatus.Betting)
            {
                Debug.Log(p.photonView.ViewID + " is still in the betting status");
                return false;
            }

        }
        return true;
    }

    /*Unused Methods
     * public static void AddCard(Card card)
    {

    }
    public static void RemoveCard(Card card)
    {

    }*/

    #region Raise Events
    // Sends player's cards to player's cliet app.
    void UpdateNetworkPlayers(Player player, int cardIndex)
    {
        CardValue tempCardValue = player.cards[cardIndex].value;
        CardSuit tempCardSuit = player.cards[cardIndex].suit;

        object[] datas = new object[] { player.photonView.ViewID, tempCardValue, tempCardSuit };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions()
        {
            Receivers = ReceiverGroup.Others,
            CachingOption = EventCaching.DoNotCache
        };
        SendOptions sendOptions = new SendOptions() { Reliability = false };

        PhotonNetwork.RaiseEvent((byte)EventCodes.PlayerCards, datas, raiseEventOptions, sendOptions);

    }

    static void UpdatePlayerMoney(int amount)
    {
        object[] datas = new object[] { amount };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions()
        {
            Receivers = ReceiverGroup.Others,
            CachingOption = EventCaching.DoNotCache
        };
        SendOptions sendOptions = new SendOptions() { Reliability = false };

        PhotonNetwork.RaiseEvent((byte)EventCodes.PlayerBet, datas, raiseEventOptions, sendOptions);

    }

    static void UpdateClientDealer()
    {
        object[] datas = new object[] { minimumBet, currentBetToMatch, pot };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions()
        {
            Receivers = ReceiverGroup.Others,
            CachingOption = EventCaching.DoNotCache
        };
        SendOptions sendOptions = new SendOptions() { Reliability = false };

        PhotonNetwork.RaiseEvent((byte)EventCodes.ClientDealer, datas, raiseEventOptions, sendOptions);
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;

        switch (eventCode)
        {
            case (byte)EventCodes.PlayerRaise:
                {
                    object[] data = (object[])photonEvent.CustomData;
                    int betToAdd = (int)data[1];
                    AddBet(betToAdd);
                    PhotonGameManager.CurrentPlayer.money -= betToAdd;
                    PhotonGameManager.CurrentPlayer.playStatus = (PlayStatus)data[0];
                    PhotonGameManager.CurrentPlayer.playerSit.UpdatePlayerMoney(betToAdd, PhotonGameManager.CurrentPlayer.money);
                    Debug.Log(PhotonGameManager.CurrentPlayer.photonView.ViewID+" raised by " + betToAdd);

                }
                break;
            case (byte)EventCodes.PlayerCall:
                {
                    object[] data = (object[])photonEvent.CustomData;
                    int betToAdd = (int)data[1];
                    AddBet(betToAdd);
                    PhotonGameManager.CurrentPlayer.money -= betToAdd;
                    PhotonGameManager.CurrentPlayer.playStatus = (PlayStatus)data[0];
                    PhotonGameManager.CurrentPlayer.playerSit.UpdatePlayerMoney(betToAdd, PhotonGameManager.CurrentPlayer.money);
                    Debug.Log(PhotonGameManager.CurrentPlayer.photonView.ViewID + " has called");
                }
                break;

            case (byte)EventCodes.PlayerCheck:
                {
                    object[] data = (object[])photonEvent.CustomData;
                    PhotonGameManager.CurrentPlayer.playStatus = (PlayStatus)data[0];
                    Debug.Log(PhotonGameManager.CurrentPlayer.photonView.ViewID + " has checked");
                }
                break;

            case (byte)EventCodes.PlayerFold:
                {
                    object[] data = (object[])photonEvent.CustomData;
                    PhotonGameManager.CurrentPlayer.playStatus = (PlayStatus)data[0];
                    Debug.Log(PhotonGameManager.CurrentPlayer.photonView.ViewID + " has folded");
                }
                break;
        }
    }
    #endregion
}
